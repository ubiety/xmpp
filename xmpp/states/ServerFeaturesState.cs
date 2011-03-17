// ServerFeaturesState.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn
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
using System.Xml;
using ubiety.core;
using ubiety.core.compression;
using ubiety.registries;
using ubiety.common.SASL;
using ubiety.logging;
using ubiety.common;
using ubiety;

namespace ubiety.states
{
	/// <summary>
	/// The server features state occurs just after connecting.
	/// </summary>
	public class ServerFeaturesState : State
	{
		/// <summary>
		/// Create the new state.
		/// </summary>
		public ServerFeaturesState() : base()
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Object"/>
		/// </param>
		public override void Execute (Tag data)
		{
			Features f = null;
			
			if (data is Stream)
			{
				Stream s = data as Stream;
				if (!s.Version.StartsWith("1."))
				{
					Errors.Instance.SendError(this, ErrorType.WrongProtocolVersion, "Expecting stream:features from 1.x server");
					return;
				}
				f = s.Features;
			}
			else
			{
				f = data as Features;
			}
			
			if (f.StartTLS != null && Settings.SSL)
			{
				_current.State = new StartTLSState();
				StartTLS tls = (StartTLS)_reg.GetTag("starttls", Namespaces.START_TLS, _current.Document);
				_current.Socket.Write(tls);
				return;
			}
			
			if (!_current.Authenticated)
			{
				Logger.Debug(this, "Creating SASL Processor");
				_current.Processor = SASLProcessor.CreateProcessor(f.StartSASL.SupportedTypes);
                if (_current.Processor == null)
                {
                    Logger.Debug(this, "No allowed type available. Allow more authentication options.");
                    _current.State = new DisconnectState();
                    _current.Execute();
                    return;
                }
		        Logger.Debug(this, "Sending auth with mechanism type");
				_current.Socket.Write(_current.Processor.Initialize());
			
				_current.State = new SASLState();
				return;
			}

            // Takes place after authentication according to XEP-0170
            if (!_current.Compress && CompressionRegistry.AlgorithmsAvailable && !Settings.SSL)
            {
                Logger.Info(this, "Starting compression");
                // Do we have a stream for any of the compressions supported by the server?
                foreach (string algorithm in f.Compression.Algorithms)
                {
                    if (CompressionRegistry.SupportsAlgorithm(algorithm))
                    {
                        Logger.DebugFormat(this, "Using {0} for compression", algorithm);
                        Tag c = _reg.GetTag("compress", Namespaces.COMPRESSION_PROTOCOL, _current.Document);
                        Tag m = _reg.GetTag("method", Namespaces.COMPRESSION_PROTOCOL, _current.Document);

                        m.InnerText = _current.Algorithm = algorithm;
                        c.AddChildTag(m);
                        _current.Socket.Write(c);
                        _current.State = new CompressedState();
                        return;
                    }
                }
            }

            Logger.Debug(this, "Authenticated");
            _current.State = new BindingState();
            _current.Execute(null);
		}
	}
}
