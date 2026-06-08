using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Chat;
using static NeoBleeper.OllamaUtility;

namespace NeoBleeper
{
    public class OllamaUtility
    {
        public class Message
        {
            public string Role { get; set; } = string.Empty; // "system" or "user"
            public string Content { get; set; } = string.Empty;
        }

        public class OllamaRequest
        {
            public string Model { get; set; } = string.Empty;
            public List<Message> Messages { get; set; } = new();
            public bool Stream { get; set; } = false;
        }

        public class OllamaResponse
        {
            public Message Message { get; set; } = new();
            public bool Done { get; set; }
        }

        public class OllamaTagsResponse
        {
            public List<OllamaModelItem> Models { get; set; } = new();
        }

        public class OllamaModelItem
        {
            public string Name { get; set; } = string.Empty; // Örn: "llama3.2:latest"
            public ModelDetails Details { get; set; } = new();
        }

        public class ModelDetails
        {
            public string Family { get; set; } = string.Empty;       // Örn: "llama"
            public string ParameterSize { get; set; } = string.Empty; // Örn: "3.2B"
        }

        public static async Task<bool> IsValidOllamaUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            // Ensure the string can be parsed as a valid HTTP/HTTPS URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUri)) return false;

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(4); // Keep timeout low for responsiveness

            try
            {
                // 1. Direct hit to the root URL
                var response = await client.GetAsync(validatedUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Ollama returns "Ollama is running" on the root path
                    return content.Contains("Ollama is running", StringComparison.OrdinalIgnoreCase);
                }

                // 2. Fallback check: try the specific version API endpoint
                var versionUri = new Uri(validatedUri, "/api/version");
                var versionResponse = await client.GetAsync(versionUri);

                return versionResponse.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                // Server is offline, port closed, or host unreachable
                return false;
            }
            catch (TaskCanceledException)
            {
                // Connection timed out
                return false;
            }
            catch
            {
                // Any other unexpected network errors
                return false;
            }
        }
        public static async Task<string> SendResponseAsync(
        string systemInstruction,
        string userPrompt,
        string modelName)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Settings1.Default.OllamaClientURL);

            var requestPayload = new OllamaRequest
            {
                Model = modelName,
                Messages = new List<Message>
            {
                // 1. Set the system instructions
                new Message { Role = "system", Content = systemInstruction },
                // 2. Set the user question
                new Message { Role = "user", Content = userPrompt }
            }
            };

            // Note the endpoint change to /api/chat
            var response = await client.PostAsJsonAsync("/api/chat", requestPayload);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Message?.Content ?? string.Empty;
        }
        private static bool TryStartOllama()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "serve",
                    CreateNoWindow = true,     // Runs hidden in the background
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to start Ollama process: " + ex.Message, Logger.LogTypes.Error);
                return false;
            }
        }

        private static bool IsOllamaProcessRunning()
        {
            try
            {
                var processes = Process.GetProcessesByName("ollama");
                return processes.Length > 0;
            }
            catch (Exception ex)
            {
                Logger.Log("Error checking for Ollama process: " + ex.Message, Logger.LogTypes.Error);
                return false;
            }
        }
        public static bool EnsureOllamaIsRunning()
        {
            if(IsOllamaProcessRunning())
            {
                return true; // Already running
            }
            else
            {
                return TryStartOllama(); // Attempt to start it to avoid user having to do it manually and checking is it installed or not
            }
        }
        public static async Task<Dictionary<string, string>> GetModelNamesAndDisplayNamesAsync(string currentUrl)
        {
            var modelMap = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(currentUrl) || !Uri.TryCreate(currentUrl, UriKind.Absolute, out var baseUri))
            {
                Logger.Log("Invalid Ollama URL provided for fetching models.", Logger.LogTypes.Error);
                return modelMap;
            }

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                // Check if Ollama is running by hitting the root URL first
                var verifyResponse = await client.GetAsync(baseUri);
                var verifyContent = await verifyResponse.Content.ReadAsStringAsync();

                if (!verifyResponse.IsSuccessStatusCode || !verifyContent.Contains("Ollama is running", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Log("Ollama is not running at the provided URL. Cannot fetch models.", Logger.LogTypes.Error);
                    return modelMap;
                }

                // Get the list of models from the /api/tags endpoint
                var tagsUri = new Uri(baseUri, "/api/tags");
                var tagsResponse = await client.GetAsync(tagsUri);
                tagsResponse.EnsureSuccessStatusCode();

                var jsonString = await tagsResponse.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaTagsResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Create a mapping of model names to display names based on the response
                if (result?.Models != null)
                {
                    // Local helper to format tokens safely
                    string FormatToken(string token)
                    {
                        if (string.IsNullOrWhiteSpace(token))
                            return string.Empty;

                        token = token.Replace('-', ' ').Replace('_', ' ');
                        
                        token = Regex.Replace(token, @"(?<=[A-Za-uw-z])(?=\d)|(?<=\d)(?=\b[A-Za-zz])|(?<=[a-z])(?=[A-Z])", " ");
                        token = token.Trim();

                        var words = token.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < words.Length; i++)
                        {
                            
                            if (Regex.IsMatch(words[i], @"^[Vv]\d"))
                            {
                                continue;
                            }
                            if (Regex.IsMatch(words[i], @"^\d+(?:\.\d+)?[KkBbGgMm]$"))
                            {
                                words[i] = words[i].ToUpperInvariant();
                                continue;
                            }

                            if (words[i] == words[i].ToLowerInvariant())
                            {
                                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                            }
                        }

                        return string.Join(" ", words);
                    }

                    foreach (var model in result.Models)
                    {
                        string keyModelName = model.Name ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(keyModelName)) continue;

                        string displayName = string.Empty;

                        var parts = keyModelName.Split(':');
                        string baseName = parts[0];
                        string tag = parts.Length > 1 ? parts[1] : string.Empty;

                        // Model ismini formatla
                        string formattedBase = FormatToken(baseName);

                        if (!string.IsNullOrWhiteSpace(tag) && !tag.Equals("latest", StringComparison.OrdinalIgnoreCase))
                        {
                            string formattedTag = Regex.IsMatch(tag, @"^\d+(?:\.\d+)?[KkBbGgMm]$")
                                ? tag.ToUpperInvariant()
                                : tag;

                            displayName = $"{formattedBase} ({formattedTag})";
                        }
                        else
                        {
                            displayName = formattedBase;
                        }

                        if (string.IsNullOrWhiteSpace(displayName))
                        {
                            displayName = keyModelName;
                        }

                        if (!modelMap.ContainsKey(keyModelName))
                        {
                            modelMap.Add(keyModelName, displayName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error fetching models from Ollama: " + ex.Message, Logger.LogTypes.Error);
            }

            return modelMap;
        }
    }
}
