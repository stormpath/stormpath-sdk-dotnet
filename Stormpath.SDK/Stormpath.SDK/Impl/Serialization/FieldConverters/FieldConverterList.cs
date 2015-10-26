// <copyright file="FieldConverterList.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stormpath.SDK.Impl.Serialization.FieldConverters
{
    internal sealed class FieldConverterList
    {
        private readonly List<AbstractFieldConverter> converters;

        public FieldConverterList()
        {
            this.converters = new List<AbstractFieldConverter>();
        }

        public FieldConverterList(AbstractFieldConverter initialConverter)
            : this()
        {
            this.converters.Add(initialConverter);
        }

        public FieldConverterList(IEnumerable<AbstractFieldConverter> initialConverters)
            : this()
        {
            this.converters.AddRange(initialConverters);
        }

        public FieldConverterList(params AbstractFieldConverter[] converters)
            : this(converters.AsEnumerable())
        {
        }

        public int Count => this.converters.Count;

        public void Add(AbstractFieldConverter converter)
        {
            this.converters.Add(converter);
        }

        public FieldConverterResult TryConvertField(KeyValuePair<string, object> token, Type targetType, Func<IDictionary<string, object>, Type, IDictionary<string, object>> recursiveConverter)
        {
            var result = FieldConverterResult.Failed; // presumed failed until proven successful

            foreach (var converter in this.converters)
            {
                result = converter.TryConvertField(token, targetType, recursiveConverter);
                if (result.Success)
                    break;
            }

            return result;
        }
    }
}
