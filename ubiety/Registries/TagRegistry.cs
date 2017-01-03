// TagRegistry.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2017 Dieter Lunn
// 
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this library; if not, write to the Free Software Foundation, Inc., 59
// Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Serilog;
using Ubiety.Common;
using Ubiety.Infrastructure.Attributes;
using Ubiety.Infrastructure.Extensions;
using Ubiety.States;

namespace Ubiety.Registries
{
    /// <remarks>
    ///     TagRegistry stores all the construction information for the <seealso cref="Tag">Tags</seealso> the library is aware
    ///     of.
    /// </remarks>
    public static class TagRegistry
    {
        private static readonly Dictionary<string, Type> RegisteredItems = new Dictionary<string, Type>();

        /// <summary>
        ///     Used to add <seealso cref="Tag">Tag(s)</seealso> to the registry. Using attributes the <see cref="TagRegistry" />
        ///     looks for and adds any appropriate tags found in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search for tags</param>
        public static void AddAssembly(Assembly assembly)
        {
            Log.Debug("Loading tags from assembly {assembly}", assembly.FullName);

            var tags = assembly.GetAttributes<XmppTagAttribute>();
            foreach (var tag in tags)
            {
                Log.Debug("Loading tag {TagName} as class {ClassName} in the {Namespace} namespace.", tag.Name, tag.ClassType.FullName, tag.Namespace);
                RegisteredItems.Add(new XmlQualifiedName(tag.Name, tag.Namespace).ToString(), tag.ClassType);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static T GetTag<T>(string name, string ns) where T : Tag
        {
            return GetTag<T>(new XmlQualifiedName(name, ns));
        }

        /// <summary>
        ///     Creates a new instance of the wanted tag.
        /// </summary>
        /// <param name="qname">Qualified Namespace</param>
        /// <returns>A new instance of the requested tag</returns>
        public static T GetTag<T>(XmlQualifiedName qname) where T : Tag
        {
            T tag = null;
            Type t;

            Log.Debug("Finding tag {TagName}...", qname);

            if (RegisteredItems.TryGetValue(qname.ToString(), out t))
            {
                var ctor = t.GetConstructor(new Type[] {});
                if (ctor == null)
                {
                    ctor = t.GetConstructor(new[] {typeof (XmlQualifiedName)});
                    if (ctor != null) tag = (T) ctor.Invoke(new object[] {qname});
                }
                else
                {
                    tag = (T) ctor.Invoke(new object[] {});
                }
            }
            else
            {
                ProtocolState.Events.Error(null, ErrorType.UnregisteredItem, ErrorSeverity.Information, "Tag {0} not found in registry. Please load appropriate library.", qname);
                return null;
            }

            return tag;
        }
    }
}