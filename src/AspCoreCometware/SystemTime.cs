using System;

namespace AspComet
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = ()=> DateTime.Now;
    }
}