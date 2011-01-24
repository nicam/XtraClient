using System;
using System.Windows.Forms;
using System.Collections;


namespace WindowsFormsApplication1
{
    public partial class LoginForm : Form
    {
        public AddItemDelegate AddItemCallback;

        public LoginForm(string errorMsg)
        {
            InitializeComponent();
            Cursor.Current = Cursors.WaitCursor;
            errorBox.Text = errorMsg;
            string Username = Properties.Settings.Default.Username;
            string Password = Properties.Settings.Default.Passwort;
            this.httpObj = new XtraClient.httpModule();
            string response;
            string parameters = "action=getCaptcha&do_sso_login=0&passphrase=&sso_password=" + Username + "&sso_user=" + Password + "&token";
            response = this.httpObj.doHTTP("https://xtrazone.sso.bluewin.ch/index.php/22,39,ajax_json,,,157/", parameters, false);

            Hashtable jsonHash = (Hashtable)Procurios.Public.JSON.JsonDecode(response);

            string imageURL = Convert.ToString(jsonHash["img"]);
            this.token = imageURL.Replace("//www.scsstatic.ch/captcha/", "");
            pictureBox1.LoadAsync("http://" + imageURL.Substring(2));
        }

        public XtraClient.httpModule httpObj;

        public string token;

        /*string HttpPost(string uri, string parameters)
        {	
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "HttpPost: Request error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            try
            { // get the response
                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                { return null; }
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "HttpPost: Response error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        } // end HttpPost 
        */
        private void sendBtn_Click(object sender, EventArgs e)
        {
            AddItemCallback(captcha.Text, this.token);
            this.Close();
        }
    }
}
