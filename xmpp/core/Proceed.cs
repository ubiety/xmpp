using System;
using System.Xml;

using xmpp.common;

namespace xmpp.core
{
    [XmppTag("proceed", xmpp.common.Namespaces.START_TLS, typeof(Proceed))]
    public class Proceed : Tag
    {
        public Proceed(String prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        {
        }
    }
}
