using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoBleeper
{
    public class AuthenticationHelper
    {
        public AuthenticationHelper()
        {

        }
        public async Task<string> Authenticate()
        {
            string token = string.Empty;
            try
            {
                string[] scopes = { "openid", "email", "profile" };

                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = "32429026607-q7n5j4qrd1usfm54umngshn5ifsvd1bq.apps.googleusercontent.com",
                    },
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Google.Apis.Auth") // Token bilgilerini şifreli olarak yerel klasöre kaydeder
                );
                if (credential != null && string.IsNullOrEmpty(credential.Token.AccessToken)) 
                {
                    token = credential.Token.AccessToken;
                }
                else
                {
                    token = string.Empty;
                }
                return token;
            }
            catch
            {
                return string.Empty;
            }
            
        }
    }
}
