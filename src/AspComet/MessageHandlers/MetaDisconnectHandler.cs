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

        public string ChannelName
        {
            get { return "/meta/disconnect"; }
        }

        public MessageHandlerResult HandleMessage(Message request)
        {
            IClient client = clientRepository.GetByID(request.clientId);
            
            client.Disconnect();

            return new MessageHandlerResult
            {
                Message = new Message
                {
                    id = request.id,
                    channel = this.ChannelName,
                    successful = true,
                    clientId = request.clientId
                },
                CanTreatAsLongPoll = false
            };
        }
    }
}