using System;
using System.IO;
using System.Net;
using System.Diagnostics;
namespace XtraClient
{
    public class httpModule
    {
        public CookieContainer cookieContainer = new CookieContainer();

        public string doHTTP(string url, string postData, bool createCookie)
        {
            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = postData.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.CookieContainer = this.cookieContainer;

            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(postData);
            }
            catch (Exception e)
            {
                Debug.Write("Could not read response");
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            if (createCookie)
            {
                foreach (Cookie cook in objResponse.Cookies)
                {
                    this.cookieContainer.Add(cook);
                }
                Debug.Write("\n Cookie written");
            }

            StreamReader reader = new StreamReader(objResponse.GetResponseStream());
            return reader.ReadToEnd().Trim();
        }

    }
}