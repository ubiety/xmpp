// ServerFeaturesState.cs
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

using System.Linq;
using Serilog;
using Ubiety.Common;
using Ubiety.Common.Sasl;
using Ubiety.Core;
using Ubiety.Registries;

namespace Ubiety.States
{
    /// <summary>
    ///     The server features state occurs just after connecting.
    /// </summary>
    public class ServerFeaturesState : IState
    {
        /// <summary>
        /// </summary>
        /// <param name="data">
        ///     A <see cref="Tag" />
        /// </param>
        /// <param name="state"></param>
        public void Execute(XmppState state, Tag data = null)
        {
            Features features;

            var stream = data as Stream;
            if (stream != null)
            {
                var s = stream;
                if (!s.Version.StartsWith("1."))
                {
                    state.Events.OnError(this, ErrorType.WrongProtocolVersion, ErrorSeverity.Fatal,
                        "Didn't receive expected stream:features tag from 1.x compliant server.");
                    return;
                }
                features = s.Features;

                state.Events.OnConnect(this);
            }
            else
            {
                features = data as Features;
            }

            if (features == null) return;
            // We have features available so make sure we have them set
            features.Update();

            // Should we use SSL and is it required
            if (state.Features.HasFlag(ProtocolFeatures.StartTls) && state.Settings.Ssl ||
                state.Features.HasFlag(ProtocolFeatures.StartTls) &&
                state.Features.HasFlag(ProtocolFeatures.SslRequired) && !ProtocolState.Encrypted)
            {
                Log.Debug("Starting SSL...");
                state.State = new StartTlsState();
                var tls = TagRegistry.GetTag<StartTls>("starttls", Namespaces.StartTls);
                state.Socket.Write(tls);
                return;
            }

            if (!ProtocolState.Authenticated)
            {
                Log.Debug("Starting Authentication...");
                ProtocolState.Processor = SaslProcessor.CreateProcessor(features.StartSasl.SupportedTypes,
                    ProtocolState.Settings.AuthenticationTypes);
                if (ProtocolState.Processor == null)
                {
                    ProtocolState.State = new DisconnectState();
                    ProtocolState.State.Execute();
                    return;
                }
                ProtocolState.Socket.Write(
                    ProtocolState.Processor.Initialize(ProtocolState.Settings.Id, ProtocolState.Settings.Password));

                ProtocolState.State = new SaslState();
                return;
            }

            // Takes place after authentication according to XEP-0170
            if (!ProtocolState.Compressed && CompressionRegistry.AlgorithmsAvailable &&
                !ProtocolState.Settings.Ssl && features.Compression != null)
            {
                Log.Debug("Starting Compression...");
                // Do we have a stream for any of the compressions supported by the server?
                foreach (var algorithm in
                    features.Compression.Algorithms.Where(CompressionRegistry.SupportsAlgorithm))
                {
                    var c = TagRegistry.GetTag<GenericTag>("compress", Namespaces.CompressionProtocol);
                    var m = TagRegistry.GetTag<GenericTag>("method", Namespaces.CompressionProtocol);

                    m.InnerText = ProtocolState.Algorithm = algorithm;
                    c.AddChildTag(m);
                    ProtocolState.Socket.Write(c);
                    ProtocolState.State = new CompressedState();
                    return;
                }
            }

            if (!ProtocolState.Authenticated) return;
            Log.Debug("Switching to Binding state");
            ProtocolState.State = new BindingState();
            ProtocolState.State.Execute();
        }
    }
}