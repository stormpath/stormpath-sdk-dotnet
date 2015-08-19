using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Tests.Integration
{
    public static class NamespaceExtensions
    {
        public static IEnumerable<Type> GetAllTypesInNamespace(this Assembly assembly, string namespaceRoot, bool onlyPublic = true)
        {
            var types = assembly.GetTypes()
                .Where(x => x.Namespace.StartsWith(namespaceRoot, StringComparison.OrdinalIgnoreCase));
            if (onlyPublic)
                types = types.Where(x => x.IsPublic);

            foreach (var type in types)
                yield return type;
        }
    }
}
