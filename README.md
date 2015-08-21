Ubiety XMPP Library
===================

Ubiety is an extensible XMPP library written in C# to be easy and powerful.

[![Build status](https://ci.appveyor.com/api/projects/status/3b7lbwih7rgmsc7a?svg=true)](https://ci.appveyor.com/project/coder2000/xmpp)

Installing
----------

Install from NuGet using: Install-Package UbietyXMPP

Example
-------

```c#
using Ubiety;
using Ubiety.Common;

public class Test {
    public static void Main() {
        Xmpp ubiety = new Xmpp();
        ubiety.Settings.ID = new JID("test@ubiety.ca");
        ubiety.Settings.Password = "test";
        
        ubiety.Connect();
    }
}
```

Support
-------

* Forum: <http://discourse.dieterlunn.ca>
