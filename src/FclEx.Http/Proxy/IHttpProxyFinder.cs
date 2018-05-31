namespace FclEx.Http.Proxy
{
    public interface IHttpProxyFinder
    {
        void Init(IProxyManager proxyManager);
        string GetHttpProxy();
    }
}
