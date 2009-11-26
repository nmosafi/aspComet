using System.Collections.Generic;

using AspComet.Eventing;

namespace AspComet.Samples.Chat
{
    public class HandshakeAuthenticator
    {
        public void CheckHandshake(HandshakingEvent ev)
        {
            // Based on the (client side) authentication Howto at http://cometd.org/documentation/howtos/authentication
            // If we have credentials, then they are transferred via "ext"
            // ext will be a Dictionary<string,object> with a key, "authentication"
            // authentication will be a Dictionary<string,object> with two keys, "user" and "credentials"            

            // Note, the following lines could be collapsed in to one giant if() statement, but they are expanded for clarity
            if (ev.Handshake.ext is Dictionary<string, object>)
            {
                Dictionary<string, object> dictExt = (Dictionary<string, object>)ev.Handshake.ext;
                if (dictExt.ContainsKey("authentication")
                    && dictExt["authentication"] is Dictionary<string, object>)
                {
                    Dictionary<string, object> dictAuth = (Dictionary<string, object>)dictExt["authentication"];
                    // Authenticate the client
                    if (dictAuth["user"] is string
                            && dictAuth["credentials"] is string
                            && (string)dictAuth["credentials"] == "password")
                    {
                        AuthenticatedClient authClient = (AuthenticatedClient)ev.Client;
                        authClient.username = (string)dictAuth["user"];
                        authClient.password = (string)dictAuth["credentials"];
                        return;
                    }
                    else
                    {
                        ev.CancellationReason = "Incorrect username or password";
                    }
                }
            }

            // If we got this far, we couldn't authenticate 
            if (ev.CancellationReason == null)
            {
                ev.CancellationReason = "Credentials not supplied";
            }
            ev.Cancel = true;
            ev.Retry = false;
        }
    }
}
