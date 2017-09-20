using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FclEx.Extensions;

namespace FclEx.Helpers
{
    public static class ResourceHelper
    {
        public static Stream LoadEmbededResource(Assembly assembly, string name)
        {
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(p => p.EndsWith(name));
            var stream = resourceName == null ? null : assembly.GetManifestResourceStream(resourceName);
            return stream;
        }

        public static T LoadEmbededResource<T>(Assembly assembly, string name, Func<Stream, T> func)
        {
            using (var resource = LoadEmbededResource(assembly, name))
            {
                return func(resource);
            }
        }

        public static string LoadStringFromEmbededResource(Assembly assembly, string name) => LoadEmbededResource(assembly, name, s => s.ToBytes().GetString());

        public static string[] LoadLinesFromEmbededResource(Assembly assembly, string name)
        {
            return LoadStringFromEmbededResource(assembly, name)
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
