using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    // A simple subset of ConfigurationManager for unit testing
    internal interface IConfigurationManager
    {
        NameValueCollection AppSettings { get; }
    }
}
