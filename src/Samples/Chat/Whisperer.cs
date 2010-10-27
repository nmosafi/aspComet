using System.Linq;
using AspComet.Eventing;

namespace AspComet.Samples.Chat
{
    public class Whisperer
    {
        private readonly IClientRepository clientRepository;

        public Whisperer(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public void SendWhisper(PublishingEvent ev)
        {
            if (ev.Cancel) return;

            AuthenticatedClient sender = (AuthenticatedClient) clientRepository.GetByID(ev.Message.clientId);
            Whisper whisper = Whisper.FromMessage(ev.Message);

            if (whisper == null)
            {
                ev.Cancel = true;
                ev.CancellationReason = "Format error";
                SendInfoToClient(sender, "Usage: /whisper <nickname> message");
                return;                
            }

            AuthenticatedClient receiver = 
                clientRepository.WhereSubscribedTo("/chat")
                                .Cast<AuthenticatedClient>()
                                .FirstOrDefault(user => user.HasUsername(whisper.Username));
            if (receiver == null)
            {
                SendInfoToClient(sender, "User " + whisper.Username + " is not connected to the chat");
                return;
            }
            SendInfoToClient(sender, "To " + whisper.Username + ": " + whisper.Message);
            SendInfoToClient(receiver, sender.Username + " whispers: " + whisper.Message);
        }

        private static void SendInfoToClient(IClient client, string info)
        {
            Message message = new Message
            {
                channel = "/chat",
                clientId = client.ID
            };

            message.SetData("message", info);
            client.Enqueue(message);
            client.FlushQueue();
        }

        private class Whisper
        {
            public string Username { get; private set; }
            public string Message { get; private set; }

            public static Whisper FromMessage(Message message)
            {
                string whisperContent = message.GetData<string>("message");

                int spacePosition = whisperContent.IndexOf(' ');
                if (spacePosition == -1)
                {
                    return null;
                }

                return new Whisper
                {
                    Username = whisperContent.Substring(0, spacePosition),
                    Message = whisperContent.Substring(spacePosition + 1)
                };
            }
        }
    }
}
