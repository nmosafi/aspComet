using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public void SendWhisper( PublishingEvent ev )
        {
            if ( ev.Cancel ) return;

            AuthenticatedClient sender = (AuthenticatedClient)clientRepository.GetByID( ev.Message.clientId );
            string message = ev.Message.GetData<string>( "message" );
            int spacePos = message.IndexOf( ' ' );
            if ( spacePos == -1 ) {
                ev.Cancel = true;
                ev.CancellationReason = "Format error";
                SendInfoToUser( sender, "Usage: /whisper <nickname> message" );
                return;
            }
            string nickname = message.Substring( 0, spacePos );
            var receiver = ( from AuthenticatedClient user in clientRepository.WhereSubscribedTo( "/chat" )
                             where String.Equals( user.username, nickname, StringComparison.InvariantCultureIgnoreCase )
                             select user ).FirstOrDefault();
            if ( receiver == null ) {
                SendInfoToUser( sender, "User " + nickname + " is not connected to the chat" );
                return;
            }
            string text = message.Substring( spacePos+1 );
            SendInfoToUser( sender, "To " + nickname + ": " + text );
            SendInfoToUser( receiver, sender.username + " whispers: " + text );
        }

        public void SendInfoToUser( AuthenticatedClient i_user, string i_text )
        {
            Message message = new Message {
                channel = "/chat",
                clientId = i_user.ID
            };
            message.SetData( "message", i_text );
            i_user.Enqueue( message );
            i_user.FlushQueue();
        }

    }
}
