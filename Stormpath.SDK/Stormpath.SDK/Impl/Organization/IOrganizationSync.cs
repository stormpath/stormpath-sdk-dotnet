// <copyright file="IOrganizationSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.Organization
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IOrganization">Organization</see>.
    /// </summary>
    internal interface IOrganizationSync :
        IHasTenantSync,
        ISaveableWithOptionsSync<IOrganization>,
        IDeletableSync,
        IExtendableSync,
        IAccountStoreSync,
        IAccountStoreContainerSync<IOrganizationAccountStoreMapping>,
        IAccountCreationActionsSync,
        IGroupCreationActionsSync
    {
    }
}
