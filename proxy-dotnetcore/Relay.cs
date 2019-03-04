using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
            relayRequest.Accept = string.Join(',', Request.AcceptTypes);
            relayRequest.ContentType = Request.ContentType;
            relayRequest.Method = Request.HttpMethod;
            relayRequest.UserAgent = Request.UserAgent;

            relayRequest.Headers["Accept-Encoding"] = Request.Headers["Accept-Encoding"];

            CopyHeaders(relayRequest.Headers, Request.Headers);

            return relayRequest.GetResponse();
        }

        private void CopyHeaders(WebHeaderCollection destinationHeader, System.Collections.Specialized.NameValueCollection originHeader)
        {

        }

        private string ExtractProxyUrl()
        {
            return Request.RawUrl.Replace("/proxy/", string.Empty);
        }
    }
}
