using System.Diagnostics;
using Xunit;

namespace Stormpath.SDK.Tests.Common
{
    public class DebugOnlyTheoryAttribute : TheoryAttribute
    {
        public DebugOnlyTheoryAttribute()
        {
            if (!Debugger.IsAttached)
            {
                this.Skip = "Only run in Debug mode.";
            }
        }
    }
}
