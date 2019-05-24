using System;
using System.Collections.Generic;
//using System.Web;
using Microsoft.AspNetCore.Http;


namespace AspComet
{
    public interface ITransport
    {
        void SendMessages(HttpResponse response, IEnumerable<Message> messages);
    }
}
