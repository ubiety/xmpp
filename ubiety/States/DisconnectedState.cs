// ClosedState.cs
//
//Ubiety XMPP Library Copyright (C) 2009 - 2017 Dieter Lunn
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

using Ubiety.Common;
using Ubiety.Core;
using Ubiety.Infrastructure;

namespace Ubiety.States
{
    /// <summary>
    /// </summary>
    public class DisconnectedState : IState
    {
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        public void Execute(Tag data = null)
        {
            if (data != null)
            {
                // TODO - Bubble the error message out
                var tag = ((Stream) data).Error;
                
            }
            else
            {
                ProtocolParser.Reset();
                ProtocolState.Socket.Disconnect();                
            }
        }
    }
}