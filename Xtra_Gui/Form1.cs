using System;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.Collections;
using System.Web;

namespace WindowsFormsApplication1
{
    public delegate void AddItemDelegate(string captcha, string token);

    public partial class XtraDesktop : Form
    {

        public DateTime cookieValidUntil = DateTime.Now;

        public XtraDesktop()
        {
            InitializeComponent();
            Username.Text = Properties.Settings.Default.Username;
            Password.Text = Properties.Settings.Default.Passwort;
            this.httpObj = new XtraClient.httpModule();
        }

        public XtraClient.httpModule httpObj;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.checkInput()) {
                    return;
                }
                if (this.cookieValidUntil <= DateTime.Now)
                {
                    state.Text = "Loading Captcha...";
                    login();
                    return;
                }
                state.Text = "Sending...";
                doSend();
                state.Text = "SMS Sent!";
                this.cookieValidUntil = DateTime.Now.AddMinutes(15);
            }
            catch (WebException)
            {
                Cursor.Current = Cursors.Default;
                button1.Enabled = true;
                Status.Text = "Fehler";
            }
        }

        private void login() {
            state.Text = "Logging in...";
            LoginForm f2 = new LoginForm(""); // do not convert implicitly
            f2.AddItemCallback = new AddItemDelegate(this.AddItemCallbackFn);
            f2.Show();
        }

        private void AddItemCallbackFn(string captcha, string token)
        {
            Cursor.Current = Cursors.WaitCursor;
            state.Text = "Sending...";
            getLoginCookie(captcha, token);
            doSend();
            state.Text = "SMS Sent!";
        }

        private void Nachricht_TextChanged(object sender, EventArgs e)
        {
            Counter.Text = Convert.ToString(440 - Nachricht.Text.Length) + " Digits";
            if (Nachricht.Text.Length > 440)
            {
                Nachricht.ForeColor = System.Drawing.Color.Red;
            }
            else {
                Nachricht.ForeColor = System.Drawing.Color.Black;
            }
        }

        public bool getLoginCookie(string passphrase, string token) {
            string username, pw, postData, url, response;
            username = Properties.Settings.Default.Username;
            pw = Properties.Settings.Default.Passwort;

            postData = "action=ssoLogin&do_sso_login=1&passphrase=" + passphrase + "&sso_password=" + pw + "&sso_user=" + username + "&token=" + token + "&redirect=/index.php/22/de/home_public/";
            url = "https://xtrazone.sso.bluewin.ch/index.php/22,39,ajax_json,,,157/";

            response = this.httpObj.doHTTP(url, postData, true);
            this.cookieValidUntil = DateTime.Now.AddMinutes(15);
            return true;
        }

        public bool doSend() {
            string recipient = Empfaenger.Text;
            string message = HttpUtility.UrlEncode(Nachricht.Text);
            string postData = "receiversnames=" + recipient + "&recipients=%5B%5D&messagebody=" + message + "&attachments=&attachmentId=";
            string url = "https://xtrazone.sso.bluewin.ch/index.php/20,53,ajax,,,283/?route=%2Fmessaging%2Foutbox%2Fsendmobilemsg";
            string response = this.httpObj.doHTTP(url, postData, false);
            Hashtable jsonHash = (Hashtable)Procurios.Public.JSON.JsonDecode(response);
            ArrayList jsonResults = (ArrayList)jsonHash["results"];
            Debug.Write(response);
            listBox1.Items.Add(Empfaenger.Text + " : " + Nachricht.Text);
            return true;
        }

            public Boolean checkInput()
            {
                if (Properties.Settings.Default.Username.Length <= 0
                            || Properties.Settings.Default.Passwort.Length <= 0)
                {
                    MessageBox.Show(
                        "Please set your username and Password in the settings tab.",
                        "Xtra Desktop",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk
                    );

                    return false;
                }
                if (Empfaenger.Text.Length <= 0)
                {
                    MessageBox.Show(
                       "No reciever entered.",
                       "Xtra Desktop",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk
                   );
                    return false;
                }
                if (Nachricht.Text.Length <= 0)
                {
                    MessageBox.Show(
                       "No message entered",
                       "Xtra Desktop",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk
                   );
                    return false;
                }
                if (Nachricht.Text.Length > 440)
                {
                    MessageBox.Show(
                       "Message is too long.",
                       "Xtra Desktop",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk
                   );
                    return false;
                }
                return true;
            }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Username = Username.Text;
            Properties.Settings.Default.Passwort = Password.Text;
            Properties.Settings.Default.Save();
        }
    }
}
