using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace proxy_dotnetcore
{
    public class ProxyListener
    {
        private HttpListener listener { get; set; }
        public ProxyListener(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

            if (!Uri.IsWellFormedUriString(prefix, UriKind.Absolute))
                throw new ArgumentException($"{prefix} is not a valid URL.");

            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
        }
        public void Start()
        {
            if (listener == null)
            {
                throw new Exception("Listener not initialized");
            }

            listener.Start();

            while (true)
            {
                var ctx = listener.GetContext();
                //new Thread(new Relay(ctx).ProcessRequest).Start();
                var a = new Thread(() =>
                {
                    var relay = new Relay(ctx.Request);
                    var response = relay.ProcessRequest();
                    
                    var stream = response.GetResponseStream();
                    stream.CopyTo(ctx.Response.OutputStream);
                    ctx.Response.OutputStream.Close();
                });

                a.Start();
            }
        }
    }
}
