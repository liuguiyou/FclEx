using System;
using System.Collections.Generic;

namespace FclEx.Fw.Modules
{
    public interface IFwModuleManager
    {
        FwModuleInfo StartupModule { get; }

        IReadOnlyList<FwModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}