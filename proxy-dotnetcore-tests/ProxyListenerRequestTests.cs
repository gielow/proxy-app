using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using proxy_dotnetcore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace proxy_dotnetcore_tests
{
    [TestClass]
    public class ProxyListenerRequestTests
    {
        private const string proxyUrl = "http://localhost:8000/proxy/";

        [ClassInitialize]
        public static void ProxyListenerValidCreation(TestContext context)
        {
            Task.Run(() =>
            {
                var _proxy = new ProxyListener(proxyUrl);
                _proxy.Start();
            });
        }

        private static readonly HttpClient client = new HttpClient();

        [TestMethod]
        public async Task ProxyListenetGetTest()
        {
            var requestUrl = $"{proxyUrl}http://httpbin.org/get";

            var relayRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            relayRequest.UserAgent = "IntegrationTestUserAgent";

            var response = await relayRequest.GetResponseAsync();
            var stream = response.GetResponseStream();

            var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            Assert.IsTrue(content.Contains("\"User-Agent\": \"IntegrationTestUserAgent\""));
        }

        public async void ProxyListenerPostTest()
        {
            var requestUrl = $"{proxyUrl}http://httpbin.org/post";

            var relayRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            relayRequest.UserAgent = "IntegrationTestUserAgent";
            relayRequest.Method = "POST";

            var response = await relayRequest.GetResponseAsync();
            var stream = response.GetResponseStream();

            var reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            Assert.IsTrue(content.Contains("\"User-Agent\": \"IntegrationTestUserAgent\""));

            var values = new Dictionary<string, string>
            {
               { "thing1", "hello" },
               { "thing2", "world" }
            };

            //var content = new FormUrlEncodedContent(values);
            //
            //var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);
            //
            //var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
