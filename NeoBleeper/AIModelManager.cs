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
            if (!isOllamaInstalled && !isGoogleGeminiAvailable)
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
                else
                {
                    textBox1.Text = Settings1.Default.OllamaClientURL; // Pre-fill the text box with the saved Ollama Client URL from settings
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageForm.Show("Please enter the URL for the Ollama Client.", "URL Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!textBox1.Text.StartsWith("http://") && !textBox1.Text.StartsWith("https://"))
            {
                MessageForm.Show("Please enter a valid URL that starts with http:// or https://", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Settings1.Default.OllamaClientURL = textBox1.Text.Trim(); // Save the URL to settings
            Settings1.Default.Save(); // Persist the settings
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Settings1.Default.OllamaClientURL = "http://localhost:11434"; // Reset to default URL
            Settings1.Default.Save(); // Persist the settings 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string folder = folderBrowserDialog1.ShowDialog() == DialogResult.OK ? folderBrowserDialog1.SelectedPath : null;
            if (folder != null)
            {
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    if (CheckForStrictlyForbiddenCharacters(folder))
                    {
                        MessageForm.Show("The selected folder path contains characters that are strictly forbidden in file paths (such as <, >, :, \", |, ?, *). Please choose a different folder without these characters.", "Invalid Folder Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (DoesFolderNameContainSpecialCharacters(folder))
                    {
                        DialogResult result = MessageForm.Show("The selected folder path contains special characters (such as emojis, non-Latin characters, or other symbols) that may cause issues with file system operations or with Ollama. Do you want to proceed with this folder?", "Special Characters Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            textBox2.Text = folder; // Set the selected folder path to the text box
                        }
                    }
                    else
                    {
                        textBox2.Text = folder; // Set the selected folder path to the text box
                    }
                }
            }
        }
        /// <summary>
        /// Checks if the provided folder path contains any special characters that are not allowed in file paths
        /// such as emojis, non-Latin characters, or other symbols that could cause issues with file system operations. 
        /// This method iterates through the set of invalid path characters defined by the .NET framework and checks if 
        /// any of them are present in the folder path. Also, if these paths are used, the Ollama may give an error like 
        /// "Error: 500 Internal Server Error: llama-server process has terminated: exit status 1: error loading model: 
        /// llama_model_loader: failed to load model from {path}" error that can be caused by special characters in the folder path. 
        /// If any special character is found, the method returns true, indicating that the folder path may not be suitable for use in file system operations or with Ollama.
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns> true if the folder path contains special characters; otherwise, false.</returns>
        private bool DoesFolderNameContainSpecialCharacters(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return true;

            foreach (char c in folderPath)
            {
                if (char.IsControl(c))
                    return true;

                if (c > 127) // ç, ğ, ü, ş, ö, emojis vb.
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the provided folder path contains any characters that are strictly forbidden in file paths, such as <, >, :, ", |, ?, *, and others defined by the .NET framework. This method uses the Path.GetInvalidPathChars() method to retrieve the set of invalid characters and checks if any of them are present in the folder path. If any forbidden character is found, the method returns true, indicating that the folder path is not valid for use in file system operations.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>true if the folder path contains strictly forbidden characters; otherwise, false.</returns>

        private bool CheckForStrictlyForbiddenCharacters(string folderPath)
        {
            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            foreach (char c in invalidChars)
            {
                if (folderPath.Contains(c))
                    return true;
            }
            return false;
        }
    }
}
