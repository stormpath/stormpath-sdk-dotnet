// <copyright file="DefaultClientApiKey.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.Api
{
    [Obsolete("Remove after 1.0. Replace with ClientApiKeyConfiguration")]
    internal sealed class DefaultClientApiKey : ImmutableValueObject<DefaultClientApiKey>, IClientApiKey
    {
        private readonly string id;
        private readonly string secret;

        internal DefaultClientApiKey(string id, string secret)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("API key ID cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("API key secret cannot be null or empty.");
            }

            this.id = id;
            this.secret = secret;
        }

        string IClientApiKey.GetId() => this.id;

        string IClientApiKey.GetSecret() => this.secret;

        bool IClientApiKey.IsValid() =>
            !string.IsNullOrEmpty(this.id) &&
            !string.IsNullOrEmpty(this.secret);
    }
}
