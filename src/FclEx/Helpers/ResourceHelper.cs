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
        public static Stream LoadEmbededResource(string name)
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(p => p.EndsWith(name));
            var stream = resourceName == null ? null : assembly.GetManifestResourceStream(resourceName);
            return stream;
        }

        public static Stream LoadFileResource(string name)
        {
            const string dir = "Resources";
            var path = Path.Combine(dir, name);
            var fs = File.Exists(path) ? File.Open(path, FileMode.Open) : null;
            return fs;
        }

        public static T LoadLocalResource<T>(string name, Func<Stream, T> func)
        {
            var resource = LoadFileResource(name) ?? LoadEmbededResource(name);
            using (resource)
            {
                return func(resource);
            }
        }

        public static string LoadStringFromLocalResource(string name)
        {
            return LoadLocalResource(name, s => s.ToBytes().GetString());
        }

        public static string[] LoadLinesFromLocalResource(string name)
        {
            return LoadStringFromLocalResource(name)
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
