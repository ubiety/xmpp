// IDiscoManager.cs
//
//Ubiety XMPP Library Copyright (C) 2011 - 2015 Dieter Lunn
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

namespace Ubiety.Common.Disco
{
	///<summary>
	/// Interface defining the methods that should be exposed by a feature discovery class.
	///</summary>
	public interface IDiscoManager
	{
		/// <summary>
		/// Lookup a verification hash in the cache.
		/// </summary>
		/// <param name="hash">The hash to lookup.</param>
		void LookupHash(string hash);
	}
}