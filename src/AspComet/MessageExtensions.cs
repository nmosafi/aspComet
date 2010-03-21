using System.Collections.Generic;

namespace AspComet
{
    /// <summary>
    ///     Extends the <see cref="Message"/> class with some useful extensions
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        ///     Gets a specific data item from the message
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve</typeparam>
        /// <param name="message">The message to retrieve the data from</param>
        /// <param name="key">The key within the data</param>
        /// <returns>The data item</returns>
        public static T GetData<T>(this Message message, string key)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>) message.data;
            return (T) dictionary[key];
        }

        /// <summary>
        ///     Gets a specific advice item from the message
        /// </summary>
        /// <typeparam name="T">The type of advice value to retrieve</typeparam>
        /// <param name="message">The message to retrieve the advice from</param>
        /// <param name="key">The key of the advice</param>
        /// <returns>The value for the advice</returns>
        public static T GetAdvice<T>(this Message message, string key)
        {
            Dictionary<string, object> advice = (Dictionary<string, object>)message.advice;
            return (T) advice[key];
        }

        /// <summary>
        ///     Sets the specified advice item in the message
        /// </summary>
        /// <param name="message">The message to set the advice on</param>
        /// <param name="key">The advice key</param>
        /// <param name="value">The advice value</param>
        public static void SetAdvice(this Message message, string key, object value)
        {
            Dictionary<string, object> advice = (Dictionary<string, object>) message.advice;
            if (advice == null)
            {
                advice = new Dictionary<string, object>();
                message.advice = advice;
            }

            advice[key] = value;
        }
    }
}