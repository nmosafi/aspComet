using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet
{
    public class ChannelPattern
    {
        private readonly string prefix;
        private readonly byte starCount;
        private readonly int hash;

        public ChannelPattern( string name )
        {
#if DEBUG
          if ( !name.StartsWith( "/" ) ) {
              throw new ArgumentException( "Name must start with /" );
          }
          if ( name.Length > 1 && name.EndsWith( "/" ) ) {
              throw new ArgumentException( "Name must end with segment" );
          }
#endif
          hash = name.GetHashCode();
          if ( name.EndsWith("/**") ) {
              starCount = 2;
              prefix = name.Substring( 0, name.Length-3 );
          } else if ( name.EndsWith( "/*" ) ) {
              starCount = 1;
              prefix = name.Substring( 0, name.Length - 2 );
          } else {
              starCount = 0;
              prefix = name;
          }
#if DEBUG
          if ( prefix.IndexOf("/*")>=0 ) {
              throw new ArgumentException( "Wildcard must be the last segment" );
          }
#endif
        }

        public bool Matches( string channelName )
        {
          switch ( starCount ) {
              default:
              case 0:
                  return prefix == channelName;
              case 1:
                  return channelName.StartsWith( prefix ) && channelName.Length > prefix.Length + 1 && channelName[prefix.Length] == '/' && channelName.IndexOf( '/', prefix.Length + 1 ) == -1;
              case 2:
                  return channelName.StartsWith( prefix ) && channelName.Length > prefix.Length + 1 && channelName[prefix.Length] == '/';
          }
        }

        public override int GetHashCode()
        {
            return hash;
        }

    }
}
