using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace NeoBleeper
{
    /* Class for handling private information in the NeoBleeper application's logger
    and "Create Music with AI" components.
    This class is designed to return status of private information or redacting
    for redacting in logs, aborting AI music generation when private information is
    detected, and other related tasks.
    */
    public class SensitiveInformationHandler
    {
        /// <summary>
        /// Checks if the given text contains any private information based on predefined patterns.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if private information is detected; otherwise, false.</returns>
        public static bool ContainsPrivateInformation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !MayContainSensitiveInformation(text))
            {
                return false;
            }

            foreach (var (name, regex, replacement) in _maskRules.Value)
            {
                try
                {
                    if (regex.IsMatch(text))
                    {
                        return true;
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    // Skip only the pattern that timed out.
                }
            }

            return false;
        }

        /// <summary>
        /// Redacts private information from the given text based on predefined patterns.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>String with private information redacted.</returns>
        public static string RedactPrivateInformation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !MayContainSensitiveInformation(text))
            {
                return text;
            }

            string result = text;

            foreach (var (name, regex, replacement) in _maskRules.Value)
            {
                try
                {
                    result = regex.Replace(result, replacement);
                }
                catch (RegexMatchTimeoutException)
                {
                    // Skip only the pattern that timed out.
                }
            }

            return result;
        }

        /// <summary>
        /// Preliminary check to determine if the text may contain sensitive information.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if the text may contain sensitive information; otherwise, false.</returns>
        private static bool MayContainSensitiveInformation(string text)
        {
            int digitCount = 0;
            int tokenRun = 0;
            bool hasDot = false;
            bool hasHyphen = false;
            bool hasColon = false;
            bool hasComma = false;
            bool hasNewLine = false;

            foreach (char character in text)
            {
                if (char.IsDigit(character))
                {
                    digitCount++;
                }

                if (char.IsLetterOrDigit(character) ||
                    character is '_' or '-' or '+' or '/' or '=')
                {
                    tokenRun++;

                    if (tokenRun >= 40)
                    {
                        return true;
                    }
                }
                else
                {
                    tokenRun = 0;
                }

                switch (character)
                {
                    case '@':
                    case '\\':
                        return true;

                    case '.':
                        hasDot = true;
                        break;

                    case '-':
                        hasHyphen = true;
                        break;

                    case ':':
                        hasColon = true;
                        break;

                    case ',':
                        hasComma = true;
                        break;

                    case '\r':
                    case '\n':
                        hasNewLine = true;
                        break;
                }
            }

            if (digitCount >= 13 ||
                (digitCount >= 4 && (hasDot || hasColon || hasHyphen)))
            {
                return true;
            }

            
            if (digitCount >= 5 && (hasComma || hasNewLine))
            {
                return true;
            }

            return text.IndexOf("-----BEGIN", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("passwd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("pwd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("api_key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("api-key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("client_secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("client-secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("Bearer ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("AIzaSy", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("AKIA", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("ASIA", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("sk-", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("eyJ", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("/home/", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("/Users/", StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// Builds the compiled masking rules once, on the background writer thread.
        /// </summary>
        private static (string Name, Regex Regex, string Replacement)[] CreateMaskRules()
        {

            const string houseNumber =
                @"\d{1,6}[A-Za-z]?(?:[-/]\d{1,6}[A-Za-z]?)?";

            const string postcode =
                @"(?:" +
                    // Generic numeric postcode.
                    @"\d{4,6}(?:-\d{3,4})?" +

                    // Poland-like.
                    @"|\d{2}-\d{3}" +

                    // Canada-like.
                    @"|[A-Z]\d[A-Z][ \t]?\d[A-Z]\d" +

                    // United Kingdom-like.
                    @"|[A-Z]{1,2}\d[A-Z\d]?[ \t]?\d[A-Z]{2}" +

                    // Netherlands-like.
                    @"|\d{4}[ \t]?[A-Z]{2}" +
                @")";

            const string roadDesignator =
                @"(?:" +
                    // English
                    @"Street|Road|Avenue|Boulevard|Lane|Drive|Court|" +
                    @"Terrace|Parkway|Place|Way|" +

                    // Turkish
                    @"Sokak|Sokağı|Cadde|Caddesi|Bulvar|Bulvarı|" +

                    // German
                    @"Straße|Strasse|Weg|Allee|Platz|" +

                    // French
                    @"Rue|Chemin|Boulevard|Avenue|Impasse|" +

                    // Spanish
                    @"Calle|Avenida|Paseo|Camino|Carretera|" +

                    // Portuguese
                    @"Rua|Avenida|Travessa|Estrada|" +

                    // Italian
                    @"Via|Viale|Piazza|Corso|" +

                    // Dutch
                    @"Straat|Laan|Plein|Weg|" +

                    // Scandinavian
                    @"Gate|Gata|Gade|Vej|Vägen|Vagen|" +

                    // Central / Eastern European
                    @"Ulica|Ulitsa|Prospekt" +
                @")";

            const string words =
                @"[\p{L}][\p{L}'’.\-]*(?:[ \t]+[\p{L}][\p{L}'’.\-]*){0,5}";

            var patterns =
                new (string Name, string Pattern, string Replacement, RegexOptions Options)[]
                {
                    // Private key headers
                    (
                        "PrivateKey",
                        @"-----BEGIN (?:RSA |EC |DSA |OPENSSH )?PRIVATE KEY-----",
                        "[REDACTED_PRIVATE_KEY]",
                        RegexOptions.IgnoreCase
                    ),

                    // Passwords, secrets, and tokens assigned in configuration/code
                    (
                        "AssignedSecret",
                        @"\b(?:password|passwd|pwd|secret|token|api[_-]?key|" +
                        @"client[_-]?secret)\s*[:=]\s*[""']?" +
                        @"[A-Za-z0-9._~+/=-]{8,}[""']?",
                        "[REDACTED_SECRET]",
                        RegexOptions.IgnoreCase
                    ),

                    // Google API keys
                    (
                        "GoogleApiKey",
                        @"\bAIzaSy[A-Za-z0-9_-]{33}\b",
                        "[REDACTED_API_KEY]",
                        RegexOptions.None
                    ),

                    // OpenAI-style keys
                    (
                        "OpenAiApiKey",
                        @"\bsk-(?:proj-|svcacct-)?[A-Za-z0-9_-]{20,}\b",
                        "[REDACTED_API_KEY]",
                        RegexOptions.IgnoreCase
                    ),

                    // AWS access key IDs
                    (
                        "AwsAccessKey",
                        @"\b(?:AKIA|ASIA)[A-Z0-9]{16}\b",
                        "[REDACTED_AWS_KEY]",
                        RegexOptions.None
                    ),

                    // Bearer tokens
                    (
                        "BearerToken",
                        @"\bBearer\s+[A-Za-z0-9._~+/=-]{20,}",
                        "Bearer [REDACTED_TOKEN]",
                        RegexOptions.IgnoreCase
                    ),

                    // JSON Web Tokens
                    (
                        "JsonWebToken",
                        @"(?<![A-Za-z0-9_-])" +
                        @"eyJ[A-Za-z0-9_-]{5,}\." +
                        @"[A-Za-z0-9_-]{5,}\." +
                        @"[A-Za-z0-9_-]{5,}" +
                        @"(?![A-Za-z0-9_-])",
                        "[REDACTED_JWT]",
                        RegexOptions.None
                    ),

                    // Email addresses
                    (
                        "Email",
                        @"(?<![A-Za-z0-9._%+-])" +
                        @"[A-Za-z0-9._%+-]+@" +
                        @"[A-Za-z0-9.-]+\.[A-Za-z]{2,}" +
                        @"(?![A-Za-z0-9._%+-])",
                        "[REDACTED_EMAIL]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 1
                     *
                     * [house_number] [road]
                     * [postcode] [settlement]
                     *
                     * Example:
                     *
                     *     123 Main Street, 10115 Berlin
                     *
                     * Requires all of:
                     *
                     *     house number
                     *     explicit road designator
                     *     postcode
                     *     settlement
                     */
                    (
                        "AddressHouseRoadPostcodeCity",
                        @"(?<![\p{L}\p{N}])" +
                        houseNumber +
                        @"[ \t]+" +
                        words +
                        @"[ \t]+" +
                        roadDesignator +
                        @"[ \t]*,\s*" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?![\p{L}\p{N}])",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 2
                     *
                     * [road] [house_number]
                     * [postcode] [settlement]
                     *
                     * Example:
                     *
                     *     Hauptstraße 12, 10115 Berlin
                     */
                    (
                        "AddressRoadHousePostcodeCity",
                        @"(?<![\p{L}\p{N}])" +
                        words +
                        @"[ \t]+" +
                        roadDesignator +
                        @"[ \t]+" +
                        houseNumber +
                        @"[ \t]*,\s*" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?![\p{L}\p{N}])",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 3
                     *
                     * [road], [house_number]
                     * [postcode] [settlement]
                     *
                     * Examples:
                     *
                     *     Via Roma, 25, 00100 Roma
                     *     Calle Mayor, 12, 28013 Madrid
                     */
                    (
                        "AddressRoadCommaHousePostcodeCity",
                        @"(?<![\p{L}\p{N}])" +
                        roadDesignator +
                        @"[ \t]+" +
                        words +
                        @"[ \t]*,\s*" +
                        houseNumber +
                        @"[ \t]*,\s*" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?![\p{L}\p{N}])",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 4
                     *
                     * North American-style:
                     *
                     * [house_number] [road],
                     * [settlement], [region] [postcode]
                     *
                     * Example:
                     *
                     *     123 Main Street, Seattle, WA 98101
                     */
                    (
                        "AddressNorthAmerican",
                        @"(?<![\p{L}\p{N}])" +
                        houseNumber +
                        @"[ \t]+" +
                        words +
                        @"[ \t]+" +
                        roadDesignator +
                        @"[ \t]*,\s*" +
                        words +
                        @"[ \t]*,\s*" +
                        @"[A-Z]{2}[ \t]+" +
                        @"\d{5}(?:-\d{4})?" +
                        @"(?![\p{L}\p{N}])",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 5
                     *
                     * Multiline house-first:
                     *
                     *     123 Main Street
                     *     10115 Berlin
                     */
                    (
                        "AddressMultilineHouseFirst",
                        @"(?im)(?<![\p{L}\p{N}])" +
                        houseNumber +
                        @"[ \t]+" +
                        words +
                        @"[ \t]+" +
                        roadDesignator +
                        @"[ \t]*\r?\n" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?=$|\r?\n)",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 6
                     *
                     * Multiline road-first:
                     *
                     *     Hauptstraße 12
                     *     10115 Berlin
                     */
                    (
                        "AddressMultilineRoadFirst",
                        @"(?im)(?<![\p{L}\p{N}])" +
                        words +
                        @"[ \t]+" +
                        roadDesignator +
                        @"[ \t]+" +
                        houseNumber +
                        @"[ \t]*\r?\n" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?=$|\r?\n)",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    /*
                     * ADDRESS GROUP 7
                     *
                     * Turkish form with explicit No/Numara marker and postcode:
                     *
                     *     Atatürk Caddesi No: 25,
                     *     34710 Kadıköy
                     *
                     * Requiring both No/Numara and postcode makes this much
                     * less likely to collide with creative text.
                     */
                    (
                        "AddressTurkish",
                        @"(?<![\p{L}\p{N}])" +
                        words +
                        @"[ \t]+" +
                        @"(?:Sokak|Sokağı|Cadde|Caddesi|Bulvar|Bulvarı)" +
                        @"[ \t]+" +
                        @"(?:No\.?|Numara)\s*:?\s*" +
                        houseNumber +
                        @"[ \t]*,\s*" +
                        postcode +
                        @"[ \t]+" +
                        words +
                        @"(?![\p{L}\p{N}])",
                        "[REDACTED_ADDRESS]",
                        RegexOptions.IgnoreCase
                    ),

                    // UUIDs
                    (
                        "Uuid",
                        @"\b[0-9A-F]{8}-[0-9A-F]{4}-" +
                        @"[1-5][0-9A-F]{3}-[89AB][0-9A-F]{3}-" +
                        @"[0-9A-F]{12}\b",
                        "[REDACTED_UUID]",
                        RegexOptions.IgnoreCase
                    ),

                    // Windows full paths
                    (
                        "WindowsPath",
                        @"[A-Za-z]:\\" +
                        @"(?:[^\\/:*?""<>|\r\n]+\\)*" +
                        @"[^\\/:*?""<>|\r\n]*",
                        "[REDACTED_PATH]",
                        RegexOptions.None
                    ),

                    // Unix/macOS user home paths
                    (
                        "UnixHomePath",
                        @"(?<!\w)/(?:home|Users)/" +
                        @"[^/\s]+(?:/[^\s""']*)?",
                        "[REDACTED_PATH]",
                        RegexOptions.None
                    ),

                    // Credit-card-number-like strings
                    (
                        "CreditCardLikeNumber",
                        @"(?<!\d)\d(?:[ -]?\d){12,18}(?!\d)",
                        "[REDACTED_NUMBER]",
                        RegexOptions.None
                    ),

                    // IPv4 addresses
                    (
                        "IPv4",
                        @"\b(?:(?:25[0-5]|2[0-4]\d|1?\d{1,2})\.){3}" +
                        @"(?:25[0-5]|2[0-4]\d|1?\d{1,2})\b",
                        "[REDACTED_IP]",
                        RegexOptions.None
                    ),

                    // MAC addresses
                    (
                        "MacAddress",
                        @"\b(?:[0-9A-F]{2}[:-]){5}[0-9A-F]{2}\b",
                        "[REDACTED_MAC]",
                        RegexOptions.IgnoreCase
                    ),

                    // Long Base64 values
                    (
                        "LongBase64",
                        @"(?<![A-Za-z0-9+/=])" +
                        @"[A-Za-z0-9+/]{40,}={0,2}" +
                        @"(?![A-Za-z0-9+/=])",
                        "[REDACTED_BASE64]",
                        RegexOptions.None
                    ),

                    // Generic long tokens or keys
                    (
                        "GenericLongToken",
                        @"(?<![A-Za-z0-9_-])" +
                        @"[A-Za-z0-9_-]{40,}" +
                        @"(?![A-Za-z0-9_-])",
                        "[REDACTED_SECRET]",
                        RegexOptions.None
                    )
                };

            var rules =
                new List<(string Name, Regex Regex, string Replacement)>(
                    patterns.Length);

            TimeSpan timeout = TimeSpan.FromMilliseconds(250);

            foreach (var (name, pattern, replacement, options) in patterns)
            {
                try
                {
                    rules.Add((
                        name,
                        new Regex(
                            pattern,
                            options |
                            RegexOptions.Compiled |
                            RegexOptions.CultureInvariant,
                            timeout),
                        replacement));
                }
                catch (ArgumentException)
                {
                    // Skip invalid patterns without disabling the redaction system entirely.
                }
            }

            return rules.ToArray();
        }

        private static readonly Lazy<
            (string Name, Regex Regex, string Replacement)[]> _maskRules =
            new(
                CreateMaskRules,
                LazyThreadSafetyMode.ExecutionAndPublication);
    }
}