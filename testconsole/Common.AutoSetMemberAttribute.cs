/*
 * Common.AutoSetMemberAttribute - Custom attribute class to automatically set a member according to a command line argument
 * Copyright (C) 2003 Sébastien Lorion
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 */

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Common
{
	/// <summary>
	/// Used by the <see cref="Common.ArgumentParser"/> class to automatically set the affected member according to the value of the related command line argument.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AutoSetMemberAttribute : Attribute
	{
		/// <summary>
		/// Supported <see cref="System.Reflection.BindingFlags"/>.
		/// </summary>
		public const BindingFlags SupportedBindingFlags = 
			BindingFlags.DeclaredOnly
			| BindingFlags.Instance
			| BindingFlags.NonPublic
			| BindingFlags.Public
			| BindingFlags.Static
			| BindingFlags.FlattenHierarchy;

		/// <summary>
		/// Supported <see cref="System.Reflection.MemberTypes"/>.
		/// </summary>
		public const MemberTypes SupportedMemberTypes =
			MemberTypes.Field
			| MemberTypes.Property;

		private ArrayList m_aliases = null;
		private String m_description = null;
		private bool m_switchMeansFalse = false;
		private object m_ID = null;

		/// <summary>
		/// Creates a new instance of <see cref="AutoSetMemberAttribute"/> class with no related command line argument aliases.
		/// </summary>
		public AutoSetMemberAttribute()
		{
			m_aliases = new ArrayList();
		}

		/// <summary>
		/// Creates a new insance of <see cref="AutoSetMemberAttribute"/> class with one or more possible related command line argument aliases.
		/// </summary>
		/// <param name="aliases">One or more possible related command line argument aliases.</param>
		public AutoSetMemberAttribute(params String[] aliases)
		{
			m_aliases = new ArrayList(aliases);
		}

		/// <summary>
		/// Gets the possible related command line argument aliases.
		/// </summary>
		public ArrayList Aliases
		{
			get {return m_aliases;}
		}

		/// <summary>
		/// Gets or sets the description of the command line argument.
		/// </summary>
		public String Description
		{
			get {return m_description;}
			set {m_description = value;}
		}

		/// <summary>
		/// Indicates the meaning of a command line switch.
		/// </summary>
		public bool SwitchMeansFalse
		{
			get {return m_switchMeansFalse;}
			set {m_switchMeansFalse = value;}
		}

		/// <summary>
		/// Gets or sets the ID of this instance.
		/// </summary>
		public object ID
		{
			get {return m_ID;}
			set {m_ID = value;}
		}

		#region Resources handling
		private static ResourceManager m_res;
		private static CultureInfo m_culture;
		private String m_resID = null;

		/// <summary>
		/// Gets or sets the <see cref="System.Globalization.ResourceManager"/> to be used for retrieving culture aware aliases.
		/// <seealso cref="AutoSetMemberAttribute.Culture"/>
		/// <seealso cref="AutoSetMemberAttribute.ResID"/>
		/// </summary>
		public static ResourceManager Resources
		{
			get {return m_res;}
			set {m_res = value;}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Globalization.CultureInfo"/> to be used for retrieving culture aware aliases.
		/// <seealso cref="AutoSetMemberAttribute.Resources"/>
		/// <seealso cref="AutoSetMemberAttribute.ResID"/>
		/// </summary>
		public static CultureInfo Culture
		{
			get {return m_culture;}
			set {m_culture = value;}
		}

		/// <summary>
		/// Gets or sets the resource ID to be used for retrieving culture aware aliases.
		/// <seealso cref="AutoSetMemberAttribute.Resources"/>
		/// <seealso cref="AutoSetMemberAttribute.Culture"/>
		/// </summary>
		public String ResID
		{
			get {return m_resID;}
			set {m_resID = value;}
		}
		#endregion
	}
}