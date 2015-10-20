using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Linq.Parsing.Translators
{
    internal abstract class AbstractNameTranslator
    {
        private readonly IDictionary<string, string> map;

        public AbstractNameTranslator(IDictionary<string, string> keyValuePairs)
        {
            this.map = keyValuePairs;
        }

        public bool TryGetValue(string key, out string value)
            => this.map.TryGetValue(key, out value);
    }
}
