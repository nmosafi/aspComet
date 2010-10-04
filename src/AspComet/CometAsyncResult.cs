using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

using AspComet.Transports;

namespace AspComet
{
    public class CometAsyncResult : ICometAsyncResult
    {
        private readonly HttpContextBase httpContext;
        private readonly AsyncCallback callback;
        private readonly object asyncState;
        private IEnumerable<Message> responseMessages;
        private ITransport transport = LongPollingTransport.Instance;

        public CometAsyncResult(HttpContextBase httpContext, AsyncCallback callback, object asyncState)
        {
            this.httpContext = httpContext;
            this.callback = callback;
            this.asyncState = asyncState;
            if ( httpContext.Request.HttpMethod == "GET" ) {
                transport = CallbackPollingTransport.Instance;
            }
        }

        public bool IsCompleted { get; private set; }

        public ITransport Transport
        {
            set { this.transport = value; }
        }

        public HttpContextBase HttpContext
        {
            get { return this.httpContext; }
        }

        public void SendAwaitingMessages()
        {
            transport.SendMessages(HttpContext.Response, HttpContext.Request, responseMessages);
        }

        public void CompleteRequestWithMessages(IEnumerable<Message> responseMessages)
        {
            // Quick explanation of how this works.  This will be called to send the response back, the callback 
            // will go back to the CometHttpHandler's EndProcessRequest() which will then call SendAwaitingMessages()
            this.responseMessages = responseMessages;
            this.IsCompleted = true;
            this.callback(this);
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotSupportedException("Not required for COMET implementation"); }
        }

        public object AsyncState
        {
            get { return asyncState; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }
    }
}