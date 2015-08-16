// ProtocolState.cs
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

using System;
using Ubiety.Common;
using Ubiety.Common.Roster;
using Ubiety.Common.Sasl;
using Ubiety.Infrastructure;
using Ubiety.Net;

namespace Ubiety.States
{
    /// <summary>
    ///     Keeps track of all the current state information like id, password, socket and the current state.
    /// </summary>
    internal static class ProtocolState
    {
        static ProtocolState()
        {
            Settings = new XmppSettings();
            Events = new XmppEvents();
            Socket = new AsyncSocket();

            State = new DisconnectedState();
            Events.OnNewTag += EventsOnOnNewTag;
            Events.OnConnect += EventsOnOnConnect;
            Events.OnDisconnect += EventsOnOnDisconnect;
        }

        /// <value>
        ///     The current state we are in.
        /// </value>
        public static State State { get; set; }

        /// <value>
        ///     The socket used for connecting to the server.
        /// </value>
        internal static AsyncSocket Socket { get; }

        /// <value>
        ///     The current SASL processor based on server communication.
        /// </value>
        public static SaslProcessor Processor { get; set; }

        /// <value>
        ///     Are we authenticated yet?
        /// </value>
        public static bool Authenticated { get; set; }

        /// <summary>
        ///     Is the stream currently compressed?
        /// </summary>
        public static bool Compressed { get; set; }

        public static string Algorithm { get; set; }

        /// <summary>
        ///     The current static settings instance
        /// </summary>
        public static XmppSettings Settings { get; }

        /// <summary>
        ///     The current event instance.
        /// </summary>
        public static XmppEvents Events { get; }

        public static IRosterManager RosterManager { get; set; }

        private static void EventsOnOnDisconnect(object sender, EventArgs eventArgs)
        {
            if ((State is DisconnectedState)) return;
            State = new DisconnectState();
            State.Execute();
        }

        private static void EventsOnOnConnect(object sender, EventArgs eventArgs)
        {
            // We need an XID and Password to connect to the server.
            if (string.IsNullOrEmpty(Settings.Password))
            {
                Events.Error(null, ErrorType.MissingPassword, ErrorSeverity.Fatal,
                    "Must have a password in the settings to connect to a server.");
                return;
            }

            if (string.IsNullOrEmpty(Settings.Id))
            {
                Events.Error(null, ErrorType.MissingId, ErrorSeverity.Fatal,
                    "Must set the ID in the settings to connect to a server.");
                return;
            }

            // Set the current state to connecting and start the process.
            State = new ConnectingState();
            State.Execute();
        }

        private static void EventsOnOnNewTag(object sender, TagEventArgs args)
        {
            State.Execute(args.Tag);
        }
    }
}