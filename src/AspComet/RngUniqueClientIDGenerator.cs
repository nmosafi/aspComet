using System;
using System.Security.Cryptography;

namespace AspComet
{
    public class RngUniqueClientIDGenerator : IClientIDGenerator
    {
        private readonly RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private readonly IClientRepository clientRepository;

        public RngUniqueClientIDGenerator(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public string GenerateClientID()
        {
            string clientID;
            do
            {
                byte[] bytes = new byte[15];
                this.rngCryptoServiceProvider.GetBytes(bytes);
                clientID = Convert.ToBase64String(bytes);
            }
            while (this.clientRepository.ContainsID(clientID));
            return clientID;
        }

    }
}