using System;

namespace Ubiety.Common.Exceptions
{
    /// <summary>
    ///     ServerXmlException is thrown when there is malformed or invalid Xml received from the server.
    /// </summary>
    public class ServerXmlException : Exception
    {
        /// <summary>
        ///     Instantiates a new instance of the ServerXmlException
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="e">Internal exception</param>
        public ServerXmlException(string message, Exception e) : base(message, e) { }
    }
}
