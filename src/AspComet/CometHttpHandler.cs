using System;
using System.Runtime.Serialization.Json;
using System.Web;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        private readonly MessageBroker messageBroker = new MessageBroker(new InMemoryClientRepository());

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

            CometAsyncResult asyncResult = new CometAsyncResult(context, callback, asyncState);
            this.messageBroker.HandleMessages(request, asyncResult);
            return asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            CometAsyncResult cometAsyncResult = (CometAsyncResult) result;
            cometAsyncResult.HttpContext.Response.Write(MessageConverter.ToJson(cometAsyncResult.ResponseMessages));            
        }
    }
}
