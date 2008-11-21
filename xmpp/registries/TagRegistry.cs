// TagRegistry.cs
//
//XMPP .NET Library Copyright (C) 2006, 2008 Dieter Lunn
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
using System.Xml;
using System.Reflection;
using xmpp.common;
using xmpp.logging;
using xmpp.attributes;

namespace xmpp.registries
{
    /// <remarks>
    /// TagRegistry stores all the construction information for the <seealso cref="Tag">Tags</seealso> the library is aware of.
    /// </remarks>
	public sealed class TagRegistry : Registry<TagRegistry, RegistryAllocator<TagRegistry>>
	{
		private TagRegistry()
		{
		}

		/// <summary>
		/// Used to add <seealso cref="Tag">Tag(s)</seealso> to the registry.  Using attributes the <see cref="TagRegistry"/> looks for and adds any appropriate tags found in the assembly.
		/// </summary>
		/// <param name="ass">The assembly to search for tags</param>
		public void AddAssembly(Assembly ass)
		{
            Logger.DebugFormat(this, "Adding assembly {0}", ass.FullName);
            
            XmppTagAttribute[] tags = GetAttributes<XmppTagAttribute>(ass);
            Logger.DebugFormat(this, "{0,-15}{1,-34}{2}", "Tag Prefix", "Class", "Namespace");
            foreach (XmppTagAttribute tag in tags)
            {
            	Logger.DebugFormat(this, "{0,-15}{1,-34}{2}", tag.Prefix, tag.ClassType.FullName, tag.NS);
            	_registeredItems.Add(new XmlQualifiedName(tag.Prefix, tag.NS), tag.ClassType);
            }
		}

        /// <summary>
        /// Creates a new instance of the wanted tag.
        /// </summary>
        /// <param name="prefix">The tag prefix</param>
        /// <param name="qname">Qualified Namespace</param>
        /// <param name="doc">XmlDocument to create tag with</param>
        /// <returns>A new instance of the requested tag</returns>
		public Tag GetTag(string prefix, XmlQualifiedName qname, XmlDocument doc)
		{
			Type t = null;
			Tag tag = null;
			Logger.DebugFormat(this, "Finding tag: {0}", qname);
			try
			{
				t = (Type)_registeredItems[qname];
        		tag =  (Tag)Activator.CreateInstance(t, new object[] { prefix, qname, doc });
			}
			catch (Exception e)
			{
				Logger.ErrorFormat(this, "Unable to find tag {0} in registry.  Check to make sure library is loaded into registry.", qname);
        		Logger.Error(this, e);
			}        	
        	return tag;
        }
	}
}
