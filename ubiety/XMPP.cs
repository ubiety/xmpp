// XMPP.cs
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
using Serilog;
using Ubiety.Common;
using Ubiety.Infrastructure;
using Ubiety.Registries;
using Ubiety.States;

namespace Ubiety
{
    /// <summary>
    ///     Implements the XMPP(Jabber) Core and IM protocols
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Extensible Messaging and Presence Protocol (XMPP) is an open XML technology for real-time
    ///         communications, which powers a wide range of applications including instant messaging, presence,
    ///         media negotiation, white boarding, collaboration, lightweight middle-ware, content syndication, and
    ///         generalized XML delivery.
    ///     </para>
    ///     <para>
    ///         This library is an implementation of this protocol.  Those involved with the design and development
    ///         of this library are as committed to open standards as the committees who created the original protocol.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///  using Ubiety;
    ///  using Ubiety.Common;
    /// 
    ///  public class Test
    ///  {
    /// 		public static Main()
    /// 		{
    /// 			// Create a new instance of the XMPP class
    /// 			Xmpp ubiety = new Xmpp();
    ///             ubiety.Settings.Id = new JID("user@jabber.org/chat");
    ///             ubiety.Settings.Password = "password";
    /// 			
    ///             ubiety.Connect();
    /// 		}
    ///  }
    ///  </code>
    /// </example>
    public class Xmpp
    {
        /// <summary>
        ///     Version of the library.
        /// </summary>
        public static readonly string Version = typeof (Xmpp).Assembly.GetName().Version.ToString();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Xmpp" /> class.
        /// </summary>
        public Xmpp()
        {
            ILogger log =
                new LoggerConfiguration().MinimumLevel.Debug()
                    .WriteTo.RollingFile("Logs\\log-{Date}.txt")
                    .WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();
            Log.Logger = log;

            TagRegistry.AddAssembly(typeof (Xmpp).Assembly);
        }

        /// <summary>
        ///     Connects this instance to an XMPP server.
        /// </summary>
        public void Connect()
        {
            ProtocolState.Events.Connect(this);
        }

        /// <summary>
        ///     Disconnects this instance from the server.
        /// </summary>
        public void Disconnect()
        {
            ProtocolState.Events.Disconnect(this);
        }

        /// <summary>
        ///     Send a message to the server via a Tag
        /// </summary>
        /// <param name="tag">Tag to send to the server</param>
        public void Send(Tag tag)
        {
            Send(new TagEventArgs(tag));
        }

        /// <summary>
        ///     Send a message to the server via a Tag
        /// </summary>
        /// <param name="args">Event argument containing tag to send to the server</param>
        public void Send(TagEventArgs args)
        {
            ProtocolState.Events.Send(this, args);
        }

        /// <summary>
        ///     An error occurred
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError
        {
            add { ProtocolState.Events.OnError += value; }
            remove { ProtocolState.Events.OnError -= value; }
        }

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Xmpp" /> is connected to a server.
        /// </summary>
        /// <value>
        ///     <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return ProtocolState.State is RunningState; }
        }

        /// <summary>
        ///     Current settings of the application including Id and Password.
        /// </summary>
        public XmppSettings Settings
        {
            get { return ProtocolState.Settings; }
        }

        #endregion
    }
}