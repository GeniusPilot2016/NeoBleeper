
namespace NeoBleeper
{
    public static class set_text_color
    {
        public static Color GetTextColor(Color backgroundColor) 
        {
            // Brightness calculation (YIQ Equation)
            double brightness = ((backgroundColor.R * 299) + (backgroundColor.G * 587) + (backgroundColor.B * 114)) / 1000;

            // Choose text color to brightness
            return brightness > 64 ? Color.Black : Color.White;
        }
    }
}
