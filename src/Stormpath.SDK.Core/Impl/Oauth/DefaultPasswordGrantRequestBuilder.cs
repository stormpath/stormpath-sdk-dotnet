// <copyright file="DefaultPasswordGrantRequestBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultPasswordGrantRequestBuilder : IPasswordGrantRequestBuilder
    {
        private string login;
        private string password;
        private string accountStoreHref;
        private string organizationNameKey;

        IPasswordGrantRequestBuilder IPasswordGrantRequestBuilder.SetLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentNullException(nameof(login));
            }

            this.login = login;
            return this;
        }

        IPasswordGrantRequestBuilder IPasswordGrantRequestBuilder.SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            this.password = password;
            return this;
        }

        IPasswordGrantRequestBuilder IPasswordGrantRequestBuilder.SetAccountStore(IAccountStore accountStore)
        {
            if (accountStore == null)
            {
                throw new ArgumentNullException(nameof(accountStore));
            }

            this.accountStoreHref = accountStore.Href;
            return this;
        }

        IPasswordGrantRequestBuilder IPasswordGrantRequestBuilder.SetAccountStore(string accountStoreHref)
        {
            this.accountStoreHref = accountStoreHref;
            return this;
        }

        IPasswordGrantRequestBuilder IPasswordGrantRequestBuilder.SetOrganizationNameKey(string nameKey)
        {
            organizationNameKey = nameKey;
            return this;
        }

        IPasswordGrantRequest IOauthAuthenticationRequestBuilder<IPasswordGrantRequest>.Build()
        {
            if (string.IsNullOrEmpty(this.login))
            {
                throw new Exception("The username field has not been set. Use SetLogin() to set this field.");
            }

            if (string.IsNullOrEmpty(this.password))
            {
                throw new Exception("The password field has not been set. Use SetLogin() to set this field.");
            }

            var request = new DefaultPasswordGrantRequest(this.login, this.password);

            if (this.accountStoreHref != null)
            {
                request.SetAccountStore(this.accountStoreHref);
            }

            if (!string.IsNullOrEmpty(organizationNameKey))
            {
                request.SetOrganizationNameKey(organizationNameKey);
            }

            return request;
        }
    }
}
