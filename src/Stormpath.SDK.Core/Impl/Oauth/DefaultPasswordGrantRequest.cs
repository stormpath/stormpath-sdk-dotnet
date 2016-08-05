// <copyright file="DefaultPasswordGrantRequest.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultPasswordGrantRequest : IPasswordGrantRequest
    {
        private readonly string login;
        private readonly string password;
        private string accountStoreHref;
        private string organizationNameKey;

        public DefaultPasswordGrantRequest(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        string IPasswordGrantRequest.Login => this.login;

        string IPasswordGrantRequest.Password => this.password;

        string IPasswordGrantRequest.AccountStoreHref => this.accountStoreHref;

        string IPasswordGrantRequest.OrganizationNameKey => organizationNameKey;

        public void SetAccountStore(string accountStoreHref)
        {
            this.accountStoreHref = accountStoreHref;
        }

        public void SetOrganizationNameKey(string nameKey)
        {
            this.organizationNameKey = nameKey;
        }
    }
}
