// <copyright file="UsernamePasswordRequest.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Auth;

namespace Stormpath.SDK.Impl.Auth
{
    /// <summary>
    /// Represents a username (or email) and password pair.
    /// </summary>
    /// <seealso cref="UsernamePasswordRequestBuilder"/>
    internal sealed class UsernamePasswordRequest : IAuthenticationRequest, IHasOrganizationNameKey
    {
        private readonly string username;
        private readonly string password;
        private readonly IAccountStore accountStore;
        private readonly string organizationNameKey;

        public UsernamePasswordRequest(string username, string password, IAccountStore accountStore, string organizationNameKey)
        {
            this.username = username;
            this.password = password;
            this.accountStore = accountStore;
            this.organizationNameKey = organizationNameKey;
        }

        string IAuthenticationRequest<string, string>.Principals => this.username;

        string IAuthenticationRequest<string, string>.Credentials => this.password;

        IAccountStore IAuthenticationRequest<string, string>.AccountStore => this.accountStore;

        string IHasOrganizationNameKey.OrganizationNameKey => this.organizationNameKey;
    }
}
