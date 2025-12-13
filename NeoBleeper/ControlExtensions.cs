// NeoBleeper - AI// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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

using System.Reflection;

namespace NeoBleeper
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Enables or disables double buffering for the specified control to reduce flicker during repaint operations.
        /// </summary>
        /// <remarks>Double buffering can improve rendering performance and visual quality for controls
        /// that experience flickering during redraws. Not all controls support double buffering, and enabling it may
        /// have no effect on some controls.</remarks>
        /// <param name="control">The control for which to set double buffering. Cannot be null.</param>
        /// <param name="enabled">true to enable double buffering; otherwise, false.</param>
        public static void DoubleBuffering(this Control control, bool enabled)
        {
            var method = typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(control, new object[] { ControlStyles.OptimizedDoubleBuffer, enabled });
        }

    }
}