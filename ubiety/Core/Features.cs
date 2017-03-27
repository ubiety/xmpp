// Features.cs
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

using System.Xml;
using Serilog;
using Ubiety.Common;
using Ubiety.Infrastructure;
using Ubiety.Infrastructure.Attributes;
using Ubiety.States;

namespace Ubiety.Core
{
    /// <summary>
    /// </summary>
    [XmppTag("features", Namespaces.Stream, typeof (Features))]
    public class Features : Tag, IUpdateable
    {
        /// <summary>
        /// </summary>
        public Features()
            : base("stream", new XmlQualifiedName("features", Namespaces.Stream))
        {
        }

        /// <summary>
        /// </summary>
        public Mechanisms StartSasl => this["mechanisms", Namespaces.Sasl] as Mechanisms;

        /// <summary>
        /// </summary>
        public Compression Compression => this["compression", Namespaces.Compression] as Compression;

        public void Update()
        {
            var ssl = (StartTls) this["starttls", Namespaces.StartTls];
            if (ssl != null)
            {
                Log.Debug("Setting SSL: On");
                ProtocolState.Features = ProtocolState.Features | ProtocolFeatures.StartTls;

                if (ssl.Required)
                {
                    Log.Debug("Setting SSL Required: On");
                    ProtocolState.Features = ProtocolState.Features | ProtocolFeatures.SslRequired;
                }
            }

            if (this["sm", Namespaces.StreamManagementV3] != null)
            {
                Log.Debug("Setting Stream Management: On");
                ProtocolState.Features = ProtocolState.Features | ProtocolFeatures.StreamManagement;
            }

            if (this["csi", Namespaces.ClientState] != null)
            {
                Log.Debug("Setting Client State Indication: On");
                ProtocolState.Features = ProtocolState.Features | ProtocolFeatures.ClientState;
            }
        }
    }
}