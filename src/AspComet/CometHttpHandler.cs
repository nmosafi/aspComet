using System;
using System.Web;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        private IMessageBus messageBus;
        private readonly object messageBusCheckingSyncRoot = new object();

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            EnsureMessageBus();

            throw new Exception("Cannot process synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object asyncState)
        {
            EnsureMessageBus();

            Message[] request = MessageConverter.FromJson(context.Request);
            CometAsyncResult asyncResult = new CometAsyncResult(context, callback, asyncState);
            this.messageBus.HandleMessages(request, asyncResult);
            return asyncResult;
        }

        private void EnsureMessageBus()
        {
            if (this.messageBus != null) return;
            lock (this.messageBusCheckingSyncRoot)
            {
                if (this.messageBus != null) return;
                this.messageBus = Configuration.MessageBus;
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            CometAsyncResult cometAsyncResult = (CometAsyncResult) result;
            cometAsyncResult.HttpContext.Response.Write(MessageConverter.ToJson(cometAsyncResult.ResponseMessages));            
        }
    }
}
