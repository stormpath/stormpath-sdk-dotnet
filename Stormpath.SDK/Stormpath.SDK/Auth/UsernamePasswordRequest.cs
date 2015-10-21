// <copyright file="UsernamePasswordRequest.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;

namespace Stormpath.SDK.Auth
{
    /// <summary>
    /// Represents a username (or email) and password pair.
    /// </summary>
    public sealed class UsernamePasswordRequest : IAuthenticationRequest
    {
        private readonly string username;
        private readonly string password;
        private readonly IAccountStore accountStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordRequest"/> class with the specified username (or email) and password.
        /// </summary>
        /// <param name="usernameOrEmail">The account's username or email address</param>
        /// <param name="password">The account's raw (plaintext) password</param>
        public UsernamePasswordRequest(string usernameOrEmail, string password)
            : this(usernameOrEmail, password, null)
        {
        }

        private UsernamePasswordRequest(string username, string password, IAccountStore accountStore)
        {
            this.username = username;
            this.password = password;
            this.accountStore = accountStore;
        }

        string IAuthenticationRequest<string, string>.Principals => this.username;

        string IAuthenticationRequest<string, string>.Credentials => this.password;
    }
}
