using System;
using System.Collections.Generic;
using System.Reflection;
using FclEx.Utils;
using JetBrains.Annotations;

namespace FclEx.Fm.Modules
{
    /// <summary>
    /// Used to store all needed information for a module.
    /// </summary>
    public class FmModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public FmModule Instance { get; }

        /// <summary>
        /// Is this module loaded as a plugin.
        /// </summary>
        public bool IsLoadedAsPlugIn { get; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<FmModuleInfo> Dependencies { get; }

        /// <summary>
        /// Creates a new FmModuleInfo object.
        /// </summary>
        public FmModuleInfo([NotNull] Type type, [NotNull] FmModule instance, bool isLoadedAsPlugIn)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            Type = type;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;
            Assembly = Type.Assembly;

            Dependencies = new List<FmModuleInfo>();
        }

        public override string ToString()
        {
            return Type.AssemblyQualifiedName ??
                   Type.FullName;
        }
    }
}