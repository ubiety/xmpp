// Namespaces.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2017 Dieter Lunn
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
    ///     Common XML namespaces used by XMPP
    /// </summary>
    public static class Namespaces
    {
        /// <summary>
        ///     Standard XMLNS namespace - http://www.w3.org/2000/xmlns
        /// </summary>
        public const string Xmlns = "http://www.w3.org/2000/xmlns";

        /// <summary>
        ///     Standard XML namespace - http://www.w3.org/XML/1998/namespace
        /// </summary>
        public const string Xml = "http://www.w3.org/XML/1998/namespace";

        /// <summary>
        ///     Default client namespace - jabber:client
        /// </summary>
        public const string Client = "jabber:client";

        /// <summary>
        ///     Stream Namespace - http://etherx.jabber.org/streams
        /// </summary>
        public const string Stream = "http://etherx.jabber.org/streams";

        /// <summary>
        ///     XMPP streams namespace - urn:ietf:params:xml:ns:xmpp-streams
        /// </summary>
        public const string XmppStreams = "urn:ietf:params:xml:ns:xmpp-streams";

        /// <summary>
        ///     Start TLS namespace
        /// </summary>
        public const string StartTls = "urn:ietf:params:xml:ns:xmpp-tls";

        /// <summary>
        ///     SASL namespace
        /// </summary>
        public const string Sasl = "urn:ietf:params:xml:ns:xmpp-sasl";

        /// <summary>
        ///     Authentication namespace
        /// </summary>
        public const string Auth = "http://jabber.org/features/iq-auth";

        /// <summary>
        ///     Account registration namespace
        /// </summary>
        public const string Register = "http://jabber.org/features/iq-register";

        /// <summary>
        ///     Resource binding namespace
        /// </summary>
        public const string Bind = "urn:ietf:params:xml:ns:xmpp-bind";

        /// <summary>
        ///     Session namespace
        /// </summary>
        public const string Session = "urn:ietf:params:xml:ns:xmpp-session";

        /// <summary>
        ///     Compression namespace
        /// </summary>
        public const string Compression = "http://jabber.org/features/compress";

        /// <summary>
        ///     Compression protocol namespace
        /// </summary>
        public const string CompressionProtocol = "http://jabber.org/protocol/compress";

        /// <summary>
        ///     Stanza namespace
        /// </summary>
        public const string Stanzas = "urn:ietf:params:xml:ns:xmpp-stanzas";

        /// <summary>
        ///     Roster version namespace
        /// </summary>
        public const string Rostver = "urn:xmpp:features:rosterver";

        /// <summary>
        ///     Namespace for Entity Capabilities XEP-0115
        /// </summary>
        public const string Entity = "http://jabber.org/protocol/caps";

        /// <summary>
        ///     Service discovery namespace
        /// </summary>
        public const string DiscoInfo = "http://jabber.org/protocol/disco#info";

        /// <summary>
        ///     Ping namespace
        /// </summary>
        public const string Ping = "urn:xmpp:ping";

        /// <summary>
        ///     Roster namespace
        /// </summary>
        public const string Roster = "jabber:iq:roster";

        /// <summary>
        /// Stream Management Version 2 namespace
        /// </summary>
        public const string StreamManagementV2 = "urn:xmpp:sm:2";

        /// <summary>
        /// Stream Management Version 3 namespace
        /// </summary>
        public const string StreamManagementV3 = "urn:xmpp:sm:3";

        /// <summary>
        /// Client State Indication namespace
        /// </summary>
        public const string ClientState = "urn:xmpp:csi:0";
    }
}