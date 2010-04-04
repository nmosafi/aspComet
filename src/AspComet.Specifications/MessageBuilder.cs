using System;

namespace AspComet.Specifications
{
    public static class MessageBuilder
    {
        private static readonly Random Random = new Random();

        public static Message BuildWithRandomPropertyValues()
        {
            return new Message
            {
                channel = RandomString(),
                clientId = RandomString(),
                connectionType = RandomString(),
                error = RandomString(),
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