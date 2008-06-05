//XMPP .NET Library Copyright (C) 2006 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using Gtk;
using GLib;
using xmpp;
using xmpp.common;
using xmpp.logging;
using System.Security.Cryptography.X509Certificates;

public partial class MainWindow: Gtk.Window
{	
	private XMPP xmpp;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		xmpp = new XMPP();
		//xmpp.LocalCertificate = X509Certificate.CreateFromCertFile("cert.pem");
		ExceptionManager.UnhandledException += new UnhandledExceptionHandler(OnExceptionEvent);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnExceptionEvent(UnhandledExceptionArgs u)
	{
		Exception e = u.ExceptionObject as Exception;
		Logger.ErrorFormat(this, "Unhandled Exception: {0}", e.Message);
	}

	protected virtual void OnConnect (object sender, System.EventArgs e)
	{
		xmpp.ID = new XID(xid.Text);
		xmpp.Password = password.Text;
		xmpp.SSL = cbSSL.Active;
		xmpp.Connect();
	}
}