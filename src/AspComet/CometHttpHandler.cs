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

            HttpContextBase abstractContext = new HttpContextWrapper(context);
            Message[] request = MessageConverter.FromJson(abstractContext.Request);
            CometAsyncResult asyncResult = new CometAsyncResult(abstractContext, callback, asyncState);
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

            // TODO Find some way to determine the correct transport. Possibly earlier, saving it in the CometAsyncResult.
            ITransport transport = new Transports.LongPollingTransport();
            transport.SendMessages(cometAsyncResult.HttpContext, cometAsyncResult.ResponseMessages);
        }
    }
}
