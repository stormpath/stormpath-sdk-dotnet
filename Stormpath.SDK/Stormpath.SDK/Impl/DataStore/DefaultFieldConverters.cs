// <copyright file="DefaultFieldConverters.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields must be private", Justification = "Reviewed.")]
    internal static class DefaultFieldConverters
    {
        public static readonly FieldConverter LinkPropertyConverter =
            new FieldConverter(
                appliesToTokenType: JTokenType.Object,
                convertAction: token =>
            {
                var firstChild = token.First as JProperty;

                bool isLinkProperty = token.Children().Count() == 1
                    && !string.IsNullOrEmpty(firstChild?.Value.ToString())
                    && string.Equals(firstChild?.Name, "href", StringComparison.OrdinalIgnoreCase);

                if (!isLinkProperty)
                    return null;

                return new LinkProperty(firstChild.Value.ToString());
            });

        public static readonly FieldConverter DateTimeOffsetConverter =
            new FieldConverter(
                appliesToTokenType: JTokenType.Date,
                convertAction: token =>
            {
                return token.Value<DateTimeOffset>();
            });

        public static readonly FieldConverter StringConverter =
            new FieldConverter(
                appliesToTokenType: JTokenType.String,
                convertAction: token =>
            {
                return token.ToString();
            });

        public static readonly FieldConverter DefaultConverter =
            new FieldConverter(token =>
            {
                return token;
            });

        public static readonly FieldConverterList All = new FieldConverterList(
            new List<FieldConverter>()
            {
                LinkPropertyConverter,
                DateTimeOffsetConverter,
                StringConverter,
                DefaultConverter,
            });
    }
}
