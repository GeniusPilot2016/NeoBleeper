using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class AIModelManager : Form
    {
        public AIModelManager()
        {
            InitializeComponent();
            bool isOllamaInstalled = IsOllamaInstalled();
            bool isGoogleGeminiAvailable = CreateMusicWithAI.IsAvailableInCountry();
            if(!isOllamaInstalled && !isGoogleGeminiAvailable)
            {
                MessageForm.Show("Ollama is not installed on this system, and it appears that Google Gemini™ is not available in your country. Please install Ollama to use the AI Model Manager.", "Ollama Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                if (!IsOllamaInstalled())
                {
                    int heightWillBeSubtracted = groupBox1.Height + flowLayoutPanel1.Padding.All;
                    flowLayoutPanel1.Controls.Remove(groupBox1);
                    this.Height -= heightWillBeSubtracted;
                }
                if (!isGoogleGeminiAvailable)
                {
                    int heightWillBeSubtracted = groupBoxCreateMusicWithAI.Height + flowLayoutPanel1.Padding.All;
                    flowLayoutPanel1.Controls.Remove(groupBoxCreateMusicWithAI);
                    this.Height -= heightWillBeSubtracted;
                }
                else if (IsPotentiallyPaidApiCountry())
                {
                    labelGoogleGeminiAPIWarning.Visible = true;
                }
                else
                {
                    int heightWillBeSubtracted = labelGoogleGeminiAPIWarning.Height + flowLayoutPanel1.Padding.All;
                    flowLayoutPanel1.Controls.Remove(labelGoogleGeminiAPIWarning);
                    this.Height -= heightWillBeSubtracted;
                }
            }
        }
        /// <summary>
        /// Contains the set of ISO 3166-1 alpha-2 country codes where the API may be subject to paid access
        /// requirements.
        /// </summary>
        /// <remarks>This set includes all countries in the European Economic Area (EEA), as well as
        /// Switzerland and the United Kingdom. Use this collection to determine if a given country code is associated
        /// with regions where API usage may incur charges or require special handling.</remarks>
        private static readonly HashSet<string> PotentiallyPaidApiCountries = new()
        {
            // European Economic Area (EEA) countries (ISO 3166-1 alpha-2 codes)
            "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "GR", "HU",
            "IS", "IE", "IT", "LV", "LI", "LT", "LU", "MT", "NL", "NO", "PL", "PT", "RO",
            "SK", "SI", "ES", "SE",
            // Additionally Switzerland and the United Kingdom
            "CH", "GB"
        };

        /// <summary>
        /// Determines whether the current user's country is considered a potentially paid API country.
        /// </summary>
        /// <remarks>This method uses the current system region settings to identify the user's country.
        /// If the country cannot be determined due to an error, the method returns false.</remarks>
        /// <returns>true if the user's country is in the list of potentially paid API countries; otherwise, false.</returns>
        public static bool IsPotentiallyPaidApiCountry()
        {
            try
            {
                string countryCode = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
                return PotentiallyPaidApiCountries.Contains(countryCode);
            }
            catch (Exception ex)
            {
                Logger.Log("Error determining user's country for API pricing: " + ex.Message, Logger.LogTypes.Error);
                return false; // Default to false if there's an error
            }
        }

        private bool IsOllamaInstalled()
        {
            try
            {
                string result = OllamaUtility.RunOllamaCommands("--version", null).GetAwaiter().GetResult();
                return !string.IsNullOrWhiteSpace(result);
            }
            catch
            {
                return false;
            }
        }
    }
}
