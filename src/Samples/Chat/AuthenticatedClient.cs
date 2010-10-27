using System;

namespace AspComet.Samples.Chat
{
    public class AuthenticatedClient : Client 
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticatedClient(string id) : base(id)
        {
        }

        public bool HasUsername(string username)
        {
            return string.Equals(Username, username, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
