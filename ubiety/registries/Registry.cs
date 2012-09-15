// Registry.cs
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

using System;
using System.Collections.Generic;
using System.Reflection;
using ubiety.common.extensions;

namespace ubiety.registries
{
	/// <summary>
	/// 
	/// </summary>
	public class Registry<T, TAllocator> : IDisposable where T : class where TAllocator : Allocator<T>
	{
		private static readonly Allocator<T> Allocator;
		
		/// <summary>
		/// 
		/// </summary>
		protected static Dictionary<string, Type> RegisteredItems = new Dictionary<string, Type>();
		
		static Registry()
		{
			var constructor = typeof(TAllocator).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
			if (constructor == null)
			{
				throw new Exception("The allocator that you want to create doesn't have a protected/private constructor");
			}
			
			try
			{
				Allocator = constructor.Invoke(new object[0]) as Allocator<T>;
			}
			catch (Exception e)
			{
				throw new Exception("The allocator couldn't be constructed, make sure it has a default constructor.", e);
			}
		}
		
		/// <value>
		/// 
		/// </value>
		public static T Instance
		{
			get { return Allocator.Instance; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ass">
		/// A <see cref="Assembly"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Object"/>
		/// </returns>
		protected IEnumerable<TE> GetAttributes<TE> (Assembly ass) where TE : Attribute
		{
			var tags = new List<TE>();
			var types = ass.GetTypes();

			foreach (var type in types)
			{
				type.GetCustomAttributes<TE>(true).Apply(tags.Add);
			}

			return tags;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void Dispose() 
		{
			Allocator.Dispose();
			RegisteredItems.Clear();
		}
	}
}
