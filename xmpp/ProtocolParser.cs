// ProtocolParser.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2011 Dieter Lunn
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
	internal class ProtocolParser
	{
		#region << Private Members >>
		private static XmlNamespaceManager _ns;
		private static TagRegistry _reg = TagRegistry.Instance;
		private static ProtocolState _states = ProtocolState.Instance;
		private static XmlElement _elem;
		private static XmlElement _root = null;

		private static XmlReader _reader;
		private static XmlReaderSettings _settings;
		#endregion

		/// <summary>
		/// Parses the message into its appropriate <seealso cref="Tag"/>
		/// </summary>
		public static void Parse(string message, int length)
		{
			if (_states.State.GetType() == typeof(ClosedState))
			{
				Logger.Debug(typeof(ProtocolParser), "Closed.  Nothing to do");
				return;
			}

			//Logger.DebugFormat(typeof(ProtocolParser), "Incoming Message: {0}", message);

			// Moved the initialization into the parse method because it has become static.  Don't really need an instance to parse the string.
			Logger.Info(typeof(ProtocolParser), "Setting up environment");
			_settings = new XmlReaderSettings();
			_settings.ConformanceLevel = ConformanceLevel.Fragment;
			_ns = new XmlNamespaceManager(_states.Document.NameTable);
			
			_ns.AddNamespace("", Namespaces.CLIENT);
			_ns.AddNamespace("stream", Namespaces.STREAM);

			Logger.Info(typeof(ProtocolParser), "Starting message parsing...");

			Logger.InfoFormat(typeof(ProtocolParser), "Current State: {0}", _states.State);

			// We have received the end tag asking to finish communication so we change to the Disconnect State.
			if (message.Contains("</stream:stream>"))
			{
				Logger.Info(typeof(ProtocolParser), "End of stream received from server");
				// Just close the socket.  We don't need to reply but we will signal we aren't connected.
				_states.State = new ClosedState();
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
				Errors.Instance.SendError(typeof(ProtocolParser), ErrorType.XMLError, "Error parsing incoming XML.  Please try again.");
				if (_states.Socket.Connected)
				{
					_states.State = new DisconnectState();
					_states.Execute();
				}				
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
				_elem.AppendChild(_states.Document.CreateTextNode(_reader.Value));
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
						_ns.AddNamespace(_reader.LocalName, _reader.Value);
					}
					else if (_reader.Name.Equals("xmlns"))
					{
						_ns.AddNamespace(string.Empty, _reader.Value);
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
			XmlElement elem = _reg.GetTag(q, _states.Document);

			//Logger.DebugFormat(typeof(ProtocolParser), "<{0}>", elem.Name);

			foreach (string attrname in ht.Keys)
			{
				int colon = attrname.IndexOf(':');
				if (colon > 0)
				{
					string prefix = attrname.Substring(0, colon);
					string name = attrname.Substring(colon + 1);

					XmlAttribute attr = _states.Document.CreateAttribute(prefix, name, _ns.LookupNamespace(prefix));
					attr.InnerXml = (string)ht[attrname];

					elem.SetAttributeNode(attr);
				}
				else
				{
					XmlAttribute attr = _states.Document.CreateAttribute(attrname);
					attr.InnerXml = (string)ht[attrname];

					elem.SetAttributeNode(attr);
				}
			}

			if (_root == null)
			{
				if (elem.Name != "stream:stream")
				{
					Errors.Instance.SendError(typeof(ProtocolParser), ErrorType.WrongProtocolVersion, "Missing stream:stream from server");
					return;
				}

				_root = elem;
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
				Errors.Instance.SendError(typeof(ProtocolParser), ErrorType.XMLError, "Wrong element");
				return;
			}
			
			//Logger.DebugFormat(typeof(ProtocolParser), "</{0}>", _elem.Name);
			
			XmlElement parent = (XmlElement)_elem.ParentNode;
			if (parent == null)
			{
				//Logger.Debug(typeof(ProtocolParser), "Top of tree. Executing current state.");
				ubiety.common.Tag tag = (ubiety.common.Tag)_elem;
				if (tag is ubiety.core.Stream)
					_states.State = new ServerFeaturesState();
				Logger.DebugFormat(typeof(ProtocolParser), "Current State: {0}", _states.State.ToString());
				if (tag is ubiety.core.Error)
				{
					Errors.Instance.SendError(typeof(ProtocolParser), ErrorType.XMLError, "Stream Error", true);
				}
				else
				{
					_states.Execute(tag);
				}
			}
			
			//Logger.Debug(typeof(ProtocolParser), "Not at top yet. Continuing the parser.");
			_elem = parent;
		}
		
		public static void Reset()
		{
			_root = null;
			_states.Authenticated = false;
		}
	}
}
