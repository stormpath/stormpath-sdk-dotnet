using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    internal class FileWrapper : IFile
    {
        public string ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    }
}
