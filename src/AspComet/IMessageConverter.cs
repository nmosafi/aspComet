using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AspComet
{
    public interface IMessageConverter
    {
        Message[] FromJson(HttpRequestBase request);
        string ToJson<TModel>(TModel model);
    }
}
