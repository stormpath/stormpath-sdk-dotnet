// <copyright file="UsernamePasswordRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.Auth;

namespace Stormpath.SDK.Auth
{
    public sealed class UsernamePasswordRequestBuilder
    {
        private string usernameOrEmail;
        private string password;
        private IAccountStore accountStore;
        private string organizationNameKey;

        public UsernamePasswordRequestBuilder SetUsernameOrEmail(string usernameOrEmail)
        {
            if (string.IsNullOrEmpty(usernameOrEmail))
                throw new ArgumentNullException(nameof(usernameOrEmail));

            this.usernameOrEmail = usernameOrEmail;
            return this;
        }

        public UsernamePasswordRequestBuilder SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(password);

            this.password = password;
            return this;
        }

        public UsernamePasswordRequestBuilder SetAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
                throw new ArgumentNullException(nameof(accountStore));

            if (!string.IsNullOrEmpty(this.organizationNameKey))
                throw new ArgumentException("Cannot set both AccountStore and hrefOrNameKey properties.");

            this.accountStore = accountStore;
            return this;
        }

        public UsernamePasswordRequestBuilder SetAccountStore(string hrefOrNameKey)
        {
            if (string.IsNullOrEmpty(hrefOrNameKey))
                throw new ArgumentNullException(hrefOrNameKey);

            if (this.accountStore != null)
                throw new ArgumentException("Cannot set both AccountStore and hrefOrNameKey properties.");

            this.organizationNameKey = hrefOrNameKey;
            return this;
        }

        public IAuthenticationRequest Build()
        {
            return new UsernamePasswordRequest(this.usernameOrEmail, this.password, this.accountStore, this.organizationNameKey);
        }
    }
}
