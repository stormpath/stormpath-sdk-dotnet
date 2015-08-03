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
        private readonly string _id;
        private readonly string _secret;

        private static readonly GenericComparer<ClientApiKey> _comparer = new GenericComparer<ClientApiKey>(
            (ClientApiKey x, ClientApiKey y) =>
            {
                return x._id == y._id && x._secret == y._secret;
            },
            (ClientApiKey x) =>
            {
                return HashCode.Start
                    .Hash(x._id);
            });

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

            _id = id;
            _secret = secret;
        }

        string IClientApiKey.GetId()
        {
            return _id;
        }

        string IClientApiKey.GetSecret()
        {
            return _secret;
        }
    }

    /// <summary>
    /// Equality-related code for ClientApiKey
    /// </summary>
    internal partial class ClientApiKey : IEquatable<ClientApiKey>
    {
        public static bool operator ==(ClientApiKey x, ClientApiKey y)
        {
            return _comparer.Equals(x, y);
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

            return _comparer.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }
    }
}
