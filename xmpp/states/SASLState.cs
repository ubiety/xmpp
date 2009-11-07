// SASLState.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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

using ubiety;
using ubiety.common;
using ubiety.logging;

namespace ubiety.states
{
	/// <summary>
	/// SASL state is used to authenticate the user with the current processor.
	/// </summary>
	public class SASLState : State
	{
		/// <summary>
		/// Create a new state.
		/// </summary>
		public SASLState() : base ()
		{
		}
		
		/// <summary>
		/// Execute the actions to authenticate the user.
		/// </summary>
		/// <param name="data">
		/// The <see cref="Tag"/> we received from the server.  Probably a challenge or response.
		/// </param>
		public override void Execute(Tag data)
		{
			Logger.Debug(this, "Processing next SASL step");
			ubiety.common.Tag res = _current.Processor.Step(data);
			if (res is ubiety.core.SASL.Success)
			{
				// We have been successfully authenticated and we need to restart the stream.
				Logger.Debug(this, "Sending start stream again");
				_current.Authenticated = true;
				// Return to the connected state to resend the start tag.
				_current.State = new ConnectedState();
				_current.Execute(null);
			}
			else if (res is ubiety.core.SASL.Failure)
			{
				// We have failed in our quest.  Error returned inside we just need to wrap this up.
				return;
			}
			else
			{
				// Neither success or failure so we send the result to the socket.
				_current.Socket.Write(res);
			}
		}
	}
}
