using System;
using System.Threading;
using System.Web;

namespace AspComet
{
    public class CometAsyncResult : IAsyncResult
    {
        private readonly HttpContext httpContext;
        private readonly AsyncCallback callback;
        private readonly object asyncState;

        public CometAsyncResult(HttpContext httpContext, AsyncCallback callback, object asyncState)
        {
            this.httpContext = httpContext;
            this.callback = callback;
            this.asyncState = asyncState;
        }

        public bool IsCompleted { get; private set; }

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

        public HttpContext HttpContext
        {
            get { return this.httpContext; }
        }

        public Message[] ResponseMessages { get; set; }

        public void Complete()
        {
            this.IsCompleted = true;
            this.callback(this);
        }
    }
}