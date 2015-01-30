// ErrorType.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2015 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

namespace Ubiety.Common
{
    /// <summary>
    ///     Describes the type of error being sent
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        ///     Id is missing from the settings class
        /// </summary>
        MissingId,

        /// <summary>
        ///     Password is missing from the settings class
        /// </summary>
        MissingPassword,

        /// <summary>
        ///     Failed to authorize the user based on provided credentials
        /// </summary>
        AuthorizationFailed,

        /// <summary>
        ///     Server and client do no support the same protocol features
        /// </summary>
        WrongProtocolVersion,

        /// <summary>
        ///     Tag sent by the server is not available in client
        /// </summary>
        UnregisteredItem,

        /// <summary>
        ///     Failed to implement compression of stream between client and server
        /// </summary>
        CompressionFailed,

        /// <summary>
        ///     XML from the server is malformed or unexpected
        /// </summary>
        XmlError,

        /// <summary>
        ///     Connection took too long to connect
        /// </summary>
        ConnectionTimeout
    }

    /// <summary>
    ///     Describes the severity of an error so the appropriate action can be taken.
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>
        ///     Error is mostly informative
        /// </summary>
        Information,

        /// <summary>
        ///     Error is bad enough to disconnect from the server
        /// </summary>
        Disconnect,

        /// <summary>
        ///     Error caused an automatic reconnect attempt
        /// </summary>
        Reconnect,

        /// <summary>
        ///     Error caused cannot allow connection to continue
        /// </summary>
        Fatal
    }
}