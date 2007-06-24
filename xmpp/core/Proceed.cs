using System;
using System.Xml;

using xmpp.common;

namespace xmpp.core
{
    ///<summary>
    ///</summary>
    [XmppTag("proceed", xmpp.common.Namespaces.START_TLS, typeof(Proceed))]
    public class Proceed : Tag
    {
        ///<summary>
        ///</summary>
        ///<param name="prefix"></param>
        ///<param name="qname"></param>
        ///<param name="doc"></param>
        public Proceed(String prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        {
        }
    }
}
