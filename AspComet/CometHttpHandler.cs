using System;
using System.Runtime.Serialization.Json;
using System.Web;

namespace AspComet
{
    public class CometHttpHandler : IHttpAsyncHandler
    {
        private static Action action;
        private static readonly MessageBroker messageBroker = new MessageBroker(new InMemoryClientRepository());

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new Exception("Cannot process synchronous requests");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Message request = MessageConverter.FromJson(context.Request, "message");

            action = () =>
            {
                Message[] response = messageBroker.HandleMessage(request);

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message[]));
                serializer.WriteObject(context.Response.OutputStream, response);
            };

            return action.BeginInvoke(cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            action.EndInvoke(result);
        }
    }
}
