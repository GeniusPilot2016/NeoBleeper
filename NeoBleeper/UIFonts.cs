// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using NeoBleeper.Properties;
using System.Drawing.Text;

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

            // Standard fonts
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

        /// <summary>
        /// Creates a new font instance using the first available private font family with the specified size and style.
        /// </summary>
        /// <param name="size">The size, in points, of the font to create. Must be greater than 0.</param>
        /// <param name="style">The style to apply to the font, such as bold or italic.</param>
        /// <returns>A <see cref="Font"/> object initialized with the specified size and style.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the UIFonts object has been disposed.</exception>
        public Font SetUIFont(float size, FontStyle style)
        {
            if (disposed) throw new ObjectDisposedException(nameof(UIFonts));
            Font font = new Font(privateFonts.Families[0], size, style, GraphicsUnit.Point);
            return font;
        }

        /// <summary>
        /// Creates a new font instance from the primary private font family at the specified size.
        /// </summary>
        /// <param name="size">The size, in points, of the font to create. Must be greater than zero.</param>
        /// <returns>A <see cref="Font"/> object representing the primary private font family at the specified size.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the UIFonts object has been disposed.</exception>
        public Font SetUIFont(float size)
        {
            if (disposed) throw new ObjectDisposedException(nameof(UIFonts));
            Font font = new Font(privateFonts.Families[0], size, GraphicsUnit.Point);
            return font;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <remarks>This method is called by both the public Dispose() method and the finalizer. When
        /// disposing is true, this method releases all resources held by managed objects. When disposing is false, only
        /// unmanaged resources are released. Override this method to release additional resources in a derived
        /// class.</remarks>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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

        /// <summary>
        /// Applies the application's standard UI fonts to all controls within the specified form, including context
        /// menu items.
        /// </summary>
        /// <remarks>This method updates the fonts of all controls recursively within the provided form,
        /// as well as any context menu items defined as fields. It is typically used to ensure a consistent look and
        /// feel across the application's user interface. The method suspends and resumes the form's layout to optimize
        /// performance during the update.</remarks>
        /// <param name="form">The form whose controls and context menu items will have their fonts updated. Cannot be null.</param>
        public static void SetFonts(Form form)
        {
            form.SuspendLayout();
            UIFonts uiFonts = UIFonts.Instance;
            SetFontsRecursive(form, uiFonts);

            foreach (var field in form.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (field.FieldType == typeof(ContextMenuStrip))
                {
                    if (field.GetValue(form) is ContextMenuStrip cms)
                    {
                        SetToolStripItemFontsRecursive(cms.Items, uiFonts);
                    }
                }
            }
            form.ResumeLayout();
        }

        /// <summary>
        /// Recursively sets the font of each ToolStripItem in the specified collection and all nested drop-down items
        /// using the provided UI font settings.
        /// </summary>
        /// <remarks>If an item in the collection is a ToolStripMenuItem with drop-down items, the method
        /// applies the font settings recursively to all nested items. If an error occurs while setting the font for an
        /// item, the error is logged and processing continues for the remaining items.</remarks>
        /// <param name="items">The collection of ToolStripItem objects whose fonts will be updated. This may include menu items, buttons,
        /// or other ToolStrip controls.</param>
        /// <param name="uiFonts">The UIFonts instance used to determine and apply the desired font settings to each ToolStripItem.</param>
        private static void SetToolStripItemFontsRecursive(ToolStripItemCollection items, UIFonts uiFonts)
        {
            foreach (ToolStripItem item in items)
            {
                try
                {
                    item.Font = uiFonts.SetUIFont(item.Font.Size, item.Font.Style);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error setting font for menu item {item.Name}: {ex.Message}", Logger.LogTypes.Error);
                }

                // Apply recursively for drop-down items
                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    SetToolStripItemFontsRecursive(menuItem.DropDownItems, uiFonts);
                }
            }
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

                // Special handling for ToolStrip controls
                if (ctrl is MenuStrip menuStrip)
                {
                    SetToolStripItemFontsRecursive(menuStrip.Items, uiFonts);
                }
                else if (ctrl is ToolStrip toolStrip)
                {
                    SetToolStripItemFontsRecursive(toolStrip.Items, uiFonts);
                }
                // Special handling for ListView items
                else if (ctrl is ListView listView)
                {
                    foreach (ListViewItem item in listView.Items)
                    {
                        try
                        {
                            item.Font = uiFonts.SetUIFont(listView.Font.Size, listView.Font.Style);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error setting font for ListViewItem {item.Text}: {ex.Message}", Logger.LogTypes.Error);
                        }
                    }
                }
                // Special handling for TreeView nodes
                else if (ctrl is TreeView treeView)
                {
                    foreach (TreeNode node in treeView.Nodes)
                    {
                        SetTreeNodeFontRecursive(node, uiFonts, treeView.Font.Size, treeView.Font.Style);
                    }
                }
                // Special handling for TabControl pages
                else if (ctrl is TabControl tabControl)
                {
                    foreach (TabPage page in tabControl.TabPages)
                    {
                        try
                        {
                            page.Font = uiFonts.SetUIFont(page.Font.Size, page.Font.Style);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error setting font for TabPage {page.Name}: {ex.Message}", Logger.LogTypes.Error);
                        }
                        SetFontsRecursive(page, uiFonts);
                    }
                }
                // Special handling for DataGridView (set font for cells)
                else if (ctrl is DataGridView dgv)
                {
                    try
                    {
                        dgv.Font = uiFonts.SetUIFont(dgv.Font.Size, dgv.Font.Style);
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            col.DefaultCellStyle.Font = uiFonts.SetUIFont(dgv.Font.Size, dgv.Font.Style);
                        }
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            row.DefaultCellStyle.Font = uiFonts.SetUIFont(dgv.Font.Size, dgv.Font.Style);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error setting font for DataGridView: {ex.Message}", Logger.LogTypes.Error);
                    }
                }
                // Special handling for PropertyGrid controls (set font for grid)
                else if (ctrl is PropertyGrid propertyGrid)
                {
                    try
                    {
                        propertyGrid.Font = uiFonts.SetUIFont(propertyGrid.Font.Size, propertyGrid.Font.Style);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error setting font for PropertyGrid: {ex.Message}", Logger.LogTypes.Error);
                    }
                }

                if (ctrl.HasChildren)
                {
                    SetFontsRecursive(ctrl, uiFonts);
                }
            }
        }

        private static void SetTreeNodeFontRecursive(TreeNode node, UIFonts uiFonts, float size, FontStyle style)
        {
            try
            {
                node.NodeFont = uiFonts.SetUIFont(size, style);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error setting font for TreeNode {node.Text}: {ex.Message}", Logger.LogTypes.Error);
            }
            foreach (TreeNode child in node.Nodes)
            {
                SetTreeNodeFontRecursive(child, uiFonts, size, style);
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you are finished using the object to free unmanaged resources
        /// and perform other cleanup operations. After calling Dispose, the object should not be used
        /// further.</remarks>
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
