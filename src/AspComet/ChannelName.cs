using System;

namespace AspComet
{
    public class ChannelName
    {
        private readonly string[] segments;

        private ChannelName(string name)
        {
            this.segments = name.Split('/');
        }

        public static ChannelName From(string name)
        {
            if (!name.StartsWith("/"))
            {
                throw new ArgumentException("Name must start with /");
            }

            if (name.Contains("*"))
            {
                throw new ArgumentException("Name cannot contain wildcards");
            }

            return new ChannelName(name);
        }

        public bool Matches(string toMatch)
        {
            string[] matchSegments = toMatch.Split('/');
            EnsureAnyWildcardIsAtEndOf(matchSegments);

            if (matchSegments.Length > this.segments.Length)
            {
                return false;
            }

            var matchEnumerator = matchSegments.GetEnumerator();
            var thisEnumerator = this.segments.GetEnumerator();
            while (matchEnumerator.MoveNext() && thisEnumerator.MoveNext())
            {
                string match = (string) matchEnumerator.Current;
                string segment = (string) thisEnumerator.Current;

                if (match == "*" && !thisEnumerator.MoveNext())
                {
                    return true;
                }

                if (match == "**")
                {
                    return true;
                }

                if (match != segment)
                {
                    return false;
                }
            }

            return true;
        }

        private static void EnsureAnyWildcardIsAtEndOf(string[] segments)
        {
            int wildCardIndex = Array.IndexOf(segments, "*");

            if (wildCardIndex < 0)
            {
                wildCardIndex = Array.IndexOf(segments, "**");
            }

            if (wildCardIndex >= 0 && wildCardIndex != segments.Length - 1)
            {
                throw new ArgumentException("Wildcard must be the last segment");
            }
        }
    }
}
