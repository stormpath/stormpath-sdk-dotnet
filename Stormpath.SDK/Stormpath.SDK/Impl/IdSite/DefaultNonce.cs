// <copyright file="DefaultNonce.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultNonce : INonce
    {
        public static readonly string ValuePropertyName = "value";

        private readonly IDictionary<string, object> properties;

        public DefaultNonce(string nonce)
        {
            if (string.IsNullOrEmpty(nonce))
                throw new ArgumentNullException(nameof(nonce));

            this.properties = new Dictionary<string, object>();
            this.properties.Add(ValuePropertyName, nonce);
        }

        public DefaultNonce(IDictionary<string, object> properties)
        {
            if (properties == null || !properties.Any())
                throw new ArgumentNullException(nameof(properties));

            object value = null;
            if (!properties.TryGetValue(ValuePropertyName, out value))
                throw new ArgumentException(nameof(properties), $"The dictionary must contain the key '{ValuePropertyName}'.");

            if (!(value is string))
                throw new ArgumentException("Cannot parse nonce value.");

            this.properties = properties;
        }

        public IDictionary<string, object> GetProperties()
            => new Dictionary<string, object>(this.properties);

        string IResource.Href => this.GetValue();

        string INonce.Value => this.GetValue();

        private string GetValue()
        {
            return this.properties[ValuePropertyName].ToString();
        }
    }
}
