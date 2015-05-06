// CompressionAttribute.cs
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

namespace Ubiety.Infrastructure.Attributes
{
	///<summary>
	///</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class CompressionAttribute : Attribute
	{
	    ///<summary>
		///</summary>
		///<param name="compression"></param>
		///<param name="type"></param>
		public CompressionAttribute(string compression, Type type)
		{
			Algorithm = compression;
			ClassType = type;
		}

		///<summary>
		///</summary>
		public string Algorithm { get; }

	    ///<summary>
		///</summary>
		public Type ClassType { get; }
	}
}