﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class set_playing_note_color
    {
        public static Color GetPlayingNoteColor(Color backgroundColor)
        {
            // Brightness calculation (YIQ Equation)
            double brightness = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;

            // Adjust color based on brightness
            if (brightness > 64)
            {
                // Make the color slightly darker
                return Color.FromArgb(
                    Math.Max(0, backgroundColor.R - 31),
                    Math.Max(0, backgroundColor.G - 31),
                    Math.Max(0, backgroundColor.B - 31)
                );
            }
            else
            {
                // Make the color slightly lighter
                return Color.FromArgb(
                    Math.Min(255, backgroundColor.R + 31),
                    Math.Min(255, backgroundColor.G + 31),
                    Math.Min(255, backgroundColor.B + 31)
                );
            }
        }
    }
}
