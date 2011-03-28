// CompressionRegistry.cs
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
using ubiety.attributes;
using System.IO;
using System.Reflection;
using ubiety.logging;
using ubiety.common;

namespace ubiety.registries
{
	///<summary>
	///</summary>
	public sealed class CompressionRegistry : Registry<CompressionRegistry, RegistryAllocator<CompressionRegistry>>
	{
		private CompressionRegistry()
		{
		}

		/// <summary>
		/// Add a compression stream to the library.  Zlib is the default.
		/// </summary>
		/// <param name="a">
		/// The assembly containing the stream definition.
		/// </param>
		public void AddCompression(Assembly a)
		{
            Logger.DebugFormat(this, "Adding assembly {0}", a.FullName);
            
            var tags = GetAttributes<CompressionAttribute>(a);
            foreach (var tag in tags)
            {
            	Logger.DebugFormat(this, "Adding {0}", tag.Algorithm);
            	RegisteredItems.Add(tag.Algorithm, tag.ClassType);
            }			
		}

		/// <summary>
		/// Creates the stream class for the compression algorithm specified.
		/// </summary>
		/// <param name="algorithm">
		/// The algorithm we want to create the stream for.
		/// </param>
		/// <param name="inner">
		/// The stream we want to wrap in our compression algorithm.
		/// </param>
		/// <returns>
		/// The wrapped stream ready for compression.
		/// </returns>
		public Stream GetCompression(string algorithm, Stream inner)
		{
			Logger.InfoFormat(this, "Finding algorithm {0}.", algorithm);
			Stream stream = null;
			try
			{
				//t = (Type)_registeredItems[algorithm];
				Type t;
				if (RegisteredItems.TryGetValue(algorithm, out t))
				{				
					stream = (Stream)Activator.CreateInstance(t, new object[] { inner });
				}
				else
				{
					Errors.Instance.SendError(this, ErrorType.UnregisteredItem, "Unable to find requested compression algorithm");
					return null;
				}
			}
			catch (Exception e)
			{
				Errors.Instance.SendError(this, ErrorType.UnregisteredItem, "Unable to find requested compression algorithm");
				Logger.Error(this, e);
			}
			return stream;
		}

		/// <summary>
		/// Does the library support the algorithm the server is requesting.
		/// </summary>
		/// <param name="algorithm">
		/// The algorithm we are looking for.
		/// </param>
		/// <returns>
		/// True if we have a stream class available.  False if not.
		/// </returns>
		public static bool SupportsAlgorithm(string algorithm)
		{
			return RegisteredItems.ContainsKey(algorithm);
		}

		/// <value>
		/// Do we have any algorithms to use?
		/// </value>
		public static bool AlgorithmsAvailable
		{
			get
			{
				return RegisteredItems.Count >= 1;
			}
		}
	}
}
