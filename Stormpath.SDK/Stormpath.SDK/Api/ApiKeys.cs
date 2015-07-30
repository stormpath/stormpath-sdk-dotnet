using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public sealed class ApiKeys
    {
        public static IApiKeyBuilder Builder()
        {
            return new Impl.Api.ClientApiKeyBuilder();
        }
    }
}
