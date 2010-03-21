using System;

namespace AspComet.Specifications.MessageHandlers
{
    public static class MessageBuilder
    {
        private static readonly Random Random = new Random();

        public static Message BuildRandomRequest()
        {
            return new Message
                       {
                           advice = new object(),
                           channel = RandomString(),
                           clientId = RandomString(),
                           connectionType = RandomString(),
                           error = RandomString(),
                           ext = new object(),
                           id = RandomString(),
                           minimumVersion = RandomString(),
                           subscription = RandomString()
                       };
        }

        private static string RandomString()
        {
            return Random.Next().ToString();
        }
    }
}