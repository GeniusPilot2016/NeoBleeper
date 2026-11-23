using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeoBleeper
{
    /// <summary>
    /// Lists suitable text models, generates text, and counts input tokens using
    /// the OpenAI, Google Gemini, Anthropic, and OpenRouter APIs.
    /// Requires the existing Settings1 and EncryptionHelper types.
    /// Targets .NET 6+ (C# 10+).
    /// </summary>
    public class CommonAIEngine
    {
        // Keep original property names and Gemini's "models/..." identifier format.
        // Anthropic and OpenRouter expose thinking-capability metadata.
        public record Model(string name, string displayName, string provider)
        {
            // Empty means no capabilities metadata; apply conservative name fallback.
            // Values: "adaptive", "enabled", or "none" (explicitly unsupported).
            public string ThinkingMode { get; init; } = string.Empty;
        }

        private const string AnthropicApiVersion = "2026-10-21";
        private static readonly HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        private static readonly string[] WantedFeatures =
        {
            "generateContent", "countTokens"
        };

        private static readonly string[] UnwantedFeatures =
        {
            "predict", "embedContent", "embedText",
            "asyncBatchEmbedContent", "generateAnswer"
        };

        // Heuristics only: OpenAI's /models endpoint does not report generation methods.
        private static readonly string[] OpenAiExcludedTerms =
        {
            "audio", "tts", "speech", "transcrib", "diariz", "whisper",
            "realtime", "live", "image", "video", "sora", "dall-e",
            "embedding", "moderation", "search", "codex", "computer-use",
            "deep-research"
        };

        private static readonly string[] GeminiExcludedTerms =
        {
            "computer-use", "robotics", "code", "image", "transcrib",
            "nano-banana", "nano banana", "tts", "audio", "omni",
            "live", "realtime", "video", "gemini-2.0"
        };

        /// <summary>
        /// provider: "Gemini", "OpenAI", "Anthropic", "OpenRouter", "All" (also null/empty).
        /// Missing API keys cause that provider to be skipped when "All" is selected.
        /// Explicitly requesting OpenRouter requires a configured API key.
        /// API errors are propagated rather than silently returning an incomplete list.
        /// </summary>
        public static async Task<List<Model>> GetModelList(
            string provider, CancellationToken cancellationToken = default)
        {
            string selection = NormalizeProvider(provider, allowAll: true);
            var result = new List<Model>();

            if (selection == "All" || selection == "Google")
            {
                string key = GetDecryptedKey("Google", required: false);
                if (!string.IsNullOrEmpty(key))
                    result.AddRange(await ListGeminiAsync(key, cancellationToken)
                        .ConfigureAwait(false));
            }

            if (selection == "All" || selection == "OpenAI")
            {
                string key = GetDecryptedKey("OpenAI", required: false);
                if (!string.IsNullOrEmpty(key))
                    result.AddRange(await ListOpenAiAsync(key, cancellationToken)
                        .ConfigureAwait(false));
            }

            if (selection == "All" || selection == "Anthropic")
            {
                string key = GetDecryptedKey("Anthropic", required: false);
                if (!string.IsNullOrEmpty(key))
                    result.AddRange(await ListAnthropicAsync(key, cancellationToken)
                        .ConfigureAwait(false));
            }

            if (selection == "All" || selection == "OpenRouter")
            {
                // Explicit OpenRouter requests require an API key.
                // "All" skips providers without a configured key.
                string key = GetDecryptedKey("OpenRouter",
                    required: selection == "OpenRouter");
                if (!string.IsNullOrWhiteSpace(key))
                    result.AddRange(await ListOpenRouterAsync(key, cancellationToken)
                        .ConfigureAwait(false));
            }

            return result
                .GroupBy(model => model.provider + "\u0000" + model.name,
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
        }

        private static async Task<IEnumerable<Model>> ListOpenRouterAsync(
            string key, CancellationToken cancellationToken)
        {
            // Include all text-output models, including proprietary models that
            // also accept image/audio inputs. Authentication remains required.
            using JsonDocument json = await SendJsonAsync(
                HttpMethod.Get, "https://openrouter.ai/api/v1/models?output_modalities=text",
                "OpenRouter", key, null, cancellationToken).ConfigureAwait(false);

            if (!TryGetArray(json.RootElement, "data", out JsonElement models))
                throw new InvalidOperationException(
                    "OpenRouter model catalog did not contain a 'data' array.");

            var result = new List<Model>();
            foreach (JsonElement item in models.EnumerateArray())
            {
                if (!IsOpenRouterTextModel(item))
                    continue;

                string id = GetString(item, "id");
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                string display = GetString(item, "name");
                bool supportsReasoning = false;
                if (TryGetArray(item, "supported_parameters", out JsonElement parameters))
                {
                    foreach (JsonElement parameter in parameters.EnumerateArray())
                    {
                        if (parameter.ValueKind == JsonValueKind.String &&
                            string.Equals(parameter.GetString(), "reasoning",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            supportsReasoning = true;
                            break;
                        }
                    }
                }

                result.Add(new Model(id,
                    string.IsNullOrWhiteSpace(display) ? id : display,
                    "OpenRouter")
                {
                    ThinkingMode = supportsReasoning ? "enabled" : "none"
                });
            }

            // Never report an empty list as a successful catalog load: it would
            // otherwise look like the UI failed to populate the model dropdown.
            if (result.Count == 0)
                throw new InvalidOperationException(
                    "OpenRouter returned " + models.GetArrayLength() +
                    " catalog entries, but none passed the chat/text model filter. " +
                    "Check the /models response schema and IsOpenRouterTextModel.");

            return result
                .OrderBy(model => model.displayName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(model => model.name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// True when this model family has an explicit, supported thinking control.
        /// The model-list endpoints do not consistently expose reasoning capabilities;
        /// unknown models are kept off rather than sending unsupported parameters.
        /// </summary>
        public static bool SupportsExtendedThinking(Model model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.name))
                return false;

            string provider;
            try
            {
                // Accept provider aliases consistently with the rest of the engine.
                provider = NormalizeProvider(model.provider, allowAll: false);
            }
            catch (ArgumentException)
            {
                return false;
            }

            switch (provider)
            {
                case "Google":
                    return IsGeminiThinkingLevelModel(model.name);
                case "OpenAI":
                    return IsOpenAiReasoningModel(model.name);
                case "Anthropic":
                    return GetAnthropicThinkingMode(model) != AnthropicThinkingMode.None;
                case "OpenRouter":
                    // Prefer the catalog's supported_parameters; use a narrow model-ID
                    // fallback only for manually constructed Model instances.
                    if (!string.IsNullOrWhiteSpace(model.ThinkingMode))
                        return model.ThinkingMode == "enabled";
                    string id = model.name.Trim();
                    if (id.StartsWith("openai/", StringComparison.OrdinalIgnoreCase))
                        return IsOpenAiReasoningModel(id.Substring("openai/".Length));
                    if (id.StartsWith("anthropic/", StringComparison.OrdinalIgnoreCase))
                        return GetAnthropicThinkingMode(id.Substring("anthropic/".Length)) !=
                            AnthropicThinkingMode.None;
                    if (id.StartsWith("google/", StringComparison.OrdinalIgnoreCase))
                        return IsGeminiThinkingLevelModel(id.Substring("google/".Length));
                    return false;
                default:
                    return false;
            }
        }

        private static bool IsOpenAiReasoningModel(string name)
        {
            string id = (name ?? string.Empty).Trim().ToLowerInvariant();
            // Do not send Responses API reasoning options to GPT-4.x, chat, search,
            // or miscellaneous specialized models, which may reject them.
            if (id.Contains("-chat") || id.Contains("-search")) return false;
            return Regex.IsMatch(id, @"^gpt-[56](?:[.\-]|$)") ||
                   Regex.IsMatch(id, @"^o(?:1|3|4)(?:-|$)");
        }

        private enum AnthropicThinkingMode { None, Manual, Adaptive }

        private static AnthropicThinkingMode GetAnthropicThinkingMode(Model model)
        {
            // Prefer exact capabilities from the Anthropic Models API. A "none"
            // response deliberately overrides optimistic model-name heuristics.
            switch (model.ThinkingMode)
            {
                case "adaptive": return AnthropicThinkingMode.Adaptive;
                case "enabled": return AnthropicThinkingMode.Manual;
                case "none": return AnthropicThinkingMode.None;
                default: return GetAnthropicThinkingMode(model.name);
            }
        }

        // Returns an empty string when the API does not include this metadata.
        private static string GetAnthropicThinkingModeFromApi(JsonElement modelInfo)
        {
            if (!TryGetObject(modelInfo, "capabilities", out JsonElement capabilities) ||
                !TryGetObject(capabilities, "thinking", out JsonElement thinking))
                return string.Empty;
            if (thinking.TryGetProperty("supported", out JsonElement overall) &&
                overall.ValueKind == JsonValueKind.False)
                return "none";
            if (!TryGetObject(thinking, "types", out JsonElement types))
                return string.Empty;
            // Earlier API versions may omit these fields altogether. Only a
            // definitive response should override the model-family fallback.
            bool hasAdaptive = TryGetObject(types, "adaptive", out _);
            bool hasEnabled = TryGetObject(types, "enabled", out _);
            if (!hasAdaptive && !hasEnabled) return string.Empty;
            if (IsAnthropicThinkingTypeSupported(types, "adaptive")) return "adaptive";
            if (IsAnthropicThinkingTypeSupported(types, "enabled")) return "enabled";
            return "none";
        }

        private static bool IsAnthropicThinkingTypeSupported(JsonElement types, string name)
        {
            return TryGetObject(types, name, out JsonElement capability) &&
                capability.TryGetProperty("supported", out JsonElement supported) &&
                supported.ValueKind == JsonValueKind.True;
        }

        private static AnthropicThinkingMode GetAnthropicThinkingMode(string name)
        {
            string id = (name ?? string.Empty).Trim().ToLowerInvariant();
            int major;
            int minor;

            // Older Anthropic identifier: claude-3-7-sonnet-20250219.
            Match older = Regex.Match(id,
                @"^claude-(?<major>\d+)[-.](?<minor>\d+)-(?:opus|sonnet|haiku)(?:-|$)");
            if (older.Success)
            {
                major = int.Parse(older.Groups["major"].Value,
                    System.Globalization.CultureInfo.InvariantCulture);
                minor = int.Parse(older.Groups["minor"].Value,
                    System.Globalization.CultureInfo.InvariantCulture);
                return major == 3 && minor >= 7
                    ? AnthropicThinkingMode.Manual
                    : major >= 4
                        ? (major > 4 || minor >= 6
                            ? AnthropicThinkingMode.Adaptive
                            : AnthropicThinkingMode.Manual)
                        : AnthropicThinkingMode.None;
            }

            // New identifiers: claude-sonnet-4-5, claude-opus-4-6, etc.
            // A date suffix (8 digits) is not a minor version.
            Match current = Regex.Match(id,
                @"^claude-(?:opus|sonnet|haiku|fable|mythos)-(?<major>\d+)(?:[-.](?<minor>\d{1,2})(?=-|$))?(?:[-.][a-z0-9]+)*$");
            if (!current.Success) return AnthropicThinkingMode.None;
            major = int.Parse(current.Groups["major"].Value,
                System.Globalization.CultureInfo.InvariantCulture);
            minor = current.Groups["minor"].Success
                ? int.Parse(current.Groups["minor"].Value,
                    System.Globalization.CultureInfo.InvariantCulture) : 0;
            if (major >= 5 || (major == 4 && minor >= 6))
                return AnthropicThinkingMode.Adaptive;
            if (major == 4 || (major == 3 && minor >= 7))
                return AnthropicThinkingMode.Manual;
            return AnthropicThinkingMode.None;
        }

        /// <summary>
        /// Options shared by all four providers. Streaming stops immediately on excessive
        /// repetition, excess output, cancellation, or a generation deadline.
        /// </summary>
        public sealed class GenerationOptions
        {
            public string SystemInstructions { get; set; } = string.Empty;
            public bool ExtendedThinking { get; set; }
            public int MaxOutputTokens { get; set; } = 16384;
            // Opt-in: keep streamed text if OpenRouter stops at max_tokens.
            // Callers must validate or repair incomplete structured output.
            public bool AllowTruncatedOutput { get; set; }
            public bool WasTruncated { get; internal set; }
            public int MaxOutputCharacters { get; set; } = 300000;
            public TimeSpan MaxDuration { get; set; } = TimeSpan.FromMinutes(5);
            public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(90);
            public IProgress<string> Progress { get; set; }
        }

        /// <summary>Thrown when the generated text is stuck in a repeating loop.</summary>
        public sealed class RepetitionDetectedException : InvalidOperationException
        {
            public RepetitionDetectedException(string reason)
                : base("AI generation stopped: repeated output detected (" + reason + ").") { }
        }

        /// <summary>
        /// Contains structured provider error information without exposing a raw API
        /// JSON response in the exception message. The form maps this to Resources.
        /// </summary>
        public sealed class ApiRequestException : HttpRequestException
        {
            public string Provider { get; }
            public string ApiErrorCode { get; }
            public int? HttpStatusCodeNumber { get; }

            internal ApiRequestException(
                string provider, int? statusCode, string apiErrorCode)
                : base(provider + " API request failed" +
                       (statusCode.HasValue ? " (HTTP " + statusCode.Value + ")" : "") +
                       (string.IsNullOrEmpty(apiErrorCode) ? "." : " [" + apiErrorCode + "]."),
                       null,
                       statusCode.HasValue
                           ? (System.Net.HttpStatusCode?)statusCode.Value
                           : null)
            {
                Provider = provider;
                ApiErrorCode = apiErrorCode ?? string.Empty;
                HttpStatusCodeNumber = statusCode;
            }
        }

        /// <summary>
        /// Extracts the provider's error status for resource mapping. The raw JSON
        /// is intentionally not included in the UI-facing exception message.
        /// </summary>
        private static ApiRequestException CreateApiRequestException(
            string provider, int? httpStatusCode, string responseBody)
        {
            string errorCode = string.Empty;
            int? embeddedStatus = null;
            try
            {
                using JsonDocument json = JsonDocument.Parse(responseBody);
                JsonElement error = json.RootElement;
                if (TryGetObject(error, "response", out JsonElement response) &&
                    TryGetObject(response, "error", out JsonElement responseError))
                    error = responseError;
                else if (TryGetObject(error, "error", out JsonElement nestedError))
                    error = nestedError;

                errorCode = GetString(error, "status");
                if (string.IsNullOrWhiteSpace(errorCode))
                    errorCode = GetString(error, "type");
                if (string.IsNullOrWhiteSpace(errorCode))
                    errorCode = GetString(error, "code");
                if (string.IsNullOrWhiteSpace(errorCode))
                    errorCode = GetString(json.RootElement, "type");

                if (error.ValueKind == JsonValueKind.Object &&
                    error.TryGetProperty("code", out JsonElement numericCode) &&
                    numericCode.ValueKind == JsonValueKind.Number &&
                    numericCode.TryGetInt32(out int code) && code >= 400 && code <= 599)
                    embeddedStatus = code;
                if (json.RootElement.ValueKind == JsonValueKind.Object &&
                    json.RootElement.TryGetProperty("status_code", out JsonElement status) &&
                    status.ValueKind == JsonValueKind.Number &&
                    status.TryGetInt32(out int statusCode) && statusCode >= 400 && statusCode <= 599)
                    embeddedStatus = statusCode;
            }
            catch (JsonException)
            {
                // Non-JSON error pages are common during outages. Keep HTTP status.
            }
            return new ApiRequestException(provider, httpStatusCode ?? embeddedStatus,
                errorCode);
        }

        /// <summary>
        /// Generates text using SSE streaming for all providers. Retains the old method signature.
        /// Repetition is checked while text arrives, not after the API call is finished.
        /// </summary>
        public static Task<string> GenerateTextAsync(
            Model model, string prompt, CancellationToken cancellationToken = default)
            => GenerateTextAsync(model, prompt, new GenerationOptions(), cancellationToken);

        /// <summary>Generate with a separate system prompt, thinking and progress options.</summary>
        public static async Task<string> GenerateTextAsync(
            Model model, string prompt, GenerationOptions options,
            CancellationToken cancellationToken = default)
        {
            ValidateInput(model, prompt);
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.WasTruncated = false;
            if (options.MaxOutputTokens < 256 || options.MaxOutputCharacters < 1000 ||
                options.MaxDuration <= TimeSpan.Zero || options.IdleTimeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(options), "Invalid generation limits.");

            string provider = NormalizeProvider(model.provider, allowAll: false);
            string key = GetDecryptedKey(provider, required: true);
            string url;
            object payload;

            switch (provider)
            {
                case "Google":
                    url = "https://generativelanguage.googleapis.com/v1beta/" +
                          GeminiModelPath(model.name) + ":streamGenerateContent?alt=sse";
                    var geminiPayload = new Dictionary<string, object>
                    {
                        ["contents"] = new[]
                        {
                            new { role = "user", parts = new[] { new { text = prompt } } }
                        }
                    };
                    if (!string.IsNullOrWhiteSpace(options.SystemInstructions))
                        geminiPayload["systemInstruction"] = new
                        {
                            parts = new[] { new { text = options.SystemInstructions } }
                        };
                    // thinkingLevel belongs to Gemini 3+; earlier models must not receive it.
                    if (IsGeminiThinkingLevelModel(model.name))
                        geminiPayload["generationConfig"] = new
                        {
                            maxOutputTokens = options.MaxOutputTokens,
                            thinkingConfig = new
                            {
                                thinkingLevel = options.ExtendedThinking ? "HIGH" : "LOW"
                            }
                        };
                    else
                        geminiPayload["generationConfig"] = new
                        {
                            maxOutputTokens = options.MaxOutputTokens
                        };
                    payload = geminiPayload;
                    break;

                case "OpenAI":
                    url = "https://api.openai.com/v1/responses";
                    var openAiPayload = new Dictionary<string, object>
                    {
                        ["model"] = model.name,
                        ["input"] = prompt,
                        ["stream"] = true,
                        ["store"] = false,
                        ["max_output_tokens"] = options.MaxOutputTokens
                    };
                    if (!string.IsNullOrWhiteSpace(options.SystemInstructions))
                        openAiPayload["instructions"] = options.SystemInstructions;
                    // Effort is supported by reasoning-capable GPT-5/6 and o-series
                    // models; leave it out for other models and when the checkbox is off.
                    if (options.ExtendedThinking && IsOpenAiReasoningModel(model.name))
                        openAiPayload["reasoning"] = new { effort = "high" };
                    payload = openAiPayload;
                    break;

                case "OpenRouter":
                    url = "https://openrouter.ai/api/v1/chat/completions";
                    var openRouterMessages = new List<object>();
                    if (!string.IsNullOrWhiteSpace(options.SystemInstructions))
                        openRouterMessages.Add(new
                        {
                            role = "system",
                            content = options.SystemInstructions
                        });
                    openRouterMessages.Add(new { role = "user", content = prompt });
                    var openRouterPayload = new Dictionary<string, object>
                    {
                        ["model"] = model.name,
                        ["messages"] = openRouterMessages,
                        ["stream"] = true,
                        ["max_tokens"] = options.MaxOutputTokens
                    };
                    if (options.ExtendedThinking && SupportsExtendedThinking(model))
                        openRouterPayload["reasoning"] = new { effort = "high" };
                    payload = openRouterPayload;
                    break;

                case "Anthropic":
                    url = "https://api.anthropic.com/v1/messages";
                    var anthropicPayload = new Dictionary<string, object>
                    {
                        ["model"] = model.name,
                        ["max_tokens"] = options.MaxOutputTokens,
                        ["stream"] = true,
                        ["messages"] = new[]
                        {
                            new { role = "user", content = prompt }
                        }
                    };
                    if (!string.IsNullOrWhiteSpace(options.SystemInstructions))
                        anthropicPayload["system"] = options.SystemInstructions;
                    if (options.ExtendedThinking)
                    {
                        // Claude 4.5 and earlier use legacy budgeted thinking.
                        // Claude 4.6+ uses adaptive thinking; manual thinking is
                        // rejected by Claude 4.7+ (HTTP 400).
                        switch (GetAnthropicThinkingMode(model))
                        {
                            case AnthropicThinkingMode.Manual:
                                int thinkingBudget = Math.Min(4096, options.MaxOutputTokens - 1024);
                                if (thinkingBudget < 1024)
                                    throw new ArgumentOutOfRangeException(nameof(options),
                                        "Anthropic extended thinking requires at least 2048 output tokens.");
                                anthropicPayload["thinking"] = new
                                {
                                    type = "enabled",
                                    budget_tokens = thinkingBudget
                                };
                                break;
                            case AnthropicThinkingMode.Adaptive:
                                anthropicPayload["thinking"] = new { type = "adaptive" };
                                anthropicPayload["output_config"] = new
                                {
                                    effort = "high"
                                };
                                break;
                        }
                    }
                    payload = anthropicPayload;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(model));
            }

            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(options.MaxDuration);
            CancellationToken token = deadline.Token;

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            ApplyAuthentication(request, provider, key);
            request.Headers.Accept.ParseAdd("text/event-stream");
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await Client.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, token)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync(token)
                    .ConfigureAwait(false);
                throw CreateApiRequestException(provider, (int)response.StatusCode, error);
            }

            using var stream = await response.Content.ReadAsStreamAsync(token)
                .ConfigureAwait(false);
            using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
            var result = new StringBuilder();
            var eventData = new StringBuilder();
            string eventName = string.Empty;
            int lastProgressSize = 0;
            int lastRepetitionCheckSize = 0;
            bool reachedDone = false;

            // SSE messages can span multiple data: lines; act only on a complete event.
            void ProcessEvent()
            {
                if (eventData.Length == 0) return;
                string data = eventData.ToString();
                eventData.Clear();
                string currentEvent = eventName;
                eventName = string.Empty;
                if (data == "[DONE]")
                {
                    reachedDone = true;
                    return;
                }

                using JsonDocument json = JsonDocument.Parse(data);
                JsonElement root = json.RootElement;
                string kind = GetString(root, "type");
                if (kind == "error" || currentEvent == "error" ||
                    kind == "response.failed" || kind == "response.incomplete" ||
                    (provider == "OpenRouter" && TryGetObject(root, "error", out _)))
                    throw CreateApiRequestException(provider, null, data);

                if (provider == "OpenRouter" &&
                    TryGetArray(root, "choices", out JsonElement openRouterChoices))
                {
                    foreach (JsonElement choice in openRouterChoices.EnumerateArray())
                    {
                        if (TryGetObject(choice, "error", out JsonElement choiceError))
                            throw CreateApiRequestException(provider, null, choiceError.GetRawText());
                        string finishReason = GetString(choice, "finish_reason");
                        if (finishReason == "length")
                        {
                            if (!options.AllowTruncatedOutput)
                                throw new InvalidOperationException(
                                    "OpenRouter stopped at max_tokens before completing the response.");
                            // Preserve the generated text for callers that can repair it.
                            options.WasTruncated = true;
                        }
                        if (finishReason == "error")
                            throw CreateApiRequestException(provider, null, data);
                    }
                }

                if (provider == "Anthropic" && kind == "message_delta" &&
                    TryGetObject(root, "delta", out JsonElement messageDelta) &&
                    GetString(messageDelta, "stop_reason") == "max_tokens")
                    throw new InvalidOperationException(
                        "Anthropic stopped at max_tokens before completing the response.");

                string delta = ExtractStreamText(provider, root, currentEvent);
                if (delta.Length == 0) return;
                result.Append(delta);
                if (result.Length > options.MaxOutputCharacters)
                    throw new InvalidOperationException(
                        "AI generation stopped: maximum output size was exceeded.");

                if (result.Length >= 500 &&
                    result.Length - lastRepetitionCheckSize >= 128)
                {
                    lastRepetitionCheckSize = result.Length;
                    if (TryDetectRepetition(result.ToString(), out string reason))
                        throw new RepetitionDetectedException(reason);
                }

                // Progress on a UI context can be expensive for long XML; throttle updates.
                if (options.Progress != null &&
                    result.Length - lastProgressSize >= 1024)
                {
                    lastProgressSize = result.Length;
                    options.Progress.Report(result.ToString());
                }
            }

            while (!reachedDone)
            {
                token.ThrowIfCancellationRequested();
                string line;
                using (var idle = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    idle.CancelAfter(options.IdleTimeout);
                    try
                    {
                        // .NET 6: WaitAsync provides cancellable awaiting of ReadLineAsync.
                        // Disposing the response stream below closes an outstanding read.
                        line = await reader.ReadLineAsync().WaitAsync(idle.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (!token.IsCancellationRequested)
                    {
                        throw new TimeoutException(
                            "AI generation stopped: the provider stream was idle too long.");
                    }
                }

                if (line == null)
                {
                    ProcessEvent();
                    break;
                }
                if (line.Length == 0)
                {
                    ProcessEvent();
                    continue;
                }
                if (line.StartsWith("data:", StringComparison.Ordinal))
                {
                    if (eventData.Length > 0) eventData.Append('\n');
                    eventData.Append(line.Substring(5).TrimStart());
                }
                else if (line.StartsWith("event:", StringComparison.Ordinal))
                {
                    eventName = line.Substring(6).Trim();
                }
            }

            string finalText = result.ToString();
            // Streaming may stop between repetition checks; always check the final tail.
            if (TryDetectRepetition(finalText, out string finalReason))
                throw new RepetitionDetectedException(finalReason);
            if (string.IsNullOrWhiteSpace(finalText))
                throw new InvalidOperationException(
                    provider + " returned no text. The response may be blocked or unsupported.");
            options.Progress?.Report(finalText);
            return finalText;
        }

        private static void ApplyAuthentication(
            HttpRequestMessage request, string provider, string key)
        {
            if (provider == "Google")
                request.Headers.Add("x-goog-api-key", key);
            else
            {
                // Claude API version 2026-10-21 accepts bearer API keys; follow the
                // current API convention while keeping Google's separate key header.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
                if (provider == "Anthropic")
                    request.Headers.Add("anthropic-version", AnthropicApiVersion);
            }
        }

        private static string ExtractStreamText(
            string provider, JsonElement root, string eventName)
        {
            if (provider == "OpenAI")
                return GetString(root, "type") == "response.output_text.delta"
                    ? GetString(root, "delta") : string.Empty;

            if (provider == "OpenRouter")
            {
                // Chat Completions SSE; skip reasoning and tool-call deltas.
                if (!TryGetArray(root, "choices", out JsonElement choices))
                    return string.Empty;
                foreach (JsonElement choice in choices.EnumerateArray())
                {
                    if (TryGetObject(choice, "delta", out JsonElement delta))
                    {
                        string content = GetString(delta, "content");
                        if (content.Length > 0)
                            return content;
                    }
                }
                return string.Empty;
            }

            if (provider == "Anthropic")
            {
                if (GetString(root, "type") != "content_block_delta" ||
                    !TryGetObject(root, "delta", out JsonElement delta) ||
                    GetString(delta, "type") != "text_delta")
                    return string.Empty;
                return GetString(delta, "text");
            }

            // Gemini streamGenerateContent returns one JSON candidate per SSE event.
            if (!TryGetArray(root, "candidates", out JsonElement candidates))
                return string.Empty;
            var parts = new StringBuilder();
            foreach (JsonElement candidate in candidates.EnumerateArray())
            {
                if (!TryGetObject(candidate, "content", out JsonElement content) ||
                    !TryGetArray(content, "parts", out JsonElement textParts))
                    continue;
                foreach (JsonElement part in textParts.EnumerateArray())
                {
                    // Do not include model thoughts in the NBPML output.
                    if (part.ValueKind == JsonValueKind.Object &&
                        part.TryGetProperty("thought", out JsonElement thought) &&
                        thought.ValueKind == JsonValueKind.True)
                        continue;
                    parts.Append(GetString(part, "text"));
                }
            }
            return parts.ToString();
        }

        private static bool IsGeminiThinkingLevelModel(string modelName)
        {
            var match = Regex.Match(modelName ?? string.Empty,
                @"gemini-(\d+(?:\.\d+)?)", RegexOptions.IgnoreCase);
            return match.Success &&
                double.TryParse(match.Groups[1].Value,
                    System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture, out double version) &&
                version >= 3.0;
        }

        /// <summary>
        /// Repetition guard migrated from CreateMusicWithAI. Inspects the final
        /// 8,000 characters and preserves common musical repetitions in NBPML XML.
        /// </summary>
        public static bool CheckIfModelIsStuckOrKeepsRepeating(string response)
            => TryDetectRepetition(response, out _);

        private static bool TryDetectRepetition(string response, out string reason)
        {
            reason = string.Empty;
            if (string.IsNullOrEmpty(response) || response.Length < 500) return false;

            // Music is naturally repetitive: do not flag a short run of repeated
            // XML <Line> elements as an AI loop. Only flag a long run of 24 identical
            // complete musical events, while other heuristics cover ordinary text.
            string tail = response.Length > 8000
                ? response.Substring(response.Length - 8000) : response;
            bool containsMusicalLines = response.Contains("<Line", StringComparison.OrdinalIgnoreCase);
            if (containsMusicalLines)
            {
                MatchCollection lines = Regex.Matches(tail,
                    @"<Line\b[^>]*>[\s\S]*?</Line>", RegexOptions.IgnoreCase);
                if (lines.Count >= 24)
                {
                    string last = Regex.Replace(lines[lines.Count - 1].Value, @"\s+", "");
                    bool same = true;
                    for (int index = lines.Count - 24; index < lines.Count; index++)
                    {
                        string candidate = Regex.Replace(lines[index].Value, @"\s+", "");
                        if (!string.Equals(last, candidate, StringComparison.Ordinal))
                        {
                            same = false;
                            break;
                        }
                    }
                    if (same)
                    {
                        reason = "24 identical consecutive NBPML lines";
                        return true;
                    }
                }
            }

            // 1. Same 150-character chunk three times consecutively (non-NBPML).
            const int chunkSize = 150;
            if (!containsMusicalLines && response.Length >= 3 * chunkSize)
            {
                string chunk = response.Substring(response.Length - chunkSize);
                if (string.Equals(chunk,
                        response.Substring(response.Length - 2 * chunkSize, chunkSize),
                        StringComparison.Ordinal) &&
                    string.Equals(chunk,
                        response.Substring(response.Length - 3 * chunkSize, chunkSize),
                        StringComparison.Ordinal))
                {
                    reason = "identical 150-character segments";
                    return true;
                }
            }

            tail = Regex.Replace(tail, @"(?m)^\s*[-_=]{10,}\s*$", " <NBPML_SEPARATOR> ");

            // 2. More than eighty identical characters.
            if (Regex.IsMatch(tail, @"(.)\1{80,}", RegexOptions.Singleline))
            {
                reason = "repeated character run";
                return true;
            }

            // 3. 100 identical consecutive tokens (case-insensitive words).
            MatchCollection matches = Regex.Matches(
                tail, @"</?[^>]+>|[\p{L}\p{N}]+|.", RegexOptions.Singleline);
            string previous = null;
            int run = 0;
            foreach (Match match in matches)
            {
                string token = match.Value;
                if (string.IsNullOrWhiteSpace(token)) continue;
                if (Regex.IsMatch(token, @"^[\p{L}\p{N}]+$"))
                    token = token.ToLowerInvariant();
                run = string.Equals(previous, token, StringComparison.Ordinal) ? run + 1 : 1;
                previous = token;
                if (run >= 100)
                {
                    reason = "repeated token run";
                    return true;
                }
            }

            // 4. Short text motifs repeated 6 times. Exclude XML structures:
            // repetitive Line/Note tags are ordinary musical notation, not necessarily a loop.
            string compact = Regex.Replace(tail, @"\s+", "");
            for (int length = 4; length <= 80; length++)
            {
                if (compact.Length < length * 6) break;
                string motif = compact.Substring(compact.Length - length, length);
                if (motif.Contains('<') || motif.Contains('>') ||
                    motif.Distinct().Count() == 1)
                    continue;
                int repeats = 1;
                int position = compact.Length - length;
                while (position >= length &&
                    string.CompareOrdinal(compact, position - length,
                        compact, position, length) == 0)
                {
                    repeats++;
                    position -= length;
                    if (repeats >= 6)
                    {
                        reason = "repeated phrase or motif";
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Count input tokens for a prompt. OpenRouter has no separate preflight
        /// counting endpoint; counting there makes a small, billable completion
        /// request and reads usage.prompt_tokens. Other providers use native counting APIs.
        /// Note: availability is model-specific, and /models does not prove support.
        /// </summary>
        public static async Task<int> CountTokensAsync(
            Model model, string prompt, CancellationToken cancellationToken = default)
        {
            ValidateInput(model, prompt);
            string provider = NormalizeProvider(model.provider, allowAll: false);
            string key = GetDecryptedKey(provider, required: true);

            string url;
            string property;
            object payload;

            switch (provider)
            {
                case "Google":
                    url = "https://generativelanguage.googleapis.com/v1beta/" +
                          GeminiModelPath(model.name) + ":countTokens";
                    payload = new
                    {
                        contents = new[]
                        {
                            new { role = "user", parts = new[] { new { text = prompt } } }
                        }
                    };
                    property = "totalTokens";
                    break;

                case "OpenAI":
                    url = "https://api.openai.com/v1/responses/input_tokens";
                    payload = new { model = model.name, input = prompt };
                    property = "input_tokens";
                    break;

                case "OpenRouter":
                    url = "https://openrouter.ai/api/v1/chat/completions";
                    // At least 16 output tokens for compatibility with providers that
                    // enforce a minimum completion length. This call may incur charges.
                    payload = new
                    {
                        model = model.name,
                        messages = new[] { new { role = "user", content = prompt } },
                        max_tokens = 16,
                        stream = false
                    };
                    property = "prompt_tokens";
                    break;

                case "Anthropic":
                    url = "https://api.anthropic.com/v1/messages/count_tokens";
                    payload = new
                    {
                        model = model.name,
                        messages = new[] { new { role = "user", content = prompt } }
                    };
                    property = "input_tokens";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(model));
            }

            using JsonDocument response = await SendJsonAsync(
                HttpMethod.Post, url, provider, key, payload, cancellationToken)
                .ConfigureAwait(false);

            JsonElement countSource = response.RootElement;
            if (provider == "OpenRouter" &&
                !TryGetObject(response.RootElement, "usage", out countSource))
                throw new InvalidOperationException(
                    "OpenRouter did not return usage data for token counting.");

            if (countSource.TryGetProperty(property, out JsonElement count) &&
                count.ValueKind == JsonValueKind.Number && count.TryGetInt32(out int number))
                return number;

            throw new InvalidOperationException(
                $"{provider} did not return a valid '{property}' field.");
        }

        private static async Task<List<Model>> ListGeminiAsync(
            string key, CancellationToken cancellationToken)
        {
            var result = new List<Model>();
            var seenTokens = new HashSet<string>(StringComparer.Ordinal);
            string pageToken = null;

            do
            {
                string url = "https://generativelanguage.googleapis.com/v1beta/models?pageSize=1000";
                if (!string.IsNullOrEmpty(pageToken))
                    url += "&pageToken=" + Uri.EscapeDataString(pageToken);

                using JsonDocument json = await SendJsonAsync(
                    HttpMethod.Get, url, "Google", key, null, cancellationToken)
                    .ConfigureAwait(false);

                JsonElement root = json.RootElement;
                if (TryGetArray(root, "models", out JsonElement models))
                {
                    foreach (JsonElement item in models.EnumerateArray())
                    {
                        string name = GetString(item, "name");
                        string displayName = GetString(item, "displayName");
                        if (!IsGeminiTextModel(name, displayName) ||
                            !TryGetArray(item, "supportedGenerationMethods", out JsonElement methods))
                            continue;

                        var supported = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (JsonElement method in methods.EnumerateArray())
                            if (method.ValueKind == JsonValueKind.String)
                                supported.Add(method.GetString());

                        if (WantedFeatures.All(supported.Contains) &&
                            UnwantedFeatures.All(feature => !supported.Contains(feature)))
                            result.Add(new Model(name,
                                string.IsNullOrWhiteSpace(displayName) ? name : displayName,
                                "Google"));
                    }
                }

                string nextToken = GetString(root, "nextPageToken");
                if (string.IsNullOrEmpty(nextToken))
                    break;
                if (!seenTokens.Add(nextToken))
                    throw new InvalidOperationException("Google returned a repeated page token.");
                pageToken = nextToken;
            } while (true);

            return result;
        }

        private static async Task<List<Model>> ListOpenAiAsync(
            string key, CancellationToken cancellationToken)
        {
            using JsonDocument json = await SendJsonAsync(
                HttpMethod.Get, "https://api.openai.com/v1/models",
                "OpenAI", key, null, cancellationToken).ConfigureAwait(false);

            var result = new List<Model>();
            if (TryGetArray(json.RootElement, "data", out JsonElement models))
            {
                foreach (JsonElement item in models.EnumerateArray())
                {
                    string name = GetString(item, "id");
                    if (!IsOpenAiTextModel(name))
                        continue;

                    // The API may return models with an announced shutdown date.
                    string shutdown = GetString(item, "shutdown_date");
                    if (DateTime.TryParse(shutdown, out DateTime shutdownDate) &&
                        shutdownDate.Date <= DateTime.UtcNow.Date)
                        continue;

                    // OpenAI's models/list response has no display_name field.
                    result.Add(new Model(name, FormatOpenAiDisplayName(name), "OpenAI"));
                }
            }
            return result;
        }

        private static async Task<List<Model>> ListAnthropicAsync(
            string key, CancellationToken cancellationToken)
        {
            var result = new List<Model>();
            var seenCursors = new HashSet<string>(StringComparer.Ordinal);
            string afterId = null;

            do
            {
                string url = "https://api.anthropic.com/v1/models?limit=100";
                if (!string.IsNullOrEmpty(afterId))
                    url += "&after_id=" + Uri.EscapeDataString(afterId);

                using JsonDocument json = await SendJsonAsync(
                    HttpMethod.Get, url, "Anthropic", key, null, cancellationToken)
                    .ConfigureAwait(false);

                JsonElement root = json.RootElement;
                if (TryGetArray(root, "data", out JsonElement models))
                {
                    foreach (JsonElement item in models.EnumerateArray())
                    {
                        string name = GetString(item, "id");
                        string lifecycle = GetString(item, "lifecycle");

                        if (!name.StartsWith("claude-", StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrEmpty(lifecycle) &&
                             !lifecycle.Equals("active", StringComparison.OrdinalIgnoreCase)))
                            continue;

                        string display = GetString(item, "display_name");
                        result.Add(new Model(name,
                            string.IsNullOrWhiteSpace(display) ? name : display,
                            "Anthropic")
                        {
                            ThinkingMode = GetAnthropicThinkingModeFromApi(item)
                        });
                    }
                }

                bool hasMore = root.TryGetProperty("has_more", out JsonElement more) &&
                               more.ValueKind == JsonValueKind.True;
                if (!hasMore)
                    break;

                string next = GetString(root, "last_id");
                if (string.IsNullOrEmpty(next) || !seenCursors.Add(next))
                    throw new InvalidOperationException("Invalid Anthropic pagination cursor.");
                afterId = next;
            } while (true);

            return result;
        }

        private static bool IsOpenRouterTextModel(JsonElement model)
        {
            // This engine sends text prompts to /chat/completions and consumes
            // text deltas. Other endpoints alone are not sufficient.
            if (TryGetArray(model, "endpoints", out JsonElement endpoints) &&
                endpoints.GetArrayLength() > 0 &&
                !endpoints.EnumerateArray().Any(endpoint =>
                    endpoint.ValueKind == JsonValueKind.String &&
                    string.Equals(endpoint.GetString(), "/api/v1/chat/completions",
                        StringComparison.OrdinalIgnoreCase)))
                return false;

            if (!TryGetObject(model, "architecture", out JsonElement architecture))
                return false; // Cannot establish the model's output format.

            string modality = GetString(architecture, "modality");

            // Multimodal *input* is fine: GPT/Claude/Gemini can accept images
            // and still produce normal text from an ordinary text prompt.
            if (TryGetArray(architecture, "input_modalities", out JsonElement inputs))
            {
                if (!inputs.EnumerateArray().Any(input =>
                    input.ValueKind == JsonValueKind.String &&
                    string.Equals(input.GetString(), "text",
                        StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
            else
            {
                // Legacy catalog entries may provide only "text+image->text".
                int arrow = modality.IndexOf("->", StringComparison.Ordinal);
                if (arrow < 0 || !modality.Substring(0, arrow)
                        .Split('+').Any(input =>
                            string.Equals(input.Trim(), "text",
                                StringComparison.OrdinalIgnoreCase)))
                    return false;
            }

            // output_modalities=text at the API is an INCLUSIVE filter: models
            // with ["text", "video"] (e.g. Gemini Omni) may still be returned.
            // Require ONLY text output, not merely text among several outputs.
            if (TryGetArray(architecture, "output_modalities", out JsonElement outputs))
            {
                if (outputs.GetArrayLength() != 1 ||
                    outputs[0].ValueKind != JsonValueKind.String ||
                    !string.Equals(outputs[0].GetString(), "text",
                        StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            else if (!modality.EndsWith("->text", StringComparison.OrdinalIgnoreCase))
            {
                return false; // Unknown output types are not safe for text-only UI.
            }

            // Gemini Omni is designed for media generation. Do not show this
            // family even if an individual catalog entry omits video metadata.
            string id = GetString(model, "id");
            if (Regex.IsMatch(id, @"(?:^|/)gemini[-_]omni(?:[-_]|$)",
                    RegexOptions.IgnoreCase))
                return false;

            return true;
        }

        private static bool IsGeminiTextModel(string name, string displayName)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                !name.StartsWith("models/gemini-", StringComparison.OrdinalIgnoreCase))
                return false;

            string combined = name + " " + displayName;
            return !GeminiExcludedTerms.Any(term =>
                       combined.Contains(term, StringComparison.OrdinalIgnoreCase)) &&
                   !name.EndsWith("-001", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOpenAiTextModel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            bool family = name.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase) ||
                          name.StartsWith("chatgpt-", StringComparison.OrdinalIgnoreCase) ||
                          Regex.IsMatch(name, @"^o\d+(?:-|$)", RegexOptions.IgnoreCase);

            return family && !OpenAiExcludedTerms.Any(term =>
                name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        private static string FormatOpenAiDisplayName(string id)
        {
            if (id.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase))
                return "GPT-" + id.Substring(4);
            if (id.StartsWith("chatgpt-", StringComparison.OrdinalIgnoreCase))
                return "ChatGPT-" + id.Substring(8);
            return id;
        }

        private static string NormalizeProvider(string provider, bool allowAll)
        {
            switch ((provider ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "":
                case "all":
                case "*":
                    if (allowAll) return "All";
                    break;
                case "google":
                case "gemini": return "Google";
                case "openai": return "OpenAI";
                case "anthropic":
                case "claude": return "Anthropic";
                case "openrouter":
                case "open router":
                case "open-router":
                case "open_router":
                case "openrouter.ai": return "OpenRouter";
            }
            throw new ArgumentException(
                "Expected provider: OpenAI, Google, Anthropic, OpenRouter, or All.", nameof(provider));
        }

        private static string GetDecryptedKey(string provider, bool required)
        {
            string encrypted;
            switch (provider)
            {
                case "Google":
                    encrypted = Settings1.Default.EncryptedGeminiAPIKeyBase64;
                    break;
                case "OpenAI":
                    encrypted = Settings1.Default.EncryptedOpenAIAPIKeyBase64;
                    break;
                case "Anthropic":
                    encrypted = Settings1.Default.EncryptedAnthropicAPIKeyBase64;
                    break;
                case "OpenRouter":
                    encrypted = Settings1.Default.EncryptedOpenRouterAPIKeyBase64;
                    break;
                default:
                    throw new ArgumentException("Unknown provider.", nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(encrypted))
            {
                if (required)
                    throw new InvalidOperationException($"No {provider} API key is configured.");
                return null;
            }

            string key = new EncryptionHelper().DecryptBase64EncryptedData(encrypted);
            if (provider == "OpenRouter")
                key = key?.Trim();
            if (string.IsNullOrWhiteSpace(key) && required)
                throw new InvalidOperationException($"The {provider} API key is empty.");
            return key;
        }

        private static async Task<JsonDocument> SendJsonAsync(
            HttpMethod method, string url, string provider, string key,
            object payload, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, url);
            ApplyAuthentication(request, provider, key);

            if (payload != null)
                request.Content = new StringContent(
                    JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await Client.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            string body = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateApiRequestException(provider, (int)response.StatusCode, body);
            }

            return JsonDocument.Parse(body);
        }

        private static string GeminiModelPath(string name)
        {
            string path = name.StartsWith("models/", StringComparison.OrdinalIgnoreCase)
                ? name
                : "models/" + name;

            // Model identifiers become URL paths; don't allow user-controlled path/query syntax.
            if (!Regex.IsMatch(path, @"^models/gemini-[A-Za-z0-9._-]+$",
                RegexOptions.IgnoreCase))
                throw new ArgumentException("Invalid Google model name.", nameof(name));
            return path;
        }

        private static void ValidateInput(Model model, string prompt)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrWhiteSpace(model.name))
                throw new ArgumentException("Model name is missing.", nameof(model));
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
        }

        private static void CollectTexts(JsonElement blocks, List<string> output)
        {
            foreach (JsonElement item in blocks.EnumerateArray())
            {
                string text = GetString(item, "text");
                if (!string.IsNullOrEmpty(text))
                    output.Add(text);
            }
        }

        private static string GetString(JsonElement obj, string key)
        {
            return obj.ValueKind == JsonValueKind.Object &&
                   obj.TryGetProperty(key, out JsonElement value) &&
                   value.ValueKind == JsonValueKind.String
                ? value.GetString() ?? string.Empty
                : string.Empty;
        }

        private static bool TryGetArray(JsonElement obj, string key, out JsonElement value)
        {
            if (obj.ValueKind == JsonValueKind.Object &&
                obj.TryGetProperty(key, out value) &&
                value.ValueKind == JsonValueKind.Array)
                return true;
            value = default;
            return false;
        }

        private static bool TryGetObject(JsonElement obj, string key, out JsonElement value)
        {
            if (obj.ValueKind == JsonValueKind.Object &&
                obj.TryGetProperty(key, out value) &&
                value.ValueKind == JsonValueKind.Object)
                return true;
            value = default;
            return false;
        }
    }
}
