using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class UIFonts
    {
        private static UIFonts instance;
        private static readonly object lockObject = new object();

        private PrivateFontCollection privateFonts = new PrivateFontCollection();
        private string[] fontFiles;
        private bool disposed = false;

        private UIFonts()
        {
            fontFiles = new string[]
            {
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName()
            };

            File.WriteAllBytes(fontFiles[0], Resources.HarmonyOS_Sans_Black);
            privateFonts.AddFontFile(fontFiles[0]);
            File.WriteAllBytes(fontFiles[1], Resources.HarmonyOS_Sans_Black_Italic);
            privateFonts.AddFontFile(fontFiles[1]);
            File.WriteAllBytes(fontFiles[2], Resources.HarmonyOS_Sans_Bold);
            privateFonts.AddFontFile(fontFiles[2]);
            File.WriteAllBytes(fontFiles[3], Resources.HarmonyOS_Sans_Bold_Italic);
            privateFonts.AddFontFile(fontFiles[3]);
            File.WriteAllBytes(fontFiles[4], Resources.HarmonyOS_Sans_Light);
            privateFonts.AddFontFile(fontFiles[4]);
            File.WriteAllBytes(fontFiles[5], Resources.HarmonyOS_Sans_Light_Italic);
            privateFonts.AddFontFile(fontFiles[5]);
            File.WriteAllBytes(fontFiles[6], Resources.HarmonyOS_Sans_Medium);
            privateFonts.AddFontFile(fontFiles[6]);
            File.WriteAllBytes(fontFiles[7], Resources.HarmonyOS_Sans_Medium_Italic);
            privateFonts.AddFontFile(fontFiles[7]);
            File.WriteAllBytes(fontFiles[8], Resources.HarmonyOS_Sans_Regular);
            privateFonts.AddFontFile(fontFiles[8]);
            File.WriteAllBytes(fontFiles[9], Resources.HarmonyOS_Sans_Thin);
            privateFonts.AddFontFile(fontFiles[9]);
            File.WriteAllBytes(fontFiles[10], Resources.HarmonyOS_Sans_Thin_Italic);
            privateFonts.AddFontFile(fontFiles[10]);
        }

        public static UIFonts Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new UIFonts();
                        }
                    }
                }
                return instance;
            }
        }

        public Font SetUIFont(float size, FontStyle style)
        {
            if (disposed) throw new ObjectDisposedException(nameof(UIFonts));
            Font font = new Font(privateFonts.Families[0], size, style);
            return font;
        }

        public Font SetUIFont(float size)
        {
            if (disposed) throw new ObjectDisposedException(nameof(UIFonts));
            Font font = new Font(privateFonts.Families[0], size);
            return font;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    privateFonts.Dispose();
                }

                // Dispose unmanaged resources
                foreach (string file in fontFiles)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception)
                    {
                        // Handle exception (e.g., log it)
                        // It's possible the file is already deleted or inaccessible
                    }
                }

                instance = null;
                disposed = true;
            }
        }
        public static void setFonts(Form form)
        {
            UIFonts uiFonts = UIFonts.Instance;
            SetFontsRecursive(form, uiFonts);
        }

        private static void SetFontsRecursive(Control parent, UIFonts uiFonts)
        {
            foreach (Control ctrl in parent.Controls)
            {
                try
                {
                    ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error setting font for control {ctrl.Name}: {ex.Message}", Logger.LogTypes.Error);
                }

                // Special Handling for MenuStrip and ToolStrip
                if (ctrl is MenuStrip menuStrip)
                {
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        try
                        {
                            item.Font = uiFonts.SetUIFont(item.Font.Size, item.Font.Style);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error setting font for menu item {item.Name}: {ex.Message}", Logger.LogTypes.Error);
                        }
                    }
                }

                // Other ToolStrip types
                if (ctrl is ToolStrip toolStrip)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        try
                        {
                            item.Font = uiFonts.SetUIFont(item.Font.Size, item.Font.Style);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error setting font for toolstrip item {item.Name}: {ex.Message}", Logger.LogTypes.Error);
                        }
                    }
                }

                if (ctrl.HasChildren)
                {
                    SetFontsRecursive(ctrl, uiFonts);
                }
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~UIFonts()
        {
            // Finalizer calls Dispose(false)
            Dispose(disposing: false);
        }
    }
}
