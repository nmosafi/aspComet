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

        public bool ShouldWait
        {
            get { return false; }
        }

        public Message HandleMessage(Message request)
        {
            Client client = clientRepository.GetByID(request.clientId);
            var e = new DisconnectedEvent(client);
            EventHub.Publish(e);

            clientRepository.RemoveByID(client.ID);

            return new Message
                       {
                           id = request.id,
                           channel = this.ChannelName,
                           successful = true,
                           clientId = request.clientId
                       };
        }
    }
}