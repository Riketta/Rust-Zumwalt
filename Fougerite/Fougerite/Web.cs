
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Fougerite
{
    using System.Net;
    using System.Text;

    public class Web
    {
        public string GET(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }

        public string POST(string url, string data)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] bytes = client.UploadData(url, "POST", Encoding.ASCII.GetBytes(data));
                return Encoding.ASCII.GetString(bytes);
            }
        }

        public string GETWithSSL(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
        
        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
    }
}