// <copyright file="FieldConverterList.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json.Linq;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class FieldConverterList
    {
        private readonly List<FieldConverter> converters;

        public FieldConverterList()
        {
            this.converters = new List<FieldConverter>();
        }

        public FieldConverterList(FieldConverter initialConverter)
            : this()
        {
            this.converters.Add(initialConverter);
        }

        public FieldConverterList(IEnumerable<FieldConverter> initialConverters)
            : this()
        {
            this.converters.AddRange(initialConverters);
        }

        public int Count => this.converters.Count;

        public void Add(FieldConverter converter)
        {
            this.converters.Add(converter);
        }

        public bool TryConvertField(JToken token, Type targetType, out object converted)
        {
            bool didSucceed = false;
            converted = null;

            foreach (var converter in this.converters)
            {
                didSucceed = converter.TryConvertField(token, targetType, out converted);
                if (didSucceed)
                    break;
            }

            return didSucceed;
        }
    }
}
