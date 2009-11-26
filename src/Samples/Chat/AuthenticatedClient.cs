using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspComet.Samples.Chat
{
    public class AuthenticatedClient : AspComet.Client 
    {
        public string username { get; set; }
        public string password { get; set; }

        public AuthenticatedClient(string id) : base(id)
        {
        }

    }
}
