using System;

namespace AspComet.Specifications
{
    public static class GetException
    {
        public static Exception From(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static TException From<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                return null;
            }
            catch (TException ex)
            {
                return ex;
            }
        }
    }
}