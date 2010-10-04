using System;
using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class LongPollingTransport : ITransport
    {
        public static LongPollingTransport Instance = new LongPollingTransport();

        public void SendMessages(HttpResponseBase response, HttpRequestBase request, IEnumerable<Message> messages)
        {
          if ( !String.IsNullOrEmpty( CometHttpHandler.AllowOrigin ) && request.Headers["Origin"]!=null ) {
            if ( CometHttpHandler.IsOriginAllowed( new Uri( request.Headers["Origin"] ) ) ) {
              response.AddHeader( "Access-Control-Allow-Origin", request.Headers["Origin"] );
              response.AddHeader( "Access-Control-Allow-Methods", "POST, GET, OPTIONS" );
              response.AddHeader( "Access-Control-Max-Age", ( 24 * 60 * 60 ).ToString() );
              if ( request.Headers["Access-Control-Request-Headers"] != null ) {
                response.AddHeader( "Access-Control-Allow-Headers", request.Headers["Access-Control-Request-Headers"] );
              }
            }
          }

          response.ContentType = "text/json";
            response.Write(MessageConverter.ToJson(messages));
        }
    }
}