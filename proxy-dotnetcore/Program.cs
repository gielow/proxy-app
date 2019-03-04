using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace proxy_dotnetcore
{
    
    public class Program
    {
        private const string _proxyEndpoint = "http://localhost:8000/proxy/";
        private static void Main(string[] args)
        {
            var proxy = new ProxyListener(_proxyEndpoint);
            proxy.Start();
        }
    }

    public class Relay2
    {
        private readonly HttpListenerContext originalContext;

        public Relay2(HttpListenerContext originalContext)
        {
            this.originalContext = originalContext;
        }

        public void ProcessRequest()
        {
            string rawUrl = originalContext.Request.RawUrl;
            var requestUrl = originalContext.Request.RawUrl.Replace("/proxy/", string.Empty);
            Console.WriteLine("Proxy receive a request for: " + requestUrl);

            var relayRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            relayRequest.KeepAlive = false;
            relayRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            relayRequest.UserAgent = this.originalContext.Request.UserAgent;

            var requestData = new RequestState(relayRequest, originalContext);
            relayRequest.BeginGetResponse(ResponseCallBack, requestData);
        }

        private static void ResponseCallBack(IAsyncResult asynchronousResult)
        {
            var requestData = (RequestState)asynchronousResult.AsyncState;
            Console.WriteLine("Proxy receive a response from " + requestData.context.Request.RawUrl);

            using (var responseFromWebSiteBeingRelayed = (HttpWebResponse)requestData.webRequest.EndGetResponse(asynchronousResult))
            {
                using (var responseStreamFromWebSiteBeingRelayed = responseFromWebSiteBeingRelayed.GetResponseStream())
                {
                    var originalResponse = requestData.context.Response;

                    if (responseFromWebSiteBeingRelayed.ContentType.Contains("text/html"))
                    {
                        var reader = new StreamReader(responseStreamFromWebSiteBeingRelayed);
                        string html = reader.ReadToEnd();
                        //Here can modify html
                        byte[] byteArray = System.Text.Encoding.Default.GetBytes(html);
                        var stream = new MemoryStream(byteArray);
                        stream.CopyTo(originalResponse.OutputStream);
                    }
                    else
                    {
                        responseStreamFromWebSiteBeingRelayed.CopyTo(originalResponse.OutputStream);
                    }
                    originalResponse.OutputStream.Close();
                }
            }
        }
    }

    public class RequestState
    {
        public readonly HttpWebRequest webRequest;
        public readonly HttpListenerContext context;

        public RequestState(HttpWebRequest request, HttpListenerContext context)
        {
            webRequest = request;
            this.context = context;
        }
    }
}
