using System;
using System.Collections.Generic;

namespace FclEx.Fm.Modules
{
    public interface IFmModuleManager
    {
        FmModuleInfo StartupModule { get; }

        IReadOnlyList<FmModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}