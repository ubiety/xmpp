// DefaultRosterManager.cs
//
//Ubiety XMPP Library Copyright (C) 2015 Dieter Lunn
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

using Ubiety.Core.Iq;
using Ubiety.Registries;
using Ubiety.States;

namespace Ubiety.Common.Roster
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultRosterManager : IRosterManager
    {
        /// <summary>
        /// 
        /// </summary>
        public void RequestRoster()
        {
            var iq = TagRegistry.GetTag<Iq>("iq", Namespaces.Client);
            iq.IqType = IqType.Get;
            iq.From = ProtocolState.Settings.Id;

            var query = TagRegistry.GetTag<Query>("query", Namespaces.Roster);

            iq.Payload = query;

            ProtocolState.Events.Send(this, iq);
        }
    }
}
