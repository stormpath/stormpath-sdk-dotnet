using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Tests.Common
{
    public static class Assertly
    {
        public static void Fail(string message)
        {
            Xunit.Assert.True(false, message);
        }

        public static void Todo()
        {
            Xunit.Assert.True(false, "TODO");
        }
    }
}
