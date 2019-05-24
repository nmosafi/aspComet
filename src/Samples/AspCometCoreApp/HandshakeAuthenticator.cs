using System.Collections.Generic;
using AspComet;
using AspComet.Eventing;

namespace AspCometCoreApp
{
    public class HandshakeAuthenticator 
    {
        public void CheckHandshake(HandshakingEvent ev)
        {

            // Based on the (client side) authentication Howto at http://cometd.org/documentation/howtos/authentication
            // If we have credentials, then they are transferred via "ext"
            // ext will be a Dictionary<string,object> with a key, "authentication"
            // authentication will be a Dictionary<string,object> with two keys, "user" and "credentials"            

            var authenticationDictionary = ev.Handshake.GetExt<Dictionary<string, object>>("authentication");

            object user;
            object credentials;
            if (authenticationDictionary != null &&
                authenticationDictionary.TryGetValue("user", out user) && 
                authenticationDictionary.TryGetValue("credentials", out credentials))
            {
                if (user is string && credentials.Equals("password"))
                {
                    AuthenticatedClient authenticatedClient = (AuthenticatedClient) ev.Client;
                    authenticatedClient.Username = (string) user;
                    authenticatedClient.Password = (string) credentials;
                    return;
                }

                ev.CancellationReason = "Incorrect username or password";
            }
            else
            {
                ev.CancellationReason = "Credentials not supplied";
            }

            ev.Cancel = true;
            ev.Retry = false;
        }
    }
}
