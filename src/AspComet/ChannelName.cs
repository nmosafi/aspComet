using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet
{
    public class ChannelName
    {
        private readonly string name;

        private ChannelName(string name)
        {
            this.name = name;
        }

        public static ChannelName From(string name)
        {
            if (!name.StartsWith("/"))
            {
                throw new ArgumentException("Name must start with /");
            }

            return new ChannelName(name);
        }

        public bool Matches(string channel)
        {
            return false;
        }
    }
}
