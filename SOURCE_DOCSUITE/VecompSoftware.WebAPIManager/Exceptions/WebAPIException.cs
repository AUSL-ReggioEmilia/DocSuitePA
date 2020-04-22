using System;
using System.Runtime.Serialization;

namespace VecompSoftware.WebAPIManager.Exceptions
{
    [Serializable]
    public class WebAPIException<T> : Exception
    {
        public T Results { get; set; }

        public string TenantName { get; set; }

        /// <summary>
        /// Creates a new <see cref="WebAPIException"/> object.
        /// </summary>
        public WebAPIException() { }

        /// <summary>
        /// Creates a new <see cref="WebAPIException"/> object.
        /// </summary>
        public WebAPIException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="WebAPIException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public WebAPIException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="WebAPIException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public WebAPIException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
