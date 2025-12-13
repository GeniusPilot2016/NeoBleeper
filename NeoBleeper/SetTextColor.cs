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
    public static class SetTextColor
    {
        /// <summary>
        /// Determines an appropriate text color (black or white) for readability against the specified background
        /// color.
        /// </summary>
        /// <remarks>This method uses a standard brightness calculation to select a text color that
        /// ensures sufficient contrast for readability. It is useful for dynamically setting text color in user
        /// interfaces based on varying background colors.</remarks>
        /// <param name="backgroundColor">The background color for which to determine a contrasting text color.</param>
        /// <returns>A Color value of either Color.Black or Color.White, chosen to provide optimal contrast with the specified
        /// background color.</returns>
        public static Color GetTextColor(Color backgroundColor)
        {
            // Brightness calculation (YIQ Equation)
            double brightness = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;

            // Choose text color to brightness
            return brightness > 64 ? Color.Black : Color.White;
        }
    }
}
