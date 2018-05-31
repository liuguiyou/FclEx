namespace FclEx.Http.Proxy
{
    public interface IProxyManager
    {
        WebProxy Pop(bool putBack = true, int timeout = -1);
    }
}
