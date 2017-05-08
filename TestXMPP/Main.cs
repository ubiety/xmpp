using System;
using System.Windows.Forms;
using TestXMPP.Properties;
using Ubiety;
using Ubiety.Common;
using Ubiety.Core;
using Ubiety.Infrastructure;
using Ubiety.States;

namespace TestXMPP
{
    public partial class Main : Form
    {
        private readonly Xmpp _xmpp;
        private const string fcm_server = "fcm-xmpp.googleapis.com";
        private const int fcm_dev_port = 5236;
        private const int fcm_prod_port = 5235;

        public Main()
        {
            InitializeComponent();
            //CompressionRegistry.AddCompression(Assembly.LoadFile(Path.Combine(Application.StartupPath, "ubiety.compression.sharpziplib.dll")));
            _xmpp = new Xmpp();
            _xmpp.OnError += _xmpp_OnError;
            slVersion.Text = Resources.Version_Label + Xmpp.Version;
        }

        void _xmpp_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, Resources.Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		private void Button1Click(object sender, EventArgs e)
		{
            Xmpp.Settings.AuthenticationTypes = MechanismType.Default | MechanismType.Plain;
            Xmpp.Settings.Id = new JID(txtUsername.Text);
			Xmpp.Settings.Password = txtPassword.Text;
		    Xmpp.Settings.Hostname = fcm_server;
		    Xmpp.Settings.Port = fcm_dev_port;
			Xmpp.Settings.Ssl = btnSSL.Checked;
            Xmpp.Settings.UseIPv6 = false;
            ProtocolState.Settings.Ssl = true;
			_xmpp.Connect();
		}

        private void BtnExitClick(object sender, EventArgs e)
        {
            if (_xmpp != null && Xmpp.Connected)
                _xmpp.Disconnect();
            Application.Exit();
        }

        private void BtnSendClick(object sender, EventArgs e)
        {
            //UbietyMessages.SendMessage(txtMessage.Text);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Serilog.Log.Debug("Disconnecting from server");
            if (_xmpp != null && Xmpp.Connected)
                _xmpp.Disconnect();
        }
    }
}