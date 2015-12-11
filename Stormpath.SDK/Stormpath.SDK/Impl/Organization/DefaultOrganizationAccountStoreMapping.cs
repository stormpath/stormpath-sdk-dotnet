// <copyright file="DefaultOrganizationAccountStoreMapping.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.Organization
{
    internal sealed class DefaultOrganizationAccountStoreMapping :
        AbstractAccountStoreMapping<IOrganizationAccountStoreMapping>,
        IOrganizationAccountStoreMapping,
        IOrganizationAccountStoreMappingSync
    {
        private static readonly string OrganizationPropertyName = "organization";

        public DefaultOrganizationAccountStoreMapping(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Organization => this.GetLinkProperty(OrganizationPropertyName);

       IOrganizationAccountStoreMapping IOrganizationAccountStoreMapping.SetOrganization(IOrganization organization)
        {
            if (string.IsNullOrEmpty(organization?.Href))
            {
                throw new ArgumentNullException(nameof(organization.Href));
            }

            this.SetLinkProperty(OrganizationPropertyName, organization.Href);

            return this;
        }

        Task<IOrganization> IOrganizationAccountStoreMapping.GetOrganizationAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IOrganization>(this.Organization.Href, cancellationToken);

        IOrganization IOrganizationAccountStoreMappingSync.GetOrganization()
            => this.GetInternalSyncDataStore().GetResource<IOrganization>(this.Organization.Href);

        // TODO remove these methods on a breaking version change.
        public override IApplication GetApplication()
        {
            throw new NotSupportedException("Use IApplicationAccountStoreMapping.GetApplication instead.");
        }

        public override Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Use IApplicationAccountStoreMapping.GetApplicationAsync instead.");
        }

        public override IOrganizationAccountStoreMapping SetApplication(IApplication application)
        {
            throw new NotSupportedException("Use IApplicationAccountStoreMapping.SetApplication instead.");
        }
    }
}
