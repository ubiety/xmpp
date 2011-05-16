using System;
using System.Reflection;
using System.Windows.Forms;
using ubiety;
using ubiety.common;
using ubiety.core;
using ubiety.registries;

namespace TestXMPP
{
	public partial class Main : Form
	{
		private XMPP _xmpp;

		public Main()
		{
			InitializeComponent();
			CompressionRegistry.Instance.AddCompression(Assembly.LoadFile(Application.StartupPath + @"\ubiety.compression.sharpziplib.dll"));
			Errors.Instance.OnError += Errors_OnError;
			slVersion.Text = "Ubiety Version: " + XMPP.Version;
		}

		private static void Errors_OnError(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message, "Protocol Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void Button1Click(object sender, EventArgs e)
		{
			_xmpp = new XMPP();
			UbietySettings.AuthenticationTypes = MechanismType.Default;
			UbietySettings.Id = new JID(txtUsername.Text);
			UbietySettings.Password = txtPassword.Text;
			UbietySettings.SSL = btnSSL.Checked;
			_xmpp.Connect();
		}

		private void BtnExitClick(object sender, EventArgs e)
		{
			if (_xmpp != null && _xmpp.Connected)
				_xmpp.Disconnect();
			Application.Exit();
		}
	}
}