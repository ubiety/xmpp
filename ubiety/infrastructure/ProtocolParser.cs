// ProtocolParser.cs
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

#region Usings

using System;
using System.Collections;
using System.IO;
using System.Xml;
using ubiety;
using Ubiety.Common;
using ubiety.infrastructure.logging;
using ubiety.registries;
using ubiety.states;

#endregion

namespace Ubiety.Infrastructure
{
    /// <remarks>
    ///     The core of the library.  All messages come through here to be translated into the appropriate <see cref="Tag" />
    /// </remarks>
    internal static class ProtocolParser
    {
        private static readonly XmlNamespaceManager NamespaceManager;
        private static XmlElement _element;
        private static XmlElement _root;

        private static XmlReader _reader;
        private static readonly XmlReaderSettings Settings;

        static ProtocolParser()
        {
            Logger.Info(typeof (ProtocolParser), "Setting up environment");
            Settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
            NamespaceManager = new XmlNamespaceManager(ProtocolState.Document.NameTable);

            NamespaceManager.AddNamespace("", Namespaces.Client);
            NamespaceManager.AddNamespace("stream", Namespaces.Stream);
        }

        /// <summary>
        ///     Parses the message into its appropriate <seealso cref="Tag" />
        /// </summary>
        public static void Parse(string message)
        {
            if (ProtocolState.State is DisconnectedState)
            {
                Logger.Debug(typeof (ProtocolParser), "Closed.  Nothing to do");
                return;
            }

            Logger.Info(typeof (ProtocolParser), "Starting message parsing...");

            // We have received the end tag asking to finish communication so we change to the Disconnect State.
            if (message.Contains("</stream:stream>"))
            {
                Logger.Info(typeof (ProtocolParser), "End of stream received from server");
                // Just close the socket.  We don't need to reply but we will signal we aren't connected.
                ProtocolState.State = new DisconnectedState();
                ProtocolState.State.Execute();
                return;
            }

            // We have to cheat because XmlTextReader doesn't like malformed XML
            if (message.Contains("<stream:stream"))
            {
                Logger.Debug(typeof (ProtocolParser), "Adding closing tag so xml parser doesn't complain");
                message += "</stream:stream>";
            }

            var context = new XmlParserContext(null, NamespaceManager, null, XmlSpace.None);
            _reader = XmlReader.Create(new StringReader(message), Settings, context);
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
                Errors.SendError(typeof (ProtocolParser), ErrorType.XmlError,
                    "Error parsing incoming XML.  Please try again.");
                if (ProtocolState.Socket.Connected)
                {
                    ProtocolState.State = new DisconnectState();
                    ProtocolState.State.Execute();
                }
            }
            catch (InvalidOperationException e)
            {
                Logger.ErrorFormat(typeof (ProtocolParser), "Invalid Operation: {0}", e);
            }
        }

        private static void AddText()
        {
            if (_element != null)
            {
                _element.AppendChild(ProtocolState.Document.CreateTextNode(_reader.Value));
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
                        NamespaceManager.AddNamespace(_reader.LocalName, _reader.Value);
                    }
                    else if (_reader.Name.Equals("xmlns"))
                    {
                        NamespaceManager.AddNamespace(string.Empty, _reader.Value);
                    }
                    else
                    {
                        ht.Add(_reader.Name, _reader.Value);
                    }
                }
                _reader.MoveToElement();
            }

            string ns = NamespaceManager.LookupNamespace(_reader.Prefix);
            var q = new XmlQualifiedName(_reader.LocalName, ns);
            XmlElement elem = TagRegistry.GetTag<Tag>(q);

            foreach (string attrname in ht.Keys)
            {
                int colon = attrname.IndexOf(':');
                if (colon > 0)
                {
                    string prefix = attrname.Substring(0, colon);
                    string name = attrname.Substring(colon + 1);

                    XmlAttribute attr = ProtocolState.Document.CreateAttribute(prefix, name, NamespaceManager.LookupNamespace(prefix));
                    attr.InnerXml = (string) ht[attrname];

                    elem.SetAttributeNode(attr);
                }
                else
                {
                    XmlAttribute attr = ProtocolState.Document.CreateAttribute(attrname);
                    attr.InnerXml = (string) ht[attrname];

                    elem.SetAttributeNode(attr);
                }
            }

            if (_root == null)
            {
                if (elem.Name != "stream:stream")
                {
                    Errors.SendError(typeof (ProtocolParser), ErrorType.WrongProtocolVersion,
                        "Missing stream:stream from server");
                    return;
                }

                _root = elem;
            }
            else
            {
                if (_element != null)
                {
                    _element.AppendChild(elem);
                }
                _element = elem;
            }
        }

        private static void EndTag()
        {
            if (_element == null)
                return;

            if ((_element.Name != _reader.Name))
            {
                Errors.SendError(typeof (ProtocolParser), ErrorType.XmlError, "Wrong element");
                return;
            }

            var parent = (XmlElement) _element.ParentNode;
            if (parent == null)
            {
                Logger.InfoFormat(typeof (ProtocolParser), "Current State: {0}", ProtocolState.State);
                UbietyMessages.OnAllMessages(new MessageEventArgs {Tag = (Tag) _element});
            }
            _element = parent;
        }

        public static void Reset()
        {
            _root = null;
            ProtocolState.Authenticated = false;
        }
    }
}