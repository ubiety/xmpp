// Conflict.cs
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

using System.Xml;
using Ubiety.Common;
using Ubiety.Infrastructure.Attributes;

namespace Ubiety.Core.Errors
{
    /// <summary>
    ///     The server either (1) is closing the existing stream for this entity
    /// because a new stream has been inititated that conflicts with the existing
    /// stream, or (2) is refusing a new stream for this entity because allowing
    /// the new stream would conflict with an existing stream.
    /// </summary>
    [XmppTag("conflict", Namespaces.XmppStreams, typeof(Conflict))]
    public class Conflict : Tag
    {
        /// <summary>
        ///     Instantiate a new instance
        /// </summary>
        public Conflict() : base("", new XmlQualifiedName("conflict", Namespaces.XmppStreams)) { }
    }
}
