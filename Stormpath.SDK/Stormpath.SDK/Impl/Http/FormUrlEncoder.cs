// <copyright file="FormUrlEncoder.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Utility;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Http
{
    internal sealed class FormUrlEncoder
    {
        private readonly Lazy<string> generated;

        public FormUrlEncoder(Map properties)
        {
            this.generated = new Lazy<string>(() => Build(properties));
        }

        public override string ToString()
            => this.generated.Value;

        private static string Build(Map properties)
        {
            var sortedProperties = new SortedDictionary<string, object>(properties);

            var urlEncoded = sortedProperties
                .Select(entry => $"{RequestHelper.UrlEncode(entry.Key)}={RequestHelper.UrlEncode(entry.Value.ToString())}")
                .Join("&");
            return urlEncoded;
        }
    }
}
