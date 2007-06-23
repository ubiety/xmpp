using System;
using System.Collections.Generic;
using System.Text;

namespace xmpp.xml
{
    /// <summary>
    /// 
    /// </summary>
    public enum XmlTokenType
    {
        /// <summary>
        /// 
        /// </summary>
        XmlStartTag,
        /// <summary>
        /// 
        /// </summary>
        XmlEndTag,
        /// <summary>
        /// 
        /// </summary>
        XmlText,
        /// <summary>
        /// 
        /// </summary>
        XmlStartCDATA,
        /// <summary>
        /// 
        /// </summary>
        XmlEndCDATA,
        /// <summary>
        /// 
        /// </summary>
        XmlEntity,
        /// <summary>
        /// 
        /// </summary>
        XmlStartPI,
        /// <summary>
        /// 
        /// </summary>
        XmlEndPI
    }

    /// <summary>
    /// 
    /// </summary>
    public class XmlToken
    {
        String _token;
        XmlTokenType _type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public XmlToken(String token)
        {
            _token = token;
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlTokenType Type
        {
            get { return _type; }
        }
    }
}
