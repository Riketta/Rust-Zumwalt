
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Fougerite
{
    using System.Net;
    using System.Text;

    public class Web
    {
        /// <summary>
        /// SSL Protocols.
        /// </summary>
        [Flags]
        public enum MySecurityProtocolType
        {
            //
            // Summary:
            //     Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
            Ssl3 = 48,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.0 security protocol.
            Tls = 192,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.1 security protocol.
            Tls11 = 768,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.2 security protocol.
            Tls12 = 3072
        }

        
        /// <summary>
        /// Does a GET request to the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GET(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }

        /// <summary>
        /// Does a post request to the specified URL with the data.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string POST(string url, string data)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] bytes = client.UploadData(url, "POST", Encoding.ASCII.GetBytes(data));
                return Encoding.ASCII.GetString(bytes);
            }
        }

        /// <summary>
        /// Does a GET request to the specified URL, and accepts all SSL certificates.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GETWithSSL(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
        
        /// <summary>
        /// Does a post request to the specified URL with the data, and accepts all SSL certificates.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string POSTWithSSL(string url, string data)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] bytes = client.UploadData(url, "POST", Encoding.ASCII.GetBytes(data));
                return Encoding.ASCII.GetString(bytes);
            }
        }
        
        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
    }
}