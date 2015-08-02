using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Utility
{
    internal class Properties
    {
        private static readonly char[] IgnoreLinesStartingWith = { '#', '!' };
        private readonly IDictionary<string, string> _props;

        public Properties(string input)
        {
            _props = Parse(input);
        }

        public string GetProperty(string key)
        {
            string value;
            if (_props.TryGetValue(key, out value)) return value;
            return null;
        }

        public string GetProperty(string key, string defaultValue)
        {
            var value = GetProperty(key);
            if (value != null) return value;
            return defaultValue;
        }

        public int Count()
        {
            return _props.Count;
        }

        private static IDictionary<string, string> Parse(string input)
        {
            // TODO Future: support the Java .properties spec better (this will work for now)

            input = input?.Trim();
            if (string.IsNullOrEmpty(input)) return new Dictionary<string, string>();

            var goodLines = input
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !(IgnoreLinesStartingWith.Contains(x.First())));

            var pairs = goodLines
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2);

            return pairs.ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
        }
    }
}
