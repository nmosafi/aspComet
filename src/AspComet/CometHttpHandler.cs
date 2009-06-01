using System;
using System.Web;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        private readonly MessageBus messageBus = new MessageBus(new InMemoryClientRepository());

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
            this.messageBus.HandleMessages(request, asyncResult);
            return asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            CometAsyncResult cometAsyncResult = (CometAsyncResult) result;
            cometAsyncResult.HttpContext.Response.Write(MessageConverter.ToJson(cometAsyncResult.ResponseMessages));            
        }
    }
}
