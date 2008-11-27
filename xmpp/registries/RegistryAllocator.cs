// RegistryAllocator.cs
//
//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
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
using System.Reflection;

namespace ubiety.registries
{	
	/// <summary>
	/// 
	/// </summary>
	public class RegistryAllocator<T> : Allocator<T> where T : class
	{
	
		private static readonly T instance;
		
		static RegistryAllocator()
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
			if (constructor == null) {
				throw new Exception("The singleton doesn't have a private/protected constructor");
			}
			
			try
			{
				instance = constructor.Invoke(new object[0]) as T;
			}
			catch (Exception e)
			{
				throw new Exception("The class couldn't be constructed, make sure it has a default constructor.", e);
			}
		}
		
		private RegistryAllocator () {}
		
		/// <value>
		/// 
		/// </value>
		public override T Instance {
			get { return instance; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void Dispose ()
		{
			
		}

	}
}
