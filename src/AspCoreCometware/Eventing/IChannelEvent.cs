using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet.Eventing
{
    public interface IChannelEvent : IEvent
    {
        string Channel { get; }
    }
}
