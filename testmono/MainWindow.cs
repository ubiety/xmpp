//XMPP .NET Library Copyright (C) 2006, 2008, 2009 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.//
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
using ubiety;
using ubiety.common;
using ubiety.logging;
using ubiety.registries;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

public partial class MainWindow: Gtk.Window
{	
	private XMPP xmpp;
	private Errors error = Errors.Instance;
	//private CompressionRegistry _creg = CompressionRegistry.Instance;
	private Boolean ssl = false;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		xmpp = new XMPP();
		error.OnError += new EventHandler<ErrorEventArgs>(OnError);
		//ubiety.LocalCertificate = X509Certificate.CreateFromCertFile("cert.pem");
		//ExceptionManager.UnhandledException += new UnhandledExceptionHandler(OnExceptionEvent);
		
		//Disabling compression until it can be fixed.
		//_creg.AddCompression(Assembly.LoadFile("ubiety.compression.dotnetzip.dll"));
		statusbar1.Push(1, "Ubiety Version: " + xmpp.Version);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		xmpp.Disconnect();
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnExceptionEvent(UnhandledExceptionArgs u)
	{
		Exception e = u.ExceptionObject as Exception;
		Logger.ErrorFormat(this, "Unhandled Exception: {0}", e.Message);
	}

	protected virtual void OnConnect(object sender, System.EventArgs e)
	{
		if (((Gtk.ToggleAction)sender).Active)
		{
			xmpp.ID = new XID(xid.Text);
			xmpp.Password = password.Text;
			xmpp.SSL = ssl;
			xmpp.Connect();
			string str = "Connecting to " + xmpp.ID.Server;
			if (ssl)
			{
				str += " with SSL";
			}
			statusbar1.Push(2, str);
		}
		else
		{
			xmpp.Disconnect();
		}
	}
	
	protected void OnError(object sender, ErrorEventArgs e)
	{
		MessageDialog d = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, "");
		d.Text = e.Message;
		d.Run();
		xmpp.Disconnect();
		d.Destroy();
	}

	protected virtual void OnSSL (object sender, System.EventArgs e)
	{
		ssl = SSLAction.Active;
	}

	protected virtual void OnQuit (object sender, System.EventArgs e)
	{
		if ( xmpp.Connected )
		xmpp.Disconnect();
		Application.Quit();
	}
}