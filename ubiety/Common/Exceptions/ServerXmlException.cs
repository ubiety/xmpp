using System;

namespace Ubiety.Common.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerXmlException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public ServerXmlException(string message, Exception e) : base(message, e) { }
    }
}
