﻿// <copyright file="DefaultDirectory.Resources.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed partial class DefaultDirectory
    {
        Task<IProvider> IDirectory.GetProviderAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IProvider>(this.Provider.Href, ProviderTypeConverter.TypeLookup, cancellationToken);

        Task<IAccountCreationPolicy> IDirectory.GetAccountCreationPolicyAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IAccountCreationPolicy>(this.AccountCreationPolicy.Href, cancellationToken);

        Task<IPasswordPolicy> IDirectory.GetPasswordPolicyAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().GetResourceAsync<IPasswordPolicy>(this.PasswordPolicy.Href, cancellationToken);
    }
}
