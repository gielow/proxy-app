
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
}
