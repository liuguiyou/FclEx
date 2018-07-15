using System.Net;

namespace FclEx.Http.Proxy
{
    public interface IWebProxyExt : IWebProxy
    {
        ProxyType ProxyType { get; }
        string Host { get; }
        int Port { get; }
    }
}
