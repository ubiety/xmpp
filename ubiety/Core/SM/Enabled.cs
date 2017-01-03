// Enabled.cs
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

namespace Ubiety.Core.SM
{
    /// <summary>
    ///
    /// </summary>
    [XmppTag("enabled", Namespaces.StreamManagementV3, typeof(Enabled))]
    public class Enabled : Tag
    {
        /// <summary>
        ///
        /// </summary>
        public Enabled() : base("", new XmlQualifiedName("enabled", Namespaces.StreamManagementV3))
        {
        }

        /// <summary>
        ///
        /// </summary>
        public string Id
        {
            get { return GetAttribute("id"); }
            set { SetAttribute("id", value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Resume
        {
            get { return GetBoolAttribute("resume"); }
            set { SetAttribute("resume", value.ToString()); }
        }
    }
}