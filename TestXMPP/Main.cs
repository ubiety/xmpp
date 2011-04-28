using System;
using System.Windows.Forms;
using ubiety;
using ubiety.common;
using ubiety.registries;
using System.Reflection;
using log4net.Config;
using log4net.Appender;

namespace TestXMPP
{
	public partial class Main : Form
	{
		XMPP _xmpp;

		public Main()
		{
			InitializeComponent();
			CompressionRegistry.Instance.AddCompression(Assembly.LoadFile(Application.StartupPath + @"\ubiety.compression.sharpziplib.dll"));
			Errors.Instance.OnError += Errors_OnError;
			XmlConfigurator.Configure();
			slVersion.Text = "Ubiety Version: " + XMPP.Version;
		}

		static void Errors_OnError(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message, "Protocol Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void Button1Click(object sender, EventArgs e)
		{
			_xmpp = new XMPP();
			UbietySettings.AuthenticationTypes = ubiety.core.MechanismType.Default;
			UbietySettings.Id = new JID(txtUsername.Text);
			UbietySettings.Password = txtPassword.Text;
			UbietySettings.SSL = btnSSL.Checked;
			_xmpp.Connect();
		}

		private void BtnExitClick(object sender, EventArgs e)
		{
			if (_xmpp != null)
				_xmpp.Disconnect();
			Application.Exit();
		}

		private void MainLoad(object sender, EventArgs e)
		{
			RichTextBoxAppender.SetRichTextBox(rtbDebug, "RichTextAppender");
		}
	}
}