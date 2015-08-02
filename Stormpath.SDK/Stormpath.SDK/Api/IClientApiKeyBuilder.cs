using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public interface IClientApiKeyBuilder
    {
        IClientApiKeyBuilder SetId(string id);

        IClientApiKeyBuilder SetSecret(string secret);

        IClientApiKeyBuilder SetInputStream(System.IO.Stream stream);

        IClientApiKeyBuilder SetFileLocation(string path);

        IClientApiKeyBuilder SetIdPropertyName(string idPropertyName);

        IClientApiKeyBuilder SetSecretPropertyName(string secretPropertyName);

        IClientApiKey Build();
    }
}
