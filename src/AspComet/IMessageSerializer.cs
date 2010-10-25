using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspComet
{
    public interface IMessageSerializer
    {
        T Deserialize<T>(string input);
        string Serialize(object model);
    }
}
