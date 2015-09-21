// <copyright file="IAccountStoreMappingSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;

namespace Stormpath.SDK.Impl.AccountStore
{
    /// <summary>
    /// Represents the assignment of an <see cref="IAccountStore"/> AccountStore (either a Group or <see cref="Directory.IDirectory"/>) to an <see cref="IApplication"/> that can be accessed synchronously.
    /// <para>When an <see cref="IAccountStoreMapping"/> is created, the accounts in the account store are granted access to become users of the linked <see cref="IApplication"/>. The order in which AccountStores are assigned to an application determines how login attempts work in Stormpath.</para>
    /// </summary>
    internal interface IAccountStoreMappingSync
    {
        /// <summary>
        /// Gets this mapping's <see cref="IAccountStore"/> (either a Group or <see cref="Directory.IDirectory"/>), to be assigned to the application.
        /// </summary>
        /// <returns>The mapping's <see cref="IAccountStore"/>.</returns>
        IAccountStore GetAccountStore();

        /// <summary>
        /// Gets the <see cref="IApplication"/> represented by this mapping.
        /// </summary>
        /// <returns>The mapping's <see cref="IApplication"/>.</returns>
        IApplication GetApplication();
    }
}
