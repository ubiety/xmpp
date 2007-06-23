using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace xmpp.xml
{
    /// <summary>
    /// XmlTokenParser is the custom parser used by this library.
    /// </summary>
    public class XmlTokenParser
    {
        Regex _reg = new Regex(@"<|>");
        IEnumerator<String> _tokens;

        /// <summary>
        /// Creates a new instance of the XmlTokenParser
        /// </summary>
        /// <param name="xml">The xml to parse.</param>
        public XmlTokenParser(String xml)
        {
            String[] tokens = _reg.Split(xml);
            _tokens = (IEnumerator<String>)tokens.GetEnumerator();
        }

        /// <summary>
        /// Gets the next element from the parsed message.
        /// </summary>
        /// <returns>XmlToken</returns>
        public XmlToken Next()
        {
            _tokens.MoveNext();
            while (_tokens.Current == "")
            {
                _tokens.MoveNext();
            }
            return new XmlToken(_tokens.Current);
        }
    }
}
