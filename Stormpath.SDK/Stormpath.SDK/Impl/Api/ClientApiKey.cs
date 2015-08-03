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

namespace Stormpath.SDK.Impl.Api
{
    using System;
    using SDK.Api;
    using Utility;

    /// <summary>
    /// Main body of ClientApiKey class. (documentation todo)
    /// </summary>
    internal partial class ClientApiKey : IClientApiKey
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
    }

    /// <summary>
    /// Equality-related code for ClientApiKey
    /// </summary>
    internal partial class ClientApiKey : IEquatable<ClientApiKey>
    {
        private static readonly GenericComparer<ClientApiKey> Comparer = new GenericComparer<ClientApiKey>(
            (ClientApiKey x, ClientApiKey y) =>
            {
                return x.id == y.id && x.secret == y.secret;
            },
            (ClientApiKey x) =>
            {
                return HashCode.Start
                    .Hash(x.id);
            });

        public static bool operator ==(ClientApiKey x, ClientApiKey y)
        {
            return Comparer.Equals(x, y);
        }

        public static bool operator !=(ClientApiKey x, ClientApiKey y)
        {
            return !(x == y);
        }

        public bool Equals(ClientApiKey other)
        {
            return Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClientApiKey;
            if (obj == null)
            {
                return false;
            }

            return Comparer.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Comparer.GetHashCode(this);
        }
    }
}
