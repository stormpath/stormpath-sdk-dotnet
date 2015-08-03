using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    // A subset of methods on System.IO.File, for easy unit testing
    internal interface IFile
    {
        string ReadAllText(string path);
    }
}
