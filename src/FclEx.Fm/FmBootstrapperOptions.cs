using FclEx.Fm.Dependency;

namespace FclEx.Fm
{
    public class FmBootstrapperOptions
    {
        ///// <summary>
        ///// Used to disable all interceptors added by ABP.
        ///// </summary>
        //public bool DisableAllInterceptors { get; set; }

        /// <summary>
        /// IIocManager that is used to bootstrap the ABP system. If set to null, uses global <see cref="FclEx.Fm.Dependency.IocManager.Instance"/>
        /// </summary>
        public IIocManager IocManager { get; set; }

        ///// <summary>
        ///// List of plugin sources.
        ///// </summary>
        //public PlugInSourceList PlugInSources { get; }

        public FmBootstrapperOptions()
        {
            IocManager = FclEx.Fm.Dependency.IocManager.Instance;
        }
    }
}
