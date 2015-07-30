using Stormpath.SDK.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Api
{
    internal sealed class ClientApiKeyBuilder : IApiKeyBuilder
    {
        private string apiKeyId;
        private string apiKeySecret;


        IApiKey IApiKeyBuilder.Build()
        {
            throw new NotImplementedException();
        }

        IApiKeyBuilder IApiKeyBuilder.SetId(string id)
        {
            apiKeyId = id;
            return this;
        }

        IApiKeyBuilder IApiKeyBuilder.SetSecret(string secret)
        {
            apiKeySecret = secret;
            return this;
        }
    }
}
