using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private Dictionary<string, string> modelNamesAndDisplayNames = new Dictionary<string, string>();
        private string noModelFoundText;
        public AIModelManager()
        {
            InitializeComponent();
            noModelFoundText = label3.Text; // Store the original text of label3 to use it later if needed
            bool isGoogleGeminiAvailable = CreateMusicWithAI.IsAvailableInCountry();
            if (!OllamaUtility.EnsureOllamaIsRunning())
            {
                int heightWillBeSubtracted = groupBox1.Height + flowLayoutPanel1.Padding.All;
                flowLayoutPanel1.Controls.Remove(groupBox1);
                this.Height -= heightWillBeSubtracted;
            }
            else
            {
                textBox1.Text = Settings1.Default.OllamaClientURL; // Pre-fill the text box with the saved Ollama Client URL from settings
                FillModelsListAndCheckEnabled(); // Populate the models list and set the checked states based on saved settings
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
        
        enum LocalModelsStatus
        {
            PleaseWait,
            NoModelsFound,
            ModelsFound
        }

        private void SetLocalModelsStatus(LocalModelsStatus status)
        {
            switch (status)
            {
                case LocalModelsStatus.PleaseWait:
                    checkedListBox1.Enabled = false;
                    checkedListBox1.Cursor = Cursors.WaitCursor;
                    label3.Cursor = Cursors.WaitCursor;
                    label3.Text = "Please wait while querying local AI models...";
                    label3.Visible = true;
                    break;
                case LocalModelsStatus.NoModelsFound:
                    checkedListBox1.Enabled = false;
                    checkedListBox1.Cursor = Cursors.Default;
                    label3.Cursor = Cursors.Default;
                    label3.Text = noModelFoundText; // Use the original text stored in the constructor for the "no models found" message
                    label3.Visible = true;
                    break;
                case LocalModelsStatus.ModelsFound:
                    checkedListBox1.Enabled = true;
                    checkedListBox1.Cursor = Cursors.Default;
                    label3.Visible = false;
                    break;
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

        private async void FillModelsListAndCheckEnabled()
        {
            checkedListBox1.Items.Clear();
            SetLocalModelsStatus(LocalModelsStatus.PleaseWait);
            string targetUrl = Settings1.Default.OllamaClientURL;
            Dictionary<string, string> modelsDictionary = await OllamaUtility.GetModelNamesAndDisplayNamesAsync(targetUrl);
            modelNamesAndDisplayNames = modelsDictionary;
            foreach (var modelPair in modelsDictionary)
            {
                checkedListBox1.Items.Add(modelPair.Value);
            }

            if(modelsDictionary.Count == 0)
            {
                SetLocalModelsStatus(LocalModelsStatus.NoModelsFound);
                return;
            }
            if (Settings1.Default.EnabledLocalModels == null)
                Settings1.Default.EnabledLocalModels = new System.Collections.Specialized.StringCollection();
            StringCollection enabledModelsCollection = Settings1.Default.EnabledLocalModels;
            foreach (string enabledModel in enabledModelsCollection)
            {
                if (modelsDictionary.TryGetValue(enabledModel, out string displayName))
                {
                    int index = checkedListBox1.Items.IndexOf(displayName);
                    if (index != -1)
                    {
                        checkedListBox1.SetItemChecked(index, true);
                    }
                }
            }
            SetLocalModelsStatus(LocalModelsStatus.ModelsFound);
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string displayName = checkedListBox1.Items[e.Index]?.ToString();
            if (string.IsNullOrEmpty(displayName))
                return;
            // Find the corresponding model key for the display name by searching through the modelNamesAndDisplayNames dictionary. This is necessary because the CheckedListBox contains display names, but it needs to update the settings based on the model keys. If the display name is not found in the dictionary, it can't proceed with updating the settings, so it returns early.
            string modelKey = null;
            foreach (var kvp in modelNamesAndDisplayNames)
            {
                if (kvp.Value == displayName)
                {
                    modelKey = kvp.Key;
                    break;
                }
            }
            if (modelKey == null)
                return;
            // Prepare the EnabledLocalModels collection in settings if it's null, then add or remove the model key based on the new check state
            if (Settings1.Default.EnabledLocalModels == null)
                Settings1.Default.EnabledLocalModels = new System.Collections.Specialized.StringCollection();
            if (e.NewValue == CheckState.Checked)
            {
                if (!Settings1.Default.EnabledLocalModels.Contains(modelKey))
                {
                    Settings1.Default.EnabledLocalModels.Add(modelKey);
                    Settings1.Default.Save();
                }
            }
            else // unchecked
            {
                if (Settings1.Default.EnabledLocalModels.Contains(modelKey))
                {
                    Settings1.Default.EnabledLocalModels.Remove(modelKey);
                    Settings1.Default.Save();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OllamaUtility.EnsureOllamaIsRunning())
            {
                FillModelsListAndCheckEnabled(); // Refresh the list of models and their checked states when the "Refresh Models" button is clicked. This allows the user to see any changes in available models or update their selections after modifying the Ollama Client URL or after starting/stopping the Ollama server.
            }
        }
    }
}
