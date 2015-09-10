// <copyright file="LinkPropertyConverter.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore.FieldConverters
{
    internal sealed class LinkPropertyConverter : AbstractFieldConverter
    {
        public LinkPropertyConverter()
            : base(nameof(LinkPropertyConverter), appliesToTargetType: AnyType)
        {
        }

        protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token)
        {
            var asEmbeddedObject = token.Value as IDictionary<string, object>;
            if (asEmbeddedObject == null)
                return FieldConverterResult.Failed;

            var firstItem = asEmbeddedObject.FirstOrDefault();

            var isLinkProperty = asEmbeddedObject.Count == 1
                && !string.IsNullOrEmpty(firstItem.ToString())
                && string.Equals(firstItem.Key, "href", StringComparison.InvariantCultureIgnoreCase);

            if (!isLinkProperty)
                return null;

            return new FieldConverterResult(true, new LinkProperty(firstItem.Value.ToString()));
        }
    }
}
