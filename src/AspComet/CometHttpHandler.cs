using System;
using System.Runtime.Serialization.Json;
using System.Web;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        private readonly Action<HttpContext, Message[]> action;
        private readonly MessageBroker messageBroker = new MessageBroker(new InMemoryClientRepository());

        public CometHttpHandler()
        {
            this.action = (context, request) => 
                context.Response.Write(MessageConverter.ToJson(this.messageBroker.HandleMessages(request)));
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new Exception("Cannot process synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object asyncState)
        {
            Message[] request = MessageConverter.FromJson(context.Request, "message");

            //this.messageBroker.HandleMessages(context, request, callback, asyncState)
            return action.BeginInvoke(context, request, callback, asyncState);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            action.EndInvoke(result);
        }
    }
}
