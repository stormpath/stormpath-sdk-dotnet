// <copyright file="ClientApiKey.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Api
{
    /// <summary>
    /// Main body of ClientApiKey class. (documentation todo)
    /// </summary>
    internal sealed class ClientApiKey : ImmutableValueObject<ClientApiKey>, IClientApiKey
    {
        private readonly string id;
        private readonly string secret;

        internal ClientApiKey(string id, string secret)
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

        string IClientApiKey.GetId()
        {
            return this.id;
        }

        string IClientApiKey.GetSecret()
        {
            return this.secret;
        }

        bool IClientApiKey.IsValid()
        {
            return !string.IsNullOrEmpty(this.id)
                && !string.IsNullOrEmpty(this.secret);
        }
    }
}
