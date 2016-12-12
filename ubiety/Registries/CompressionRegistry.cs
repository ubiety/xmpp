// CompressionRegistry.cs
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
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using Ubiety.Common;
using Ubiety.Infrastructure.Attributes;
using Ubiety.Infrastructure.Extensions;
using Ubiety.States;

namespace Ubiety.Registries
{
    /// <summary>
    /// </summary>
    public static class CompressionRegistry
    {
        private static readonly Dictionary<string, Type> RegisteredItems = new Dictionary<string, Type>();

        /// <value>
        ///     Do we have any algorithms to use?
        /// </value>
        public static bool AlgorithmsAvailable => RegisteredItems.Count >= 1;

        /// <summary>
        ///     Add a compression stream to the library.  Zlib is the default.
        /// </summary>
        /// <param name="a">
        ///     The assembly containing the stream definition.
        /// </param>
        public static void AddCompression(Assembly a)
        {
            Log.Debug("Loading compression algorithms from {Assembly}", a.FullName);

            IEnumerable<CompressionAttribute> tags = a.GetAttributes<CompressionAttribute>();
            foreach (CompressionAttribute tag in tags)
            {
                Log.Debug("Loading algorithm {Algorithm}", tag.Algorithm);
                RegisteredItems.Add(tag.Algorithm, tag.ClassType);
            }
        }

        /// <summary>
        ///     Creates the stream class for the compression algorithm specified.
        /// </summary>
        /// <param name="algorithm">
        ///     The algorithm we want to create the stream for.
        /// </param>
        /// <returns>
        ///     The wrapped stream ready for compression.
        /// </returns>
        public static ICompression GetCompression(string algorithm)
        {
            ICompression stream;
            try
            {
                Type t;
                if (RegisteredItems.TryGetValue(algorithm, out t))
                {
                    stream = (ICompression) Activator.CreateInstance(t);
                }
                else
                {
                    ProtocolState.Events.Error(null, ErrorType.UnregisteredItem, ErrorSeverity.Information, "Unable to find requested compression algorithm.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to locate appropriate compression algorithm.");
                ProtocolState.Events.Error(null, ErrorType.UnregisteredItem, ErrorSeverity.Information, "Unable to find requested compression algorithm.");

                throw;
            }
            return stream;
        }

        /// <summary>
        ///     Does the library support the algorithm the server is requesting.
        /// </summary>
        /// <param name="algorithm">
        ///     The algorithm we are looking for.
        /// </param>
        /// <returns>
        ///     True if we have a stream class available.  False if not.
        /// </returns>
        public static bool SupportsAlgorithm(string algorithm)
        {
            return RegisteredItems.ContainsKey(algorithm);
        }
    }
}