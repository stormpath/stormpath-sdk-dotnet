using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal sealed class DefaultCreationOptions : ICreationOptions
    {
        private readonly KeyValuePair<string, string>[] _parameters;

        public DefaultCreationOptions(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _parameters = parameters?.ToArray() ?? new KeyValuePair<string, string>[0];
        }

        public string GetQueryString()
        {
            if (!_parameters.Any())
            {
                return null;
            }

            return string.Join("&", _parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }
    }
}
