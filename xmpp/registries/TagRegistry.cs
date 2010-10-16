// TagRegistry.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2009 Dieter Lunn
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
using System.Xml;
using System.Reflection;
using ubiety.common;
using ubiety.logging;
using ubiety.attributes;

namespace ubiety.registries
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
            Logger.DebugFormat(this, "{0,-15}{1,-36}{2}", "Tag Name", "Class", "Namespace");
            foreach (XmppTagAttribute tag in tags)
            {
            	Logger.DebugFormat(this, "{0,-15}{1,-36}{2}", tag.Name, tag.ClassType.FullName, tag.NS);
            	_registeredItems.Add(new XmlQualifiedName(tag.Name, tag.NS).ToString(), tag.ClassType);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Tag GetTag(string name, string ns, XmlDocument doc)
        {
            XmlQualifiedName qname = new XmlQualifiedName(name, ns);
            return GetTag(qname, doc);
        }

        /// <summary>
        /// Creates a new instance of the wanted tag.
        /// </summary>
        /// <param name="qname">Qualified Namespace</param>
        /// <param name="doc">XmlDocument to create tag with</param>
        /// <returns>A new instance of the requested tag</returns>
		public Tag GetTag(XmlQualifiedName qname, XmlDocument doc)
		{
			Type t;
			Tag tag = null;
			ConstructorInfo ctor = null;

			Logger.DebugFormat(this, "Finding tag: {0}", qname);
			try
			{
				if (_registeredItems.TryGetValue(qname.ToString(), out t))
				{
	        		//tag = (Tag)Activator.CreateInstance(t, new object[] { doc });
	        		ctor = t.GetConstructor(new Type[] { typeof(XmlDocument) });
	        		if (ctor == null)
	        		{
	        			ctor = t.GetConstructor(new Type[] { typeof(XmlDocument), typeof(XmlQualifiedName) });
	        			tag = (Tag)ctor.Invoke(new object[] { doc, qname });
	        		}
	        		else
	        		{
	        			tag = (Tag)ctor.Invoke(new object[] { doc });
	        		}
				}
				else
				{				
					Errors.Instance.SendError(this, ErrorType.UnregisteredItem, "Tag " + qname + " not found in registry.  Please load appropriate library.");
					return null;
				}
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
