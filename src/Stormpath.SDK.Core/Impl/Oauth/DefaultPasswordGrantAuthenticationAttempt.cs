// <copyright file="DefaultPasswordGrantAuthenticationAttempt.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultPasswordGrantAuthenticationAttempt : AbstractResource, IPasswordGrantAuthenticationAttempt
    {
        private static readonly string LoginPropertyName = "username";
        private static readonly string PasswordPropertyName = "password";
        private static readonly string AccountStoreHrefPropertyName = "accountStore";
        private static readonly string GrantTypePropertyName = "grant_type";
        private const string OrganizationNameKeyPropertyName = "organizationNameKey";


        public DefaultPasswordGrantAuthenticationAttempt(ResourceData data)
            : base(data)
        {
            this.SetProperty(GrantTypePropertyName, "password");
        }

        string IPasswordGrantAuthenticationAttempt.AccountStoreHref
            => this.GetStringProperty(AccountStoreHrefPropertyName);

        string IPasswordGrantAuthenticationAttempt.Login
            => this.GetStringProperty(LoginPropertyName);

        string IPasswordGrantAuthenticationAttempt.Password
            => this.GetStringProperty(PasswordPropertyName);

        string IPasswordGrantAuthenticationAttempt.OrganizationNameKey
            => GetStringProperty(OrganizationNameKeyPropertyName);

        void IPasswordGrantAuthenticationAttempt.SetLogin(string login)
            => this.SetProperty(LoginPropertyName, login);

        void IPasswordGrantAuthenticationAttempt.SetPassword(string password)
            => this.SetProperty(PasswordPropertyName, password);

        void IPasswordGrantAuthenticationAttempt.SetAccountStore(IAccountStore accountStore)
            => this.SetProperty(AccountStoreHrefPropertyName, accountStore.Href);

        void IPasswordGrantAuthenticationAttempt.SetAccountStore(string accountStoreHref)
            => this.SetProperty(AccountStoreHrefPropertyName, accountStoreHref);

        void IPasswordGrantAuthenticationAttempt.SetOrganizationNameKey(string nameKey)
            => SetProperty(OrganizationNameKeyPropertyName, nameKey);
    }
}
