// ProtocolFeatures.cs
//
//Ubiety XMPP Library Copyright (C) 2017 Dieter Lunn
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

using System;

namespace Ubiety.Common
{
    /// <summary>
    ///
    /// </summary>
    [Flags]
    public enum ProtocolFeatures
    {
        /// <summary>
        /// No protocol features available or enabled
        /// </summary>
        None,
        /// <summary>
        /// SSL available on the server
        /// </summary>
        StartTls,
        /// <summary>
        /// SSL is required to connect to the current server
        /// </summary>
        SslRequired,
        /// <summary>
        /// Server supports compressed streams
        /// </summary>
        Compression,
        /// <summary>
        /// Server supports Stream Management (XEP-0198)
        /// </summary>
        StreamManagement,
        /// <summary>
        /// Server supports Client State Indication (XEP-0352)
        /// </summary>
        ClientState
    }
}