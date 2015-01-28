using System;
using System.Windows.Forms;
using TestXMPP.Properties;
using Ubiety;
using Ubiety.Common;
using Ubiety.Core;
using ErrorEventArgs = Ubiety.ErrorEventArgs;

namespace TestXMPP
{
	public partial class Main : Form
	{
		private readonly Xmpp _xmpp;

		public Main()
		{
			InitializeComponent();
            //CompressionRegistry.AddCompression(Assembly.LoadFile(Path.Combine(Application.StartupPath, "ubiety.compression.sharpziplib.dll")));
			Errors.OnError += Errors_OnError;
			_xmpp = new Xmpp(); 
			slVersion.Text = Resources.Version_Label + Xmpp.Version;
		}

		private static void Errors_OnError(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message, Resources.Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void Button1Click(object sender, EventArgs e)
		{
            _xmpp.Settings.AuthenticationTypes = MechanismType.Default;
            _xmpp.Settings.Id = new JID(txtUsername.Text);
			_xmpp.Settings.Password = txtPassword.Text;
			_xmpp.Settings.Ssl = btnSSL.Checked;
			_xmpp.Connect();
		}

		private void BtnExitClick(object sender, EventArgs e)
		{
			if (_xmpp != null && _xmpp.Connected)
				_xmpp.Disconnect();
			Application.Exit();
		}

		private void BtnSendClick(object sender, EventArgs e)
		{
			UbietyMessages.SendMessage(txtMessage.Text);
		}
	}
}