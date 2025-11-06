using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NeoBleeper
{
    public class LogInHelper
    {
        string clientSecret;
        string clientId;
        string clientURL;
        public LogInHelper()
        {
            clientId = "YOUR_CLIENT ID"; // Placeholder for client ID.
            clientSecret = "YOUR CLIENT SECRET"; // Placeholder for client secret.
            clientURL = "CLIENT REDIRECT URL"; // Placeholder for client redirect URL.
        }
        public static void LogIn()
        {
            DateTime currentDate = DateTime.Now; // Get the current date and time to compare user's age.
            Image profileImage = null; // Create an Image object to hold the profile picture.
            string userName = String.Empty; // Initialize an empty string for the user's name.

        }
    }
}
