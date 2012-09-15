// ServerFeaturesState.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2012 Dieter Lunn
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
using ubiety.common;
using ubiety.common.logging;
using ubiety.common.SASL;
using ubiety.core;
using ubiety.registries;

namespace ubiety.states
{
	/// <summary>
	/// The server features state occurs just after connecting.
	/// </summary>
	public class ServerFeaturesState : State
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">
		/// A <see cref="Tag"/>
		/// </param>
		public override void Execute(Tag data = null)
		{
			Features f;

			if (data is Stream)
			{
				var s = data as Stream;
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

			if (f != null)
			{
				if (f.StartTLS != null && UbietySettings.SSL)
				{
					Current.State = new StartTLSState();
					var tls = (StartTLS) Reg.GetTag("starttls", Namespaces.StartTLS, Current.Document);
					Current.Socket.Write(tls);
					return;
				}

				if (!Current.Authenticated)
				{
					Logger.Debug(this, "Creating SASL Processor");
					Current.Processor = SASLProcessor.CreateProcessor(f.StartSASL.SupportedTypes);
					if (Current.Processor == null)
					{
						Logger.Debug(this, "No allowed type available. Allow more authentication options.");
						Current.State = new DisconnectState();
						Current.State.Execute();
						return;
					}
					Logger.Debug(this, "Sending auth with mechanism type");
					Current.Socket.Write(Current.Processor.Initialize());

					Current.State = new SASLState();
					return;
				}

				// Takes place after authentication according to XEP-0170
				if (!Current.Compressed && CompressionRegistry.AlgorithmsAvailable && !UbietySettings.SSL && f.Compression != null)
				{
					Logger.Info(this, "Starting compression");
					// Do we have a stream for any of the compressions supported by the server?
					foreach (var algorithm in
						f.Compression.Algorithms.Where(CompressionRegistry.SupportsAlgorithm))
					{
						Logger.DebugFormat(this, "Using {0} for compression", algorithm);
						var c = Reg.GetTag("compress", Namespaces.CompressionProtocol, Current.Document);
						var m = Reg.GetTag("method", Namespaces.CompressionProtocol, Current.Document);

						m.InnerText = Current.Algorithm = algorithm;
						c.AddChildTag(m);
						Current.Socket.Write(c);
						Current.State = new CompressedState();
						return;
					}
				}
			}

			Logger.Debug(this, "Authenticated");
			Current.State = new BindingState();
			Current.State.Execute();
		}
	}
}