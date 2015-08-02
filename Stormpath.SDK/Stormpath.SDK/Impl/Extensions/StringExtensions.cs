using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Extensions
{
    internal static class StringExtensions
    {
        public static string OrIfEmptyUse(this string source, string defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            return source;
        }
    }
}
