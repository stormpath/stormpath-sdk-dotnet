﻿// <copyright file="IDirectorySync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Directory
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IDirectory">Directory</see>.
    /// </summary>
    internal interface IDirectorySync :
        ISaveableWithOptionsSync<IDirectory>,
        IDeletableSync,
        IExtendableSync,
        IAccountCreationActionsSync,
        IGroupCreationActionsSync,
        IAccountStoreSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IDirectory.GetProviderAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Provider for this Directory.</returns>
        IProvider GetProvider();

        /// <summary>
        /// Synchronous counterpart to <see cref="IDirectory.GetAccountCreationPolicyAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Account Creation Policy for this Directory.</returns>
        IAccountCreationPolicy GetAccountCreationPolicy();

        /// <summary>
        /// Synchronous counterpart to <see cref="IDirectory.GetPasswordPolicyAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Password Policy for this Directory.</returns>
        IPasswordPolicy GetPasswordPolicy();
    }
}
