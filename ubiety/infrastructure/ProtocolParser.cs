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
using Serilog;
using Ubiety.Common;
using Ubiety.Common.Exceptions;
using Ubiety.Registries;
using Ubiety.States;

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
            Settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
            NamespaceManager = new XmlNamespaceManager(Tag.Document.NameTable);

            NamespaceManager.AddNamespace("", Namespaces.Client);
            NamespaceManager.AddNamespace("stream", Namespaces.Stream);
        }

        /// <summary>
        ///     Parses the message into its appropriate <seealso cref="Tag" />
        /// </summary>
        public static void Parse(string message)
        {
            bool fullStream = false;

            if (ProtocolState.State is DisconnectedState)
            {
                return;
            }

            // We have received the end tag asking to finish communication so we change to the Disconnect State.
            if (message.Contains("</stream:stream>"))
            {
                // Just close the socket.  We don't need to reply but we will signal we aren't connected.
                ProtocolState.State = new DisconnectedState();
                ProtocolState.State.Execute();

                if (message.Length == 16)
                {
                    return;
                }

                fullStream = true;
            }

            // We have to cheat because XmlTextReader doesn't like malformed XML
            if (message.Contains("<stream:stream"))
            {
                if (!fullStream)
                {
                    message += "</stream:stream>";
                }
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
                Log.Error(e, "Error in xml from server");
                ProtocolState.Events.Error(null, ErrorType.XmlError, ErrorSeverity.Fatal, "Error parsing XML from server.");
                if (ProtocolState.Socket.Connected)
                {
                    ProtocolState.State = new DisconnectState();
                    ProtocolState.State.Execute();
                }

                throw new ServerXmlException("Error in xml from server", e);
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e, "Invalid operation parsing incoming message.");

                throw;
            }
        }

        private static void AddText()
        {
            _element?.AppendChild(Tag.Document.CreateTextNode(_reader.Value));
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

                    XmlAttribute attr = Tag.Document.CreateAttribute(prefix, name,
                        NamespaceManager.LookupNamespace(prefix));
                    attr.InnerXml = (string) ht[attrname];

                    elem.SetAttributeNode(attr);
                }
                else
                {
                    XmlAttribute attr = Tag.Document.CreateAttribute(attrname);
                    attr.InnerXml = (string) ht[attrname];

                    elem.SetAttributeNode(attr);
                }
            }

            if (_root == null)
            {
                if (elem.Name != "stream:stream")
                {
                    ProtocolState.Events.Error(null, ErrorType.WrongProtocolVersion, ErrorSeverity.Fatal, "Missing proper stream:stream header from server.");
                    return;
                }

                _root = elem;
            }
            else
            {
                _element?.AppendChild(elem);
                _element = elem;
            }
        }

        private static void EndTag()
        {
            if (_element == null)
                return;

            if ((_element.Name != _reader.Name))
            {
                ProtocolState.Events.Error(null, ErrorType.XmlError, ErrorSeverity.Fatal, "Wrong end tag for current element.");
                return;
            }

            var parent = (XmlElement) _element.ParentNode;
            if (parent == null)
            {
                ProtocolState.Events.NewTag(null, (Tag) _element);
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