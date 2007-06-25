//XMPP .NET Library Copyright (C) 2006 Dieter Lunn

//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 2.1 of the License, or (at your option)
//any later version.

//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//details.

//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections;
using System.IO;
using System.Xml;
using log4net;
using log4net.Config;
using xmpp.common;

namespace xmpp
{
	/// <remarks>
	/// Provides data for the Tag events
	/// </remarks>
	public class TagEventArgs : EventArgs
	{
		private Tag _tag;

		/// <summary>
		/// Initializes a new instance of the <see cref="TagEventArgs"/> class.
		/// </summary>
		/// <param name="tag"></param>
		public TagEventArgs(Tag tag)
		{
			_tag = tag;
		}

		/// <summary>
		/// Gets the tag parsed from the incoming stream.
		/// </summary>
		public Tag Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}
	}
	
    /// <remarks>
    /// The core of the library.  All messages come through here to be translated into the appropriate <see cref="Tag"/>
    /// </remarks>
	public class ProtocolParser
	{
        /// <summary>
        /// Occurs when the first <seealso cref="Tag"/> is seen on the stream.
        /// </summary>
		public event EventHandler<TagEventArgs> StreamStart;

        /// <summary>
        /// Occurs when the last <seealso cref="Tag"/> is seen on the stream.
        /// </summary>
		public event EventHandler StreamEnd;

        /// <summary>
        /// Occurs when any <seealso cref="Tag"/> is seen.
        /// </summary>
		public event EventHandler<TagEventArgs> Tag;

		private XmlNamespaceManager _ns;
		private TagRegistry _reg = TagRegistry.Instance;
		private XmlDocument _doc = new XmlDocument();
		private XmlElement _elem;
		private XmlElement _root;

        private XmlReader _reader;
    	private XmlReaderSettings _settings;

        private static readonly ILog logger = LogManager.GetLogger(typeof(ProtocolParser));

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolParser"/> class.
        /// </summary>
		public ProtocolParser()
		{
            XmlConfigurator.Configure();
			NameTable nt = new NameTable();
			_settings = new XmlReaderSettings();
        	_settings.NameTable = nt;
        	_settings.ConformanceLevel = ConformanceLevel.Fragment;
			_ns = new XmlNamespaceManager(nt);

			_ns.AddNamespace("stream", Namespaces.STREAM);
		}

        /// <summary>
        /// Parses the message into its appropriate <seealso cref="Tag"/>
        /// </summary>
		public void Parse(String message)
		{
            logger.Info("Starting message parsing...");


            // We have to cheat because XmlTextReader doesn't like malformed XML
            if (message.Contains("</stream:stream>"))
            {
                OnDocumentEnd();
                return;
            }

            if (message.Contains("<stream:stream"))
            {
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
                logger.Error("Message Parsing Error: ", e);
            }
            catch (InvalidOperationException)
            {
            }
		}

		private void AddText()
		{
			if (_elem != null)
			{
				_elem.AppendChild(_doc.CreateTextNode(_reader.Value));
			}
		}

		private void StartTag()
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

            logger.Debug("Start: " + elem.Name);

            if (_root == null)
            {
                _root = elem;
                OnDocumentStart(_root);
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

		private void EndTag()
		{
            if (_reader.Name == "stream:stream")
            {
                return;
            }

			if (_elem.Name != _reader.Name)
			{
				throw new XmlException();
			}

            logger.Debug("End: " + _elem);
            
            XmlElement parent = (XmlElement)_elem.ParentNode;
			if (parent == null)
			{
				OnElement(_elem);
			}
			_elem = parent;
		}

		private void OnElement(XmlElement tag)
		{
			if (Tag != null)
			{
				Tag(this, new TagEventArgs((Tag)tag));
			}
		}

		private void OnDocumentEnd()
		{
			if (StreamEnd != null)
			{
				StreamEnd(this, EventArgs.Empty);
			}
		}

		private void OnDocumentStart(XmlElement elem)
		{
			if (StreamStart != null)
			{
				StreamStart(this, new TagEventArgs((Tag)elem));
			}
		}
	}
}
