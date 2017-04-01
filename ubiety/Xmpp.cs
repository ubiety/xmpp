// XMPP.cs
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

using Serilog;
using Ubiety.Common;
using Ubiety.Registries;

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
    ///      public static Main()
    /// 	 {
    /// 	     // Create a new instance of the XMPP class
    /// 		 var xmpp = Xmpp.Connect(new JID("user@jabber.org/chat"), "password");
    /// 		 xmpp.Events.NewTag += Test_OnNewTag;
    /// 	 }
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
        static Xmpp()
        {
            ILogger log =
                new LoggerConfiguration().MinimumLevel.Debug()
                    .WriteTo.RollingFile("Logs\\log-{Date}.txt")
                    .CreateLogger();
            Log.Logger = log;

            TagRegistry.AddAssembly(typeof (Xmpp).Assembly);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static XmppState Connect(JID user, string password, int port = 5222)
        {
            var state = new XmppState
            {
                Settings =
                {
                    Id = user,
                    Password = password,
                    Port = port
                }
            };
            return state;
        }
    }
}