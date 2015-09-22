// <copyright file="Properties.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed class Properties
    {
        private static readonly char[] IgnoreLinesStartingWith = { '#', '!' };
        private readonly IDictionary<string, string> props;

        public Properties(string input)
        {
            this.props = Parse(input);
        }

        public string GetProperty(string key)
        {
            string value;
            if (this.props.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        public string GetProperty(string key, string defaultValue)
        {
            var value = this.GetProperty(key);
            if (value != null)
            {
                return value;
            }

            return defaultValue;
        }

        public int Count() => this.props.Count;

        private static IDictionary<string, string> Parse(string input)
        {
            input = input?.Trim();
            if (string.IsNullOrEmpty(input))
                return new Dictionary<string, string>();

            var goodLines = input
                .Split(PossibleNewlines(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !IgnoreLinesStartingWith.Contains(x.First()));

            var pairs = goodLines
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2);

            return pairs.ToDictionary(
                pair => pair[0].Trim(),
                pair => pair[1].Trim());
        }

        private static string[] PossibleNewlines()
        {
            // Windows thinks newlines are \r\n, but Unix swears it's just \n
            return new string[] { "\r\n", "\n" };
        }
    }
}
