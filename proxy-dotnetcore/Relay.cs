using System;
using System.Linq;
using System.Net;

namespace proxy_dotnetcore
{
    public class Relay
    {
        private HttpListenerRequest Request { get; set; }
        public Relay(HttpListenerRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Request = request;
        }

        public WebResponse ProcessRequest()
        {
            var requestUrl = ExtractProxyUrl();

            var relayRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            relayRequest.KeepAlive = false;

            if (Request.AcceptTypes?.FirstOrDefault() != null)
            {
                relayRequest.Accept = string.Join(",", Request.AcceptTypes);
            }

            relayRequest.ContentType = Request.ContentType;
            relayRequest.Method = Request.HttpMethod;
            relayRequest.UserAgent = Request.UserAgent;

            // POST form data
            if (Request.HasEntityBody)
            {
                var relayStream = relayRequest.GetRequestStream();
                Request.InputStream.CopyTo(relayStream);
                relayStream.Close();
            }

            return relayRequest.GetResponse();
        }

        private string ExtractProxyUrl()
        {
            return Request.RawUrl.Replace("/proxy/", string.Empty);
        }
    }
}
