using System.Diagnostics;
using Xunit;

namespace Stormpath.SDK.Tests.Common
{
    public class DebugOnlyFactAttribute : FactAttribute
    {
        public DebugOnlyFactAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only run in Debug mode.";
            }
        }
    }
}
