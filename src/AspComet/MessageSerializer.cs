using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace AspComet
{
    public class MessageSerializer: IMessageSerializer
    {
        private readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        private static readonly Regex NullRegex = new Regex(@"(""[^""]+"":null,)|(,""[^""]+"":null)", RegexOptions.Compiled);

        public T Deserialize<T>(string input)
        {
            return Serializer.Deserialize<T>(input);
        }

        public string Serialize(object model)
        {
            string txt = Serializer.Serialize(model);
            return NullRegex.Replace(txt, string.Empty);
        }
    }
}
