// <copyright file="DefaultGrantAuthenticationAttempt.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultGrantAuthenticationAttempt : AbstractResource, IGrantAuthenticationAttempt
    {
        private static readonly string LoginPropertyName = "username";
        private static readonly string PasswordPropertyName = "password";
        private static readonly string AccountStoreHrefPropertyName = "accountStore";
        private static readonly string GrantTypePropertyName = "grant_type";

        public DefaultGrantAuthenticationAttempt(ResourceData data)
            : base(data)
        {
        }

        string IGrantAuthenticationAttempt.AccountStoreHref
            => this.GetStringProperty(AccountStoreHrefPropertyName);

        string IGrantAuthenticationAttempt.GrantType
            => this.GetStringProperty(GrantTypePropertyName);

        string IGrantAuthenticationAttempt.Login
            => this.GetStringProperty(LoginPropertyName);

        string IGrantAuthenticationAttempt.Password
            => this.GetStringProperty(PasswordPropertyName);

        void IGrantAuthenticationAttempt.SetLogin(string login)
            => this.SetProperty(LoginPropertyName, login);

        void IGrantAuthenticationAttempt.SetPassword(string password)
            => this.SetProperty(PasswordPropertyName, password);

        void IGrantAuthenticationAttempt.SetAccountStore(IAccountStore accountStore)
            => this.SetProperty(AccountStoreHrefPropertyName, accountStore.Href);

        void IGrantAuthenticationAttempt.SetAccountStore(string accountStoreHref)
            => this.SetProperty(AccountStoreHrefPropertyName, accountStoreHref);

        void IGrantAuthenticationAttempt.SetGrantType(string grantType)
            => this.SetProperty(GrantTypePropertyName, grantType);
    }
}
