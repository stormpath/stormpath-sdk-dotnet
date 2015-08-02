using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public sealed class ClientApiKeys
    {
        public static IClientApiKeyBuilder Builder()
        {
            return new Impl.Api.DefaultClientApiKeyBuilder();
        }
    }
}
