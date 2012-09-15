// Namespaces.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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

namespace ubiety.common
{
	/// <summary>
	/// 
	/// </summary>
	public class Namespaces
	{
		///<summary>
		///</summary>
		public const string XMLNS = "http://www.w3.org/2000/xmlns";
		///<summary>
		///</summary>
		public const string XML = "http://www.w3.org/XML/1998/namespace";
		
		///<summary>
		///</summary>
		public const string Client = "jabber:client";
		/// <summary>
		/// Stream Namespace - http://etherx.jabber.org/streams
		/// </summary>
		public const string Stream = "http://etherx.jabber.org/streams";

		///<summary>
		///</summary>
		public const string XMPPStreams = "urn:ietf:params:xml:ns:xmpp-streams";
		/// <summary>
		/// 
		/// </summary>
		public const string StartTLS = "urn:ietf:params:xml:ns:xmpp-tls";
		/// <summary>
		/// 
		/// </summary>
		public const string SASL = "urn:ietf:params:xml:ns:xmpp-sasl";
		/// <summary>
		/// 
		/// </summary>
		public const string Auth = "http://jabber.org/features/iq-auth";
		/// <summary>
		/// 
		/// </summary>
		public const string Register = "http://jabber.org/features/iq-register";
		
		///<summary>
		///</summary>
		public const string Bind = "urn:ietf:params:xml:ns:xmpp-bind";
		///<summary>
		///</summary>
		public const string Session = "urn:ietf:params:xml:ns:xmpp-session";
		
		///<summary>
		///</summary>
		public const string Compression = "http://jabber.org/features/compress";
		///<summary>
		///</summary>
		public const string CompressionProtocol = "http://jabber.org/protocol/compress";

		///<summary>
		///</summary>
		public const string Stanzas = "urn:ietf:params:xml:ns:xmpp-stanzas";

		///<summary>
		///</summary>
		public const string Rostver = "urn:xmpp:features:rosterver";

		/// <summary>
		/// Namespace for Entity Capabilities XEP-0115
		/// </summary>
		public const string Entity = "http://jabber.org/protocol/caps";

		/// <summary>
		/// Service discovery namespace
		/// </summary>
		public const string DiscoInfo = "http://jabber.org/protocol/disco#info";

		/// <summary>
		/// 
		/// </summary>
		public const string Ping = "urn:xmpp:ping";

		public const string Roster = "jabber:iq:roster";
	}
}
