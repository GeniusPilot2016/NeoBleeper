using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class OllamaUtility
    {
        public static async Task<string> RunOllamaCommands(string command, string[] args)
        {
            try
            {
                using var process = new Process();

                process.StartInfo.FileName = "ollama";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.ArgumentList.Add(command);

                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        process.StartInfo.ArgumentList.Add(arg);
                    }
                }

                process.Start();

                Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
                Task<string> stderrTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync().ConfigureAwait(false);

                string output = await stdoutTask.ConfigureAwait(false);
                string error = await stderrTask.ConfigureAwait(false);

                return string.IsNullOrWhiteSpace(output) ? error.Trim() : output.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string> GenerateTextResponse(string prompt, string modelName, string systemPrompt = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(modelName))
                    return string.Empty;

                string finalPrompt = string.IsNullOrWhiteSpace(systemPrompt)
                    ? $"\"{prompt}\""
                    : $"\"{systemPrompt}{Environment.NewLine}{Environment.NewLine}{prompt}\"";

                return await RunOllamaCommands("run", new[] { modelName, finalPrompt }).ConfigureAwait(false);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
