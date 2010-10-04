using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;

using Microsoft.Practices.ServiceLocation;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        public static int LongPollDurationInMilliseconds
        {
            get { return s_longPollDuration; }
            set { s_longPollDuration = value; }
        }

        public static int ClientTimeoutInMilliseconds
        {
            get { return s_clientTimeout; }
            set { s_clientTimeout = value; }
        }

        static int s_longPollDuration = 10000;
        static int s_clientTimeout = 20000;
        static string s_allowOrigin = null;
        static Dictionary<string, bool> s_allowOriginUris = new Dictionary<string, bool>();
        static ReaderWriterLockSlim s_allowOriginLock = new ReaderWriterLockSlim();

        public static string AllowOrigin
        {
            get { return s_allowOrigin; }
            set
            {
                s_allowOriginLock.EnterWriteLock();
                try {
                    s_allowOriginUris.Clear();
                    s_allowOrigin = value;
                    if ( !String.IsNullOrEmpty( value ) && value != "*" ) {
                        string[] parts = value.Split( new char[] { ',' } );
                        foreach ( string part in parts ) {
                            Uri normalized = new Uri( part );
                            s_allowOriginUris.Add( normalized.Scheme + "//" + normalized.Host + ":" + normalized.Port, true );
                        }
                    } else if ( value == "" ) {
                      value = null;
                    }
                }
                finally {
                    s_allowOriginLock.ExitWriteLock();
                }
            }
        }

        public static bool IsOriginAllowed( Uri i_origin )
        {
            if ( s_allowOrigin==null ) return false;
            if ( s_allowOrigin=="*" ) return true;
            if ( i_origin == null ) return false;
            s_allowOriginLock.EnterReadLock();
            try {
                return s_allowOriginUris.ContainsKey( i_origin.Scheme + "//" + i_origin.Host + ":" + i_origin.Port );
            }
            finally {
                s_allowOriginLock.ExitReadLock();
            }
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
            return BeginProcessRequest(new HttpContextWrapper(context), callback, asyncState);
        }

        public IAsyncResult BeginProcessRequest(HttpContextBase context, AsyncCallback callback, object asyncState)
        {
            if ( context.Request.HttpMethod == "OPTIONS" ) {
              CometAsyncResult result = new CometAsyncResult( context, callback, asyncState );
              result.CompleteRequestWithMessages( null );
              return result;
            }
            if ( context.Request.HttpMethod == "GET" && !IsOriginAllowed( context.Request.UrlReferrer ) ) {
              context.Response.StatusCode = 403;
              CometAsyncResult result = new CometAsyncResult( context, callback, asyncState );
              result.CompleteRequestWithMessages( null );
              return result;
            }
            Message[] request = MessageConverter.FromJson(context.Request);
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

        public void EndProcessRequest(IAsyncResult result)
        {
            ICometAsyncResult cometAsyncResult = (ICometAsyncResult) result;
            cometAsyncResult.SendAwaitingMessages();
        }
    }
}
