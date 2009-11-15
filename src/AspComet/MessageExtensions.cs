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
    }
}