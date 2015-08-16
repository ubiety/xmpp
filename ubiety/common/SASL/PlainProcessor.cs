// PlainProcessor.cs
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
using System.Text;
using Ubiety.Core;
using Ubiety.Core.Sasl;
using Ubiety.Registries;

namespace Ubiety.Common.Sasl
{
    /// <summary>
    /// </summary>
    public class PlainProcessor : SaslProcessor
    {
        /// <summary>
        /// </summary>
        /// <param name="tag"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override Tag Step(Tag tag)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public override Tag Initialize(string id, string password)
        {
            base.Initialize(id, password);

            var sb = new StringBuilder();

            sb.Append((char) 0);
            sb.Append(Id.User);
            sb.Append((char) 0);
            sb.Append(Password);

            var auth = TagRegistry.GetTag<Auth>("auth", Namespaces.Sasl);

            auth.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
            auth.Mechanism = Mechanism.GetMechanism(MechanismType.Plain);

            return auth;
        }
    }
}