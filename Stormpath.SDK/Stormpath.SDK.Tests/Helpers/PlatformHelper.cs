using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Tests.Helpers
{
    public static class PlatformHelper
    {
        private static readonly Lazy<bool> IsRunningOnMonoValue = new Lazy<bool>(() =>
        {
            return Type.GetType("Mono.Runtime") != null;
        });

        public static bool IsRunningOnMono()
        {
            return IsRunningOnMonoValue.Value;
        }
    }
}
