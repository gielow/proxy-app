using Microsoft.VisualStudio.TestTools.UnitTesting;
using proxy_dotnetcore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace proxy_dotnetcore_tests
{
    [TestClass]
    public class GitTests
    {
        private const string proxyUrl = "http://localhost:8000/proxy/";

        [ClassInitialize]
        public static void BasicHttpServer(TestContext testContext)
        {
            Task.Run(() =>
            {
                var proxy = new ProxyListener(proxyUrl);
                proxy.Start();
            });
        }
        
        [TestMethod]
        public void should_return_html()
        {
            var proxyRequestUrl = $"{proxyUrl}http://httpbin.org/get";

            //BasicHttpServer(proxyAddress);
            var relayRequest = (HttpWebRequest)WebRequest.Create(proxyRequestUrl);
            relayRequest.UserAgent = "IntegrationTestUserAgent";
            var response = relayRequest.GetResponse();
            var stream = response.GetResponseStream();
                
            var reader = new StreamReader(stream);
            string html = reader.ReadToEnd();

            Assert.IsTrue(html.Contains("IntegrationTestUserAgent"));
        }
    }
}
