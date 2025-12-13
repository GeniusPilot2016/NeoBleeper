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

namespace NeoBleeper
{
    public static class SetInactiveNoteColor
    {
        /// <summary>
        /// Calculates an appropriate color for displaying inactive notes based on the specified background color.
        /// </summary>
        /// <remarks>This method ensures that inactive notes remain visible and distinguishable regardless
        /// of the background color by adjusting the brightness of the returned color. The adjustment is based on the
        /// perceived brightness of the input color.</remarks>
        /// <param name="backgroundColor">The background color used as the basis for determining the inactive note color.</param>
        /// <returns>A color that contrasts with the specified background color, adjusted to appear either lighter or darker
        /// depending on the background's brightness.</returns>
        public static Color GetInactiveNoteColor(Color backgroundColor)
        {
            // Brightness calculation (YIQ Equation)
            double brightness = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;

            // Adjust color based on brightness
            if (brightness > 64)
            {
                // Make the color slightly darker
                return Color.FromArgb(
                    Math.Max(0, backgroundColor.R - 75),
                    Math.Max(0, backgroundColor.G - 75),
                    Math.Max(0, backgroundColor.B - 75)
                );
            }
            else
            {
                // Make the color slightly lighter
                return Color.FromArgb(
                    Math.Min(255, backgroundColor.R + 75),
                    Math.Min(255, backgroundColor.G + 75),
                    Math.Min(255, backgroundColor.B + 75)
                );
            }
        }
    }
}
