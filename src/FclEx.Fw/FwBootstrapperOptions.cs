using FclEx.Fw.Dependency;

namespace FclEx.Fw
{
    public class FwBootstrapperOptions
    {
        public IIocManager IocManager { get; set; } = Dependency.IocManager.Instance;
    }
}
