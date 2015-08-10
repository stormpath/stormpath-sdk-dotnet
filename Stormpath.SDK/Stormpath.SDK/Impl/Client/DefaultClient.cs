// <copyright file="DefaultClient.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultClient : IClient
    {
        private readonly string baseUrl;
        private readonly AuthenticationScheme authScheme;
        private readonly int connectionTimeout;

        public DefaultClient(IClientApiKey apiKey, string baseUrl, AuthenticationScheme authenticationScheme, int timeout)
        {
            if (apiKey == null || !apiKey.IsValid()) throw new ArgumentException("API Key is not valid.");
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException("Base URL cannot be empty.");
            if (timeout < 0) throw new ArgumentException("Timeout cannot be negative.");

            this.baseUrl = baseUrl;
            this.authScheme = authenticationScheme;
            this.connectionTimeout = timeout;

            // TODO
        }

        AuthenticationScheme IClient.AuthenticationScheme
        {
            get { return this.authScheme; }
        }

        string IClient.BaseUrl
        {
            get { return this.baseUrl; }
        }

        int IClient.ConnectionTimeout
        {
            get { return this.connectionTimeout; }
        }

        Task<IApplication> ITenantActions.CreateApplicationAsync(IApplication application)
        {
            throw new NotImplementedException();
        }

        Task<IDirectory> ITenantActions.CreateDirectoryAsync(IDirectory directory)
        {
            throw new NotImplementedException();
        }

        Task<IAccountList> ITenantActions.GetAccountsAsync()
        {
            throw new NotImplementedException();
        }

        IApplicationList ITenantActions.GetApplications()
        {
            throw new NotImplementedException();
        }

        Task<IApplicationList> ITenantActions.GetApplicationsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IDirectoryList> ITenantActions.GetDirectoriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<IGroupList> ITenantActions.GetGroupsAsync()
        {
            throw new NotImplementedException();
        }

        Task<IAccount> ITenantActions.VerifyAccountEmailAsync()
        {
            throw new NotImplementedException();
        }

        Task<ITenant> IClient.GetCurrentTenantAsync()
        {
            throw new NotImplementedException();
        }
    }
}
