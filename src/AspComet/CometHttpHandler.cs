using System;
using System.Configuration;
using System.Web;

using Microsoft.Practices.ServiceLocation;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        public const int LongPollDurationInMilliseconds = 10000;
        public const int ClientTimeoutInMilliseconds = 20000;

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
            return BeginProcessRequest(new HttpContextWrapper(context), callback, asyncState);
        }

        public IAsyncResult BeginProcessRequest(HttpContextBase context, AsyncCallback callback, object asyncState)
        {
            Message[] request = GetMessageConverter().FromJson(context.Request);
            CometAsyncResult asyncResult = new CometAsyncResult(context, callback, asyncState);
            GetMessageBus().HandleMessages(request, asyncResult);
            return asyncResult;
        }

        private static IMessageBus GetMessageBus()
        {
            IServiceLocator serviceLocator = ServiceLocator.Current;
            if (serviceLocator == null)
            {
                throw new ConfigurationErrorsException("AspComet has not been configured. Either use the default configuration or set up a service locator");
            }

            return serviceLocator.GetInstance<IMessageBus>();
        }

        private static IMessageConverter GetMessageConverter()
        {
            IServiceLocator serviceLocator = ServiceLocator.Current;
            if (serviceLocator == null)
            {
                throw new ConfigurationErrorsException("AspComet has not been configured. Either use the default configuration or set up a service locator");
            }

            return serviceLocator.GetInstance<IMessageConverter>();
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            ICometAsyncResult cometAsyncResult = (ICometAsyncResult) result;
            cometAsyncResult.SendAwaitingMessages();
        }
    }
}
