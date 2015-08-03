using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    // A subset of methods available on the static Environment object (used for testing)
    internal interface IEnvironment
    {
        string ExpandEnvironmentVariables(string name);
        string GetEnvironmentVariable(string variable);
    }
}
