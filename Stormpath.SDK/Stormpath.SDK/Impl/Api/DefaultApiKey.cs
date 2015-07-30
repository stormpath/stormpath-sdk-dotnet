using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Api;

namespace Stormpath.SDK.Impl.Api
{
    internal class DefaultApiKey : IApiKey
    {
        Task<string> IApiKey.GetIdAsync()
        {
            return Task.FromResult("foo");
        }

        Task<string> IApiKey.GetSecretAsync()
        {
            return Task.FromResult("bar");
        }
    }
}
