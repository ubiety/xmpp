Ubiety XMPP Library
===================

Ubiety is an extensible XMPP library written in C# to be easy and powerful.

Example
-------

```c#
using ubiety;
using ubiety.common;

public class Test {
    public static void Main() {
        Settings.ID = new JID("test@ubiety.ca");
        Settings.Password = "test";

        XMPP ubiety = new XMPP();
        ubiety.Connect();
    }
}
```

Support
-------

* Mailing List: <xmppnet-devel@googlegroups.com>
* Issues: <http://jira.ubiety.ca/jira/>