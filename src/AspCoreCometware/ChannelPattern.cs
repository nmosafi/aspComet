using System;

namespace AspComet
{
    public class ChannelPattern
    {
        private readonly string prefix;
        private readonly byte starCount;
        private readonly int hashCode;

        public ChannelPattern(string pattern)
        {
            if (!pattern.StartsWith("/"))
            {
                throw new ArgumentException("Name must start with /");
            }
            if (pattern.Length > 1 && pattern.EndsWith("/"))
            {
                throw new ArgumentException("Name must end with segment");
            }

            hashCode = pattern.GetHashCode();
            if (pattern.EndsWith("/**"))
            {
                starCount = 2;
                prefix = pattern.Substring(0, pattern.Length - 3);
            }
            else if (pattern.EndsWith("/*"))
            {
                starCount = 1;
                prefix = pattern.Substring(0, pattern.Length - 2);
            }
            else
            {
                starCount = 0;
                prefix = pattern;
            }

            if (prefix.IndexOf("/*") >= 0)
            {
                throw new ArgumentException("Wildcard must be the last segment");
            }
        }

        public bool Matches(string channelName)
        {
            switch (starCount)
            {
                default:
                case 0:
                    return prefix == channelName;
                case 1:
                    return channelName.StartsWith(prefix) && channelName.Length > prefix.Length + 1 && channelName[prefix.Length] == '/' && channelName.IndexOf('/', prefix.Length + 1) == -1;
                case 2:
                    return channelName.StartsWith(prefix) && channelName.Length > prefix.Length + 1 && channelName[prefix.Length] == '/';
            }
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
