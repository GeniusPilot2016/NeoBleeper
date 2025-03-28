using System.Diagnostics;

namespace NeoBleeper
{
    public static class check_system_theme
    {
        public static bool IsDarkTheme()
        {
            try
            {
                Process process = new();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments =
                    @"/C reg query HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string[] keys = output.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].Contains("AppsUseLightTheme"))
                    {
                        return keys[i].EndsWith("0");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }
    }
}
