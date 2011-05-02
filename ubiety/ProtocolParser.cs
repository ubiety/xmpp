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
using ubiety.common;
using ubiety.registries;
using ubiety.states;
using Stream = ubiety.core.Stream;

#endregion

namespace ubiety
{
	/// <remarks>
	/// The core of the library.  All messages come through here to be translated into the appropriate <see cref="Tag"/>
	/// </remarks>
	internal class ProtocolParser
	{
		private static XmlNamespaceManager _ns;
		private static readonly TagRegistry Reg = TagRegistry.Instance;
		private static readonly ProtocolState States = ProtocolState.Instance;
		private static XmlElement _elem;
		private static XmlElement _root;

		private static XmlReader _reader;
		private static XmlReaderSettings _settings;

		/// <summary>
		/// Parses the message into its appropriate <seealso cref="Tag"/>
		/// </summary>
		public static void Parse(string message, int length)
		{
			if (States.State is ClosedState)
			{
				Logger.Debug(typeof (ProtocolParser), "Closed.  Nothing to do");
				return;
			}

			//Logger.DebugFormat(typeof(ProtocolParser), "Incoming Message: {0}", message);

			// Moved the initialization into the parse method because it has become static.  Don't really need an instance to parse the string.
			Logger.Info(typeof (ProtocolParser), "Setting up environment");
			_settings = new XmlReaderSettings();
			_settings.ConformanceLevel = ConformanceLevel.Fragment;
			_ns = new XmlNamespaceManager(States.Document.NameTable);

			_ns.AddNamespace("", Namespaces.Client);
			_ns.AddNamespace("stream", Namespaces.Stream);

			Logger.Info(typeof (ProtocolParser), "Starting message parsing...");

			Logger.InfoFormat(typeof (ProtocolParser), "Current State: {0}", States.State);

			// We have received the end tag asking to finish communication so we change to the Disconnect State.
			if (message.Contains("</stream:stream>"))
			{
				Logger.Info(typeof (ProtocolParser), "End of stream received from server");
				// Just close the socket.  We don't need to reply but we will signal we aren't connected.
				States.State = new ClosedState();
				States.Execute();
				return;
			}

			// We have to cheat because XmlTextReader doesn't like malformed XML
			if (message.Contains("<stream:stream"))
			{
				Logger.Debug(typeof (ProtocolParser), "Adding closing tag so xml parser doesn't complain");
				message += "</stream:stream>";
			}

			var context = new XmlParserContext(null, _ns, null, XmlSpace.None);
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
				Logger.ErrorFormat(typeof (ProtocolParser), "Message Parsing Error: {0}", e);
				Errors.Instance.SendError(typeof (ProtocolParser), ErrorType.XMLError,
				                          "Error parsing incoming XML.  Please try again.");
				if (States.Socket.Connected)
				{
					States.State = new DisconnectState();
					States.Execute();
				}
			}
			catch (InvalidOperationException e)
			{
				Logger.ErrorFormat(typeof (ProtocolParser), "Invalid Operation: {0}", e);
			}
		}

		private static void AddText()
		{
			if (_elem != null)
			{
				_elem.AppendChild(States.Document.CreateTextNode(_reader.Value));
			}
		}

		private static void StartTag()
		{
			var ht = new Hashtable();

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

			var ns = _ns.LookupNamespace(_reader.Prefix);
			var q = new XmlQualifiedName(_reader.LocalName, ns);
			XmlElement elem = Reg.GetTag(q, States.Document);

			//Logger.DebugFormat(typeof(ProtocolParser), "<{0}>", elem.Name);

			foreach (string attrname in ht.Keys)
			{
				var colon = attrname.IndexOf(':');
				if (colon > 0)
				{
					var prefix = attrname.Substring(0, colon);
					var name = attrname.Substring(colon + 1);

					var attr = States.Document.CreateAttribute(prefix, name, _ns.LookupNamespace(prefix));
					attr.InnerXml = (string) ht[attrname];

					elem.SetAttributeNode(attr);
				}
				else
				{
					var attr = States.Document.CreateAttribute(attrname);
					attr.InnerXml = (string) ht[attrname];

					elem.SetAttributeNode(attr);
				}
			}

			if (_root == null)
			{
				if (elem.Name != "stream:stream")
				{
					Errors.Instance.SendError(typeof (ProtocolParser), ErrorType.WrongProtocolVersion,
					                          "Missing stream:stream from server");
					return;
				}

				_root = elem;
				// If the tag is a stream change to the Server Features State.
				States.State = new ServerFeaturesState();
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
				Errors.Instance.SendError(typeof (ProtocolParser), ErrorType.XMLError, "Wrong element");
				return;
			}

			//Logger.DebugFormat(typeof(ProtocolParser), "</{0}>", _elem.Name);

			var parent = (XmlElement) _elem.ParentNode;
			if (parent == null)
			{
				//Logger.Debug(typeof(ProtocolParser), "Top of tree. Executing current state.");
				var tag = (Tag) _elem;
				if (tag is Stream)
					States.State = new ServerFeaturesState();
				Logger.DebugFormat(typeof (ProtocolParser), "Current State: {0}", States.State.ToString());
				if (tag.Name == "error")
				{
					Errors.Instance.SendError(typeof (ProtocolParser), ErrorType.XMLError, "Stream Error", true);
				}
				else
				{
					States.Execute(tag);
				}
			}

			//Logger.Debug(typeof(ProtocolParser), "Not at top yet. Continuing the parser.");
			_elem = parent;
		}

		public static void Reset()
		{
			_root = null;
			States.Authenticated = false;
		}
	}
}