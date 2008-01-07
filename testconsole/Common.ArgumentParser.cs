/*
 * Common.ArgumentParser - Parsing class for command line arguments
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
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
	/// <summary>
	/// Enumerates possible command line argument formats.
	/// </summary>
	[Flags]
	public enum ArgumentFormats
	{
		/// <summary>
		/// Name / value pairs. Eg. -foo=bar
		/// </summary>
		NamedValue = 0x1,
		/// <summary>
		/// Flags. Eg. -abcd means options a, b, c and d are on.
		/// </summary>
		Flags = 0x2,
		/// <summary>
		/// All supported formats.
		/// </summary>
		All = 0x7FFF
	}

	/// <summary>
	/// Collection of accepted flag name/values combinations.
	/// </summary>
	/// <remarks>The key is the flag identifier and the value is all possible flag values. If you want no flag identifier, simply use an empty string as key.</remarks>
	public class FlagCollection : StringDictionary
	{
		/// <summary>
		/// Initializes a new instance of the FlagCollection.
		/// </summary>
		public FlagCollection() : base() {}
	}

	/// <summary>
	/// Represents a parsing class for command line arguments.
	/// </summary>
	public class ArgumentParser
	{
		private StringDictionary m_handled = null;
		private StringDictionary m_unhandled = null;
		private bool m_ignoreCase = false;

		#region Constructors
		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <remarks>By default, all supported formats will be accepted and parsing is case sensitive.</remarks>
		public ArgumentParser() : this(ArgumentFormats.All, false, null, null) {}

		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <param name="format">The valid format(s).</param>
		public ArgumentParser(ArgumentFormats format) : this(format, false, null, null) {}

		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <param name="format">The valid format(s).</param>
		/// <param name="ignoreCase">Indicates if parsing is case sensitive.</param>
		public ArgumentParser(ArgumentFormats format, bool ignoreCase) : this(format, ignoreCase, null, null) {}

		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <param name="format">The valid format(s).</param>
		/// <param name="ignoreCase">Indicates if parsing is case sensitive.</param>
		/// <param name="flags">Accepted flags (named or not) and their respective values.</param>
		public ArgumentParser(ArgumentFormats format, bool ignoreCase, FlagCollection flags) : this(format, ignoreCase, flags, null) {}

		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <param name="format">The valid format(s).</param>
		/// <param name="flags">Accepted flags (named or not) and their respective values.</param>
		/// <param name="ignoreCase">Indicates if parsing is case sensitive.</param>
		/// <param name="customPattern">An additional regular expression pattern to be used when parsing arguments. It will have priority over the standard pattern.</param>
		/// <remarks>In your pattern, use capture name constants made public by this class.</remarks>
		public ArgumentParser(ArgumentFormats format, bool ignoreCase, FlagCollection flags, String customPattern)
		{
			m_handled = new StringDictionary();
			m_unhandled = new StringDictionary();

			m_allowedFormats = format;
			m_ignoreCase = ignoreCase;
			m_flags = flags;
			m_customPattern = customPattern;

			// default values

			m_allowedPrefixes = new Char[] {'-', '/'};
			m_assignSymbols = new Char[] {'=', ':'};
		}

		/// <summary>
		/// Parses an array of arguments, typically obtained from Main method of console application.
		/// </summary>
		/// <param name="customPattern">The custom pattern to be used.</param>
		/// <param name="useOnlyCustomPattern">Indicates if the custom pattern override the built-in pattern.</param>
		/// <remarks>In your pattern, use capture name constants made public by this class.</remarks>
		public ArgumentParser(String customPattern, bool useOnlyCustomPattern)
		{
			m_handled = new StringDictionary();
			m_unhandled = new StringDictionary();

			m_customPattern = customPattern;
			m_useOnlyCustomPattern = useOnlyCustomPattern;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the argument(s) that have been automatically set by AutoSetMembers method.
		/// </summary>
		public StringDictionary HandledArguments
		{
			get {return m_handled;}
		}

		/// <summary>
		/// Gets the argument(s) that have not been automatically set by AutoSetMembers method.
		/// </summary>
		public StringDictionary UnhandledArguments
		{
			get {return m_unhandled;}
		}
		#endregion

		#region Parsing
		public const String FlagNameCaptureName = "flagName";
		public const String FlagsCaptureName = "flags";
		public const String PrefixCaptureName = "prefix";
		public const String ArgumentNameCaptureName = "name";
		public const String ArgumentValueCaptureName = "value";
		
		private bool m_useOnlyCustomPattern = false;
		private Char m_literalStringSymbol = '@';
		private String m_customPattern = null;

		private Char[] m_allowedPrefixes = null;
		private Char[] m_assignSymbols = null;
		
		private ArgumentFormats m_allowedFormats = ArgumentFormats.All;
		private FlagCollection m_flags = null;

		#region Properties
		/// <summary>
		/// Gets or sets the accepted argument format(s).
		/// </summary>
		public ArgumentFormats ArgumentFormats
		{
			get {return m_allowedFormats;}
			set {m_allowedFormats = value;}
		}

		/// <summary>
		/// Gets or sets the accepted prefix(es).
		/// </summary>
		public Char[] AllowedPrefixes
		{
			get {return m_allowedPrefixes;}
			set {m_allowedPrefixes = value;}
		}

		/// <summary>
		/// Gets or sets the assignation symbol(s).
		/// </summary>
		public Char[] AssignSymbols
		{
			get {return m_assignSymbols;}
			set {m_assignSymbols = value;}
		}

		/// <summary>
		/// Gets or sets the case sensitive status.
		/// </summary>
		public bool IgnoreCase
		{
			get {return m_ignoreCase;}
			set {m_ignoreCase = value;}
		}

		/// <summary>
		/// Gets or sets the litteral string symbol.
		/// </summary>
		public Char LiteralStringSymbol
		{
			get {return m_literalStringSymbol;}
			set {m_literalStringSymbol = value;}
		}

		/// <summary>
		/// Gets or sets the custom pattern.
		/// </summary>
		public String CustomPattern
		{
			get {return m_customPattern;}
			set {m_customPattern = value;}
		}

		/// <summary>
		/// Gets or sets the override status of the custom pattern.
		/// </summary>
		public bool UseOnlyCustomPattern
		{
			get {return m_useOnlyCustomPattern;}
			set {m_useOnlyCustomPattern = value;}
		}
		#endregion

		/// <summary>
		/// Parses the array of arguments.
		/// </summary>
		/// <param name="args">The array of arguments.</param>
		/// <returns>The dictionary holding parsed arguments.</returns>
		public StringDictionary Parse(String[] args)
		{
			int noNameCount = 0;

			Regex argex = BuildPattern();

			foreach(string arg in args)
			{
				Match match = argex.Match(arg);

				if (!match.Success)
					throw new ArgumentException("Invalid argument format: " + arg);
				else
				{
					if (match.Groups[PrefixCaptureName].Success)
					{
						if (match.Groups[FlagsCaptureName].Success)
							m_unhandled.Add(match.Groups[FlagNameCaptureName].Value, match.Groups[FlagsCaptureName].Value);
						else
							m_unhandled.Add(match.Groups[ArgumentNameCaptureName].Value, match.Groups[ArgumentValueCaptureName].Value);
					}
					else
					{
						m_unhandled.Add(noNameCount.ToString(), match.Groups[ArgumentValueCaptureName].Value);
						noNameCount++;
					}
				}
			}

			return m_unhandled;
		}

		/// <summary>
		/// Builds the pattern to be used when parsing each argument.
		/// </summary>
		/// <returns>A Regex instance to be used for parsing arguments.</returns>
		private Regex BuildPattern()
		{
			// The whole parsing string (with all possible argument formats) :
			// ---------------------------------------------------------------
			// (CUSTOM_PATTERN)
			// |(^(?<prefix>[PREFIXES])(?<flagName>)FLAG_NAMES)(?<flags>[FlagsCaptureName]+)$)
			// |(^(?<prefix>[PREFIXES])(?<name>[^EQUAL_SYMBOLS]+)([EQUAL_SYMBOLS](?<value>.+))?$)
			// |(LITERAL_STRING_SYMBOL?(?<value>.*))
			//
			// Again, but commented :
			// ----------------------
			// (CUSTOM_PATTERN)|				# custom pattern, if any (it has priority over standard pattern)
			//
			// foreach flag in FlagCollection :
			//
			// (^
			// (?<prefix>[PREFIXES])			# mandatory prefix
			// (?<flagName>)FLAG_NAMES)			# flag name
			// (?<flags>[FlagsCaptureName]+)				# flag value
			// $)|
			//
			// (^
			// (?<prefix>[PREFIXES])			# mandatory prefix
			// (?<name>[^EQUAL_SYMBOLS]+)		# argument name (which includes flag name/values)
			// ([EQUAL_SYMBOLS](?<value>.+))?	# argument value, if any
			// $)
			//
			// |(
			// LITERAL_STRING_SYMBOL?			# optional @ caracter indicating literal string
			// (?<value>.*)						# standalone value (will be given an index when parsed in Parse() method)
			// )

			RegexOptions regexOptions = RegexOptions.ExplicitCapture;

			if (m_ignoreCase)
				regexOptions |= RegexOptions.IgnoreCase;

			if (m_useOnlyCustomPattern)
				return new Regex(m_customPattern, regexOptions);
			else
			{
				StringBuilder argPattern = new StringBuilder();

				// build prefixes pattern
				StringBuilder prefixesBuilder = new StringBuilder();

				if (m_allowedPrefixes != null)
					if (m_allowedPrefixes.Length != 0)
					{
						prefixesBuilder.Append("(?<" + PrefixCaptureName + ">[");
					
						foreach (Char prefix in m_allowedPrefixes)
							prefixesBuilder.Append(prefix);

						prefixesBuilder.Append("])");
					}
			
				String prefixes = prefixesBuilder.ToString();

				// build equality symbols pattern
				String equalSymbols = '[' + new String(m_assignSymbols) + ']';

				// build custom pattern
				if (m_customPattern != null)
				{
					argPattern.Append('(');
					argPattern.Append(m_customPattern);
					argPattern.Append(")|");
				}

				// build flag pattern(s)
				if ((m_allowedFormats & ArgumentFormats.Flags) != 0)
				{
					foreach (DictionaryEntry flag in m_flags)
					{
						// pattern template : (^PREFIX_PATTERN(?<flagName>KEY)(?<flags>[ArgumentValueCaptureName]+))|
						// eg. (^(?<prefix>[-/])(?<flagName>a)(?<flags>[ABCDEFG]+))|

						argPattern.Append("(^");
						argPattern.Append(prefixes);
						argPattern.Append("(?<" + FlagNameCaptureName + ">");
						argPattern.Append(flag.Key);
						argPattern.Append(")(?<" + FlagsCaptureName + ">[");
						argPattern.Append(flag.Value);
						argPattern.Append("]+)$)|");
					}
				}

				// named arguments pattern
				argPattern.Append("(^");
				argPattern.Append(prefixes);

				if ((m_allowedFormats & ArgumentFormats.NamedValue) != 0)
				{
					argPattern.Append("(?<" + ArgumentNameCaptureName + ">[^");
					argPattern.Append(m_assignSymbols);
					argPattern.Append("]+)");

					argPattern.Append("([");
					argPattern.Append(m_assignSymbols);
					argPattern.Append("](?<" + ArgumentValueCaptureName + ">.+))?");
				}
				else
				{
					argPattern.Append("(?<" + ArgumentNameCaptureName + ">.+)");
				}

				argPattern.Append("$)");

				// standalone values
				argPattern.Append("|(@?(?<" + ArgumentValueCaptureName + ">.*))");

				return new Regex(argPattern.ToString(), regexOptions);
			}
		}
		#endregion

		#region AutoSetMembers
		/// <summary>
		/// Automatically sets members for the provided <see cref="System.Reflection.Assembly"/>.
		/// </summary>
		/// <param name="assembly">The <see cref="System.Reflection.Assembly"/> to process.</param>
		public void AutoSetMembers(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();

			foreach (Type type in types)
				AutoSetMembers(type);
		}

		/// <summary>
		/// Automatically sets members for the provided <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to process.</param>
		/// <remarks>Only static members will be processed.</remarks>
		public void AutoSetMembers(Type type)
		{
			MemberInfo[] members = type.FindMembers(AutoSetMemberAttribute.SupportedMemberTypes, AutoSetMemberAttribute.SupportedBindingFlags, Type.FilterName, "*");

			foreach (MemberInfo member in members)
				AutoSetMembers(type, member);
		}

		/// <summary>
		/// Automatically sets members for the provided class instance.
		/// </summary>
		/// <param name="instance">The class instance to process. Must not be null.</param>
		/// <remarks>Both static and instance members will be processed.</remarks>
		public void AutoSetMembers(object instance)
		{
			MemberInfo[] members = instance.GetType().FindMembers(AutoSetMemberAttribute.SupportedMemberTypes, AutoSetMemberAttribute.SupportedBindingFlags, Type.FilterName, "*");

			foreach (MemberInfo member in members)
				AutoSetMembers(instance, member);
		}

		/// <summary>
		/// Automatically sets member of the provided class instance or <see cref="System.Type"/>.
		/// </summary>
		/// <param name="classToProcess">The class instance or <see cref="System.Type"/> to process.</param>
		/// <param name="member">The member which will be set. Must be a field or a property.</param>
		/// <remarks>Both static and instance members are accepted.</remarks>
		public void AutoSetMembers(object classToProcess, MemberInfo member)
		{
			AutoSetMemberAttribute attrib = Attribute.GetCustomAttribute(member, typeof(AutoSetMemberAttribute)) as AutoSetMemberAttribute;

			if (attrib != null)
			{
				if (attrib.ResID != null && AutoSetMemberAttribute.Resources != null)
					attrib.Aliases.Add(AutoSetMemberAttribute.Resources.GetString(attrib.ResID, AutoSetMemberAttribute.Culture));

				String argValue = null;
				bool found = false;

				foreach (String alias in attrib.Aliases)
				{
					if (m_unhandled.ContainsKey(alias))
					{
						argValue = (String)m_unhandled[alias];

						m_handled.Add(alias, argValue);
						m_unhandled.Remove(alias);

						found = true;
						break;
					}
					else if (m_handled.ContainsKey(alias))
					{
						argValue = (String)m_handled[alias];

						found = true;
						break;
					}
				}

				if (found)
				{
					Type memberType = null;

					switch (member.MemberType)
					{
						case MemberTypes.Property:
							memberType = ((PropertyInfo)member).PropertyType;
							break;

						case MemberTypes.Field:
							memberType = ((FieldInfo)member).FieldType;
							break;
					}

					if (memberType == typeof(bool))
					{
						if (argValue == "")
							SetMemberValue(classToProcess, member, !attrib.SwitchMeansFalse);
						else if (argValue == Boolean.FalseString || argValue == Boolean.TrueString)
							SetMemberValue(classToProcess, member, Boolean.Parse(argValue));
						else
							// last chance ... if can't parse it as integer, an exception will be raised
							SetMemberValue(classToProcess, member, Int32.Parse(argValue) != 0);
					}
					else if (memberType == typeof(String))
						SetMemberValue(classToProcess, member, argValue);
					else if (memberType.IsEnum)
					{
						object value = Enum.Parse(memberType, argValue, m_ignoreCase);
						SetMemberValue(classToProcess, member, value);
					}
					else if (memberType.IsValueType)
						SetMemberValue(classToProcess, member, Convert.ChangeType(argValue, memberType));
				}
			}
		}

		/// <summary>
		/// Sets the static or instance member (property or field) to the specified value.
		/// </summary>
		/// <param name="instance">The class instance or <see cref="System.Type"/> to be used.</param>
		/// <param name="memberInfo">The member to be set.</param>
		/// <param name="value">The new value of the member.</param>
		private void SetMemberValue(object instance, MemberInfo memberInfo, object value)
		{
			if (memberInfo is PropertyInfo)
			{
				PropertyInfo pi = (PropertyInfo) memberInfo;

				if (pi.CanWrite)
				{
					MethodInfo methodInfo = pi.GetSetMethod(true);

					BindingFlags bindingFlags = BindingFlags.SetProperty;

					if (methodInfo.IsStatic)
						bindingFlags |= BindingFlags.Static;

					pi.SetValue(instance, value, bindingFlags, null, null, null);
				}
			}
			else if (memberInfo is FieldInfo)
			{
				FieldInfo fi = (FieldInfo) memberInfo;

				BindingFlags bindingFlags = BindingFlags.SetField;

				if (fi.IsStatic)
					bindingFlags |= BindingFlags.Static;

				fi.SetValue(instance, value, bindingFlags, null, null);
			}
		}
		#endregion

		/// <summary>
		/// Clears all saved arguments (both handled and unhandled).
		/// </summary>
		public void Clear()
		{
			m_handled.Clear();
			m_unhandled.Clear();
		}
	}
}