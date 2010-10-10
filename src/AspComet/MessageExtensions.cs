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
            Dictionary<string, object> data = (Dictionary<string, object>) message.data;
            if (data == null || !data.ContainsKey(key)) return default(T);
            return (T) data[key];
        }

        /// <summary>
        ///     Sets the specified data item in the message
        /// </summary>
        /// <param name="message">The message to set the data on</param>
        /// <param name="key">The data key</param>
        /// <param name="value">The data item</param>
        public static void SetData(this Message message, string key, object value)
        {
            Dictionary<string, object> data = (Dictionary<string, object>) message.data;
            if (data == null)
            {
                data = new Dictionary<string, object>();
                message.data = data;
            }

            data[key] = value;
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
            Dictionary<string, object> advice = (Dictionary<string, object>) message.advice;
            if (advice == null || !advice.ContainsKey(key)) return default(T);
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

        /// <summary>
        ///     Gets a specific ext item from the message
        /// </summary>
        /// <typeparam name="T">The type of ext value to retrieve</typeparam>
        /// <param name="message">The message to retrieve the ext value from</param>
        /// <param name="key">The key of the ext value</param>
        /// <returns>The value for the advice</returns>
        public static T GetExt<T>(this Message message, string key)
        {
            Dictionary<string, object> ext = (Dictionary<string, object>) message.ext;
            if (ext == null || !ext.ContainsKey(key)) return default(T);
            return (T) ext[key];
        }

        /// <summary>
        ///     Sets the specified ext item in the message
        /// </summary>
        /// <param name="message">The message to set the ext on</param>
        /// <param name="key">The ext key</param>
        /// <param name="value">The ext value</param>
        public static void SetExt(this Message message, string key, object value)
        {
            Dictionary<string, object> ext = (Dictionary<string, object>) message.ext;
            if (ext == null)
            {
                ext = new Dictionary<string, object>();
                message.ext = ext;
            }

            ext[key] = value;
        }
    }
}