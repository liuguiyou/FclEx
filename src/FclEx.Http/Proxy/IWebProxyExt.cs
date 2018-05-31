using System.Net;

namespace FclEx.Http.Proxy
{
    public interface IWebProxyExt : IWebProxy
    {
        EnumProxyType ProxyType { get; }
        string Host { get; }
        int Port { get; }
    }
}
