// ProtocolParser.cs
//
//XMPP .NET Library Copyright (C) 2006, 2007, 2008 Dieter Lunn
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

#region Usings
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text;
using ubiety.logging;
using ubiety.common;
using ubiety.registries;
using ubiety.core;
using ubiety.states;
#endregion

namespace ubiety
{
	/// <remarks>
	/// The core of the library.  All messages come through here to be translated into the appropriate <see cref="Tag"/>
	/// </remarks>
	public class ProtocolParser
    {
        #region Private Members
		private static XmlNamespaceManager _ns;
		private static TagRegistry _reg = TagRegistry.Instance;
		private static ProtocolState _states = ProtocolState.Instance;
		private static XmlDocument _doc = new XmlDocument();
		private static XmlElement _elem;
		private static XmlElement _root = null;

		private static XmlReader _reader;
		private static XmlReaderSettings _settings;
		private static Decoder _decoder = Encoding.UTF8.GetDecoder();
        #endregion

		/// <summary>
		/// Parses the message into its appropriate <seealso cref="Tag"/>
		/// </summary>
		public static void Parse(byte[] m, int length)
		{
			char[] chars = new char[length];
            _decoder.GetChars(m, 0, length, chars, 0);
            string message = new string(chars);
            Logger.DebugFormat(typeof(ProtocolParser), "Incoming Message: {0}", message);

			// Moved the initialization into the parse method because it has become static.  Don't really need an instance to parse the string.
			Logger.Info(typeof(ProtocolParser), "Setting up environment");
			NameTable nt = new NameTable();
			_settings = new XmlReaderSettings();
			_settings.NameTable = nt;
			_settings.ConformanceLevel = ConformanceLevel.Fragment;
			_ns = new XmlNamespaceManager(nt);

			_ns.AddNamespace("stream", Namespaces.STREAM);

            Logger.Info(typeof(ProtocolParser), "Starting message parsing...");

			// We have received the end tag asking to finish communication so we change to the Disconnect State.
			if (message.Contains("</stream:stream>"))
			{
				Logger.Info(typeof(ProtocolParser), "End of stream received from server");
                _states.State = new DisconnectState();
                _states.Execute(null);
                return;
            }

			// We have to cheat because XmlTextReader doesn't like malformed XML
            if (message.Contains("<stream:stream"))
            {
            	Logger.Debug(typeof(ProtocolParser), "Adding closing tag so xml parser doesn't complain");
                message += "</stream:stream>";
            }
            
			XmlParserContext context = new XmlParserContext(null, _ns, null, XmlSpace.None);
            _reader = XmlReader.Create(new StringReader(message), _settings, context);
            try
            {
                while (_reader.Read())
                {
                    switch (_reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            StartTag();
                            if (_reader.IsEmptyElement)
                            {
                                EndTag();
                            }
                            break;
                        case XmlNodeType.EndElement:
                            EndTag();
                            break;
                        case XmlNodeType.Text:
                            AddText();
                            break;
                    }
                }
            }
            catch (XmlException e)
            {
                Logger.ErrorFormat(typeof(ProtocolParser), "Message Parsing Error: {0}", e);
            }
            catch (InvalidOperationException e)
            {
            	Logger.ErrorFormat(typeof(ProtocolParser), "Invalid Operation: {0}", e);
            }
		}

		private static void AddText()
		{
			if (_elem != null)
			{
				_elem.AppendChild(_doc.CreateTextNode(_reader.Value));
			}
		}

		private static void StartTag()
		{
			Hashtable ht = new Hashtable();

			if (_reader.HasAttributes)
			{
				while (_reader.MoveToNextAttribute())
				{
					if (_reader.Prefix.Equals("xmlns"))
					{
						//_ns.AddNamespace(_reader.LocalName, _reader.Value);
					}
					else if (_reader.Name.Equals("xmlns"))
					{
						//_ns.AddNamespace(string.Empty, _reader.Value);
					}
					else
					{
						ht.Add(_reader.Name, _reader.Value);
					}
				}
				_reader.MoveToElement();
			}

			string ns = _ns.LookupNamespace(_reader.Prefix);
			XmlQualifiedName q = new XmlQualifiedName(_reader.LocalName, ns);
			XmlElement elem = _reg.GetTag(_reader.Prefix, q, _doc);

			Logger.DebugFormat(typeof(ProtocolParser), "<{0}>", elem.Name);

			foreach (string attrname in ht.Keys)
			{
				int colon = attrname.IndexOf(':');
				if (colon > 0)
				{
					string prefix = attrname.Substring(0, colon);
					string name = attrname.Substring(colon + 1);

					XmlAttribute attr = _doc.CreateAttribute(prefix, name, _ns.LookupNamespace(prefix));
					attr.InnerXml = (string)ht[attrname];

					elem.SetAttributeNode(attr);
				}
				else
				{
					XmlAttribute attr = _doc.CreateAttribute(attrname);
					attr.InnerXml = (string)ht[attrname];

					elem.SetAttributeNode(attr);
				}
			}

//			Logger.DebugFormat(typeof(ProtocolParser), "<{0}>", elem.Name);

			if (_root == null)
			{
				_root = elem;
				// Changing ProtocolState to Singleton so this eliminates the events.
				// If the tag is a stream change to the Server Features State.
				_states.State = new ServerFeaturesState();
			}
			else
			{
				if (_elem != null)
				{
					_elem.AppendChild(elem);
				}
				_elem = elem;
			}
		}

		private static void EndTag()
		{
            if ((_elem == null) && (_reader.Name == "stream:stream"))
                return;

			if ((_elem.Name != _reader.Name))
			{
				Errors.Instance.SendError(typeof(ProtocolParser), ErrorType.AuthorizationFailed, "Wrong element");
                return;
			}
            
            Logger.DebugFormat(typeof(ProtocolParser), "</{0}>", _elem.Name);
            
            XmlElement parent = (XmlElement)_elem.ParentNode;
			if (parent == null)
			{
				Logger.Debug(typeof(ProtocolParser), "Top of tree. Executing current state.");
				ubiety.common.Tag tag = (ubiety.common.Tag)_elem;
				Logger.DebugFormat(typeof(ProtocolParser), "Current State: {0}", _states.State.ToString());
				_states.Execute(tag);
			}
			_elem = parent;
		}
	}
}
