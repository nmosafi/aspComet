using System.Collections.Generic;
using System.Web;

namespace AspComet.Transports
{
    public class CallbackPollingTransport : ITransport
    {
      public static CallbackPollingTransport Instance = new CallbackPollingTransport();

        public void SendMessages(HttpResponseBase response, HttpRequestBase request, IEnumerable<Message> messages)
        {
            string messageAsJson = MessageConverter.ToJson(messages);
            string functionName = request.Params["jsonp"] ?? "jsonpcallback";
            string functionCall = string.Format("{0} ( {1} )", functionName, messageAsJson);

            response.ContentType = "text/javascript";
            response.Write(functionCall);
        }
    }
}