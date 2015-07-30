using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Api
{
    public interface IApiKeyBuilder
    {
        IApiKeyBuilder SetId(string id);

        IApiKeyBuilder SetSecret(string secret);

        // begin TODO
        // setProperties
        // setReader
        // setInputString
        // setFileLocaiton
        // setIdPropertyName
        // setSecretPropertyName
        // end TODO

        IApiKey Build();
    }
}
