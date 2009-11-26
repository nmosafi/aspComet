using AspComet.Eventing;

namespace AspComet.Samples.Chat
{
    public class HandshakeAuthenticator
    {
        public void CheckHandshake(HandshakingEvent ev)
        {
            if (ev.Handshake.authentication == null)
            {
                ev.Cancel = true;
                ev.CancellationReason = "Authentication failed; no credentials were supplied";
            } 
            else if( ev.Handshake.authentication["credentials"] != "password" ) 
            {
                ev.Cancel = true;
                ev.CancellationReason = "Authentication failed; incorrect username or password";
            } 
            else 
            {
                AuthenticatedClient authClient = (AuthenticatedClient)ev.Client;
                authClient.username = ev.Handshake.authentication["user"];
                authClient.password = ev.Handshake.authentication["credentials"];
            }
        }
    }
}
