using System.Collections.Generic;
using System.Linq;

using AspComet;
using AspComet.Eventing;

namespace AspCometCoreApp
{
    /// <summary>
    ///     Blocks bad language from being sent to a channel, demonstrating use of the publishing
    ///     event and shows how to manually send messages to clients or channels
    /// </summary>
    public class BadLanguageBlocker 
    {
        private readonly IClientRepository clientRepository;

        public BadLanguageBlocker(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public void CheckMessage(PublishingEvent ev)
        {
            if (!ev.Message.GetData<string>("message").Contains("pish")) return;

            IClient sender = this.clientRepository.GetByID(ev.Message.clientId);

            ev.Cancel = true;
        	ev.CancellationReason = "Bad Language";

            SendBadLanguageWarningToSender(ev.Message, sender);
            InformChannelOfBadLanguage(ev.Message, sender);
        }

        private void InformChannelOfBadLanguage(Message incomingMessage, IClient sender)
        {
            string warning = "Beware; " + incomingMessage.GetData<string>("sender") + " has received a warning about his language";
            Message message = new Message
            {
                channel = incomingMessage.channel,
            };

            message.SetData("message", warning);

            foreach (Client subscriber in this.clientRepository.WhereSubscribedTo(incomingMessage.channel).Where(c => c.ID != sender.ID))
            {
                subscriber.Enqueue(message);
                subscriber.FlushQueue();
            }
        }

        private static void SendBadLanguageWarningToSender(Message incomingMessage, IClient sender)
        {
            Message message = new Message 
            {
                channel = incomingMessage.channel,
            };

            string senderName = incomingMessage.GetData<string>("sender");

            message.SetData("message", string.Format("Please don't use such fowl language, {0}.", senderName));

            sender.Enqueue(message);
            sender.FlushQueue();
        }
    }
}