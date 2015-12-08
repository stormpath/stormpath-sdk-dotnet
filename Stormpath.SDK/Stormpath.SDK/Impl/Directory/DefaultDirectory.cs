// <copyright file="DefaultDirectory.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed partial class DefaultDirectory : AbstractExtendableInstanceResource, IDirectory, IDirectorySync
    {
        private static readonly string AccountCreationPolicyPropertyName = "accountCreationPolicy";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string ApplicationMappingsPropertyName = "applicationMappings";
        private static readonly string ApplicationsPropertyName = "applications";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string GroupsPropertyName = "groups";
        private static readonly string NamePropertyName = "name";
        private static readonly string PasswordPolicyPropertyName = "passwordPolicy";
        private static readonly string ProviderPropertyName = "provider";
        private static readonly string StatusPropertyName = "status";

        public DefaultDirectory(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty AccountCreationPolicy => this.GetLinkProperty(AccountCreationPolicyPropertyName);

        internal IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty ApplicationMappings => this.GetLinkProperty(ApplicationMappingsPropertyName);

        internal IEmbeddedProperty Applications => this.GetLinkProperty(ApplicationsPropertyName);

        string IDirectory.Description => this.GetProperty<string>(DescriptionPropertyName);

        internal IEmbeddedProperty Groups => this.GetLinkProperty(GroupsPropertyName);

        string IDirectory.Name => this.GetProperty<string>(NamePropertyName);

        internal IEmbeddedProperty PasswordPolicy => this.GetLinkProperty(PasswordPolicyPropertyName);

        internal IEmbeddedProperty Provider => this.GetLinkProperty(ProviderPropertyName);

        DirectoryStatus IDirectory.Status => this.GetProperty<DirectoryStatus>(StatusPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        IDirectory IDirectory.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IDirectory IDirectory.SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IDirectory IDirectory.SetStatus(DirectoryStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        internal IDirectory SetProvider(IProvider provider)
        {
            if (!string.IsNullOrEmpty(this.AsInterface.Href))
            {
                throw new ApplicationException("Cannot change the provider of an existing Directory");
            }

            this.SetProperty(ProviderPropertyName, provider);
            return this;
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);

        Task<IDirectory> ISaveable<IDirectory>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IDirectory>(cancellationToken);

        IDirectory ISaveableSync<IDirectory>.Save()
            => this.Save<IDirectory>();

        Task<IDirectory> ISaveableWithOptions<IDirectory>.SaveAsync(Action<IRetrievalOptions<IDirectory>> options, CancellationToken cancellationToken)
             => this.SaveAsync(options, cancellationToken);

        IDirectory ISaveableWithOptionsSync<IDirectory>.Save(Action<IRetrievalOptions<IDirectory>> options)
             => this.Save(options);
    }
}
