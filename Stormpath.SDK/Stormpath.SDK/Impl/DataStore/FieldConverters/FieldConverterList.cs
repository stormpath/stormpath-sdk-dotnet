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
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Stormpath.SDK.Impl.DataStore.FieldConverters
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

        public FieldConverterList(params FieldConverter[] converters)
            : this(converters.AsEnumerable())
        {
        }

        public int Count => this.converters.Count;

        public void Add(FieldConverter converter)
        {
            this.converters.Add(converter);
        }

        public ConverterResult TryConvertField(JProperty token, Type targetType)
        {
            var result = ConverterResult.Failed; // presumed failed until proven successful

            foreach (var converter in this.converters)
            {
                result = converter.TryConvertField(token, targetType);
                if (result.Success)
                    break;
            }

            return result;
        }
    }
}
