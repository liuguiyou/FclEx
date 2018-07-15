namespace FclEx.Http.Proxy
{
    public interface IProxyManager
    {
        IWebProxyExt Pop(bool putBack = true, int timeout = -1);
    }
}
