// <copyright file="DefaultGroup.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Group
{
    internal sealed partial class DefaultGroup : AbstractExtendableInstanceResource, IGroup, IGroupSync
    {
        private static readonly string NamePropertyName = "name";
        private static readonly string DescriptionPropertyName = "description";
        private static readonly string StatusPropertyName = "status";
        private static readonly string DirectoryPropertyName = "directory";
        private static readonly string AccountsPropertyName = "accounts";
        private static readonly string AccountMembershipsPropertyName = "accountMemberships";

        public DefaultGroup(ResourceData data)
            : base(data)
        {
        }

        private new IGroup AsInterface => this;

        string IGroup.Name => this.GetProperty<string>(NamePropertyName);

        string IGroup.Description => this.GetProperty<string>(DescriptionPropertyName);

        GroupStatus IGroup.Status => this.GetProperty<GroupStatus>(StatusPropertyName);

        internal IEmbeddedProperty Directory => this.GetLinkProperty(DirectoryPropertyName);

        internal IEmbeddedProperty Tenant => this.GetLinkProperty(TenantPropertyName);

        internal IEmbeddedProperty Accounts => this.GetLinkProperty(AccountsPropertyName);

        internal IEmbeddedProperty AccountMemberships => this.GetLinkProperty(AccountMembershipsPropertyName);

        IGroup IGroup.SetDescription(string description)
        {
            this.SetProperty(DescriptionPropertyName, description);
            return this;
        }

        IGroup IGroup.SetName(string name)
        {
            this.SetProperty(NamePropertyName, name);
            return this;
        }

        IGroup IGroup.SetStatus(GroupStatus status)
        {
            this.SetProperty(StatusPropertyName, status);
            return this;
        }

        Task<IGroup> ISaveable<IGroup>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IGroup>(cancellationToken);

        IGroup ISaveableSync<IGroup>.Save()
            => this.Save<IGroup>();

        Task<IGroup> ISaveableWithOptions<IGroup>.SaveAsync(Action<IRetrievalOptions<IGroup>> options, CancellationToken cancellationToken)
             => this.SaveAsync(options, cancellationToken);

        IGroup ISaveableWithOptionsSync<IGroup>.Save(Action<IRetrievalOptions<IGroup>> options)
             => this.Save(options);

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
