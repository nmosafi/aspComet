using AspComet.Eventing;

namespace AspComet.MessageHandlers
{
    public class MetaDisconnectHandler : IMessageHandler
    {
        private readonly IClientRepository clientRepository;

        public MetaDisconnectHandler(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            IClient client = clientRepository.GetByID(request.clientId);

            if (client != null)
            {
                client.Disconnect();
            }

            return new MessageHandlerResult
            {
                Message = new Message
                {
                    id = request.id,
                    channel = request.channel,
                    successful = true,
                    clientId = request.clientId
                },
                CanTreatAsLongPoll = false
            };
        }
    }
}