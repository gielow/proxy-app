using Microsoft.VisualStudio.TestTools.UnitTesting;
using proxy_dotnetcore;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace proxy_dotnetcore_tests
{
    [TestClass]
    public class ProxyListenerCtorTests
    {
        [TestMethod]
        public void ProxyListenerValidCreation()
        {
            Assert.IsNotNull(new ProxyListener("http://localhost:8000/proxy/"));
        }

        [TestMethod]
        public void ProxyListenerEmptyUrlCreation()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyListener(string.Empty));
            Assert.ThrowsException<ArgumentNullException>(() => new ProxyListener(null));
        }

        [TestMethod]
        public void ProxyListenerInvalidUrlCreation()
        {
            Assert.ThrowsException<ArgumentException>(() => new ProxyListener("blabla.bla"));
        }
    }
}
