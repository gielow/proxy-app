using Microsoft.VisualStudio.TestTools.UnitTesting;
using proxy_dotnetcore;
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

        

        [TestMethod]
        public async Task ProxyListenetGetTest()
        {
            var requestUrl = $"{proxyUrl}http://httpbin.org/get";

            var relayRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            relayRequest.UserAgent = "IntegrationTestUserAgent";

            var response = await relayRequest.GetResponseAsync();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            Assert.IsTrue(content.Contains("\"User-Agent\": \"IntegrationTestUserAgent\""));
        }

        [TestMethod]
        public async Task ProxyListenerPostTest()
        {
            var requestUrl = $"{proxyUrl}http://httpbin.org/post";
            
            var values = new Dictionary<string, string>
            {
               { "asdf", "blah" },
            };

            var content = new FormUrlEncodedContent(values);
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(requestUrl, content);
            
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.IsNotNull(responseString);
            Assert.IsTrue(responseString.Contains("\"asdf\": \"blah\""));
        }
    }
}
