// <copyright file="IDirectory.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Group;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// A directory is a top-level container of <see cref="IAccount"/>s and <see cref="IGroup"/>s.
    /// Accounts and groups are guaranteed to be unique within a directory, but not across multiple directories.
    /// <para>You can think of a <see cref="IDirectory"/> as an account 'store'.
    /// You can map one or more directories (or groups within a directory) to an <see cref="Application.IApplication"/>.
    /// This forms the application's effective 'user base' of all <see cref="IAccount"/> that may use the application.</para>
    /// </summary>
    public interface IDirectory : IResource, ISaveableWithOptions<IDirectory>, IDeletable, IAuditable, IExtendable, IAccountStore, IAccountCreationActions, IGroupCreationActions
    {
        /// <summary>
        /// Gets the directory's name.
        /// </summary>
        /// <value>This directory's name. The name is guaranteed to be non-null and unique among all other directories in the owning <see cref="Tenant.ITenant"/>.</value>
        string Name { get; }

        /// <summary>
        /// Gets the directory's description.
        /// </summary>
        /// <value>This directory's description. This is an optional property and may be null or empty.</value>
        string Description { get; }

        /// <summary>
        /// Gets the directory's status.
        /// </summary>
        /// <value>
        /// This directory's status.
        /// An <see cref="DirectoryStatus.Enabled"/> directory may be used by applications to login accounts found within the directory.
        /// A <see cref="DirectoryStatus.Disabled"/> directory prevents its accounts from being used to login to applications.
        /// </value>
        DirectoryStatus Status { get; }

        /// <summary>
        /// Sets the directory description.
        /// </summary>
        /// <param name="description">The directory's description. This is an optional property and may be <see langword="null"/> or empty.</param>
        /// <returns>This instance for method chaining.</returns>
        IDirectory SetDescription(string description);

        /// <summary>
        /// Sets the directory's name.
        /// </summary>
        /// <param name="name">The directory's name. Directory names are required and must be unique within a <see cref="Tenant.ITenant"/>.</param>
        /// <returns>This instance for method chaining.</returns>
        IDirectory SetName(string name);

        /// <summary>
        /// Sets the directory's status.
        /// </summary>
        /// <param name="status">The directory's status.
        /// An <see cref="DirectoryStatus.Enabled"/> directory may be used by applications to login accounts found within the directory.
        /// A <see cref="DirectoryStatus.Disabled"/> directory prevents its accounts from being used to login to applications.
        /// </param>
        /// <returns>This instance for method chaining.</returns>
        IDirectory SetStatus(DirectoryStatus status);

        /// <summary>
        /// Gets the <see cref="IProvider"/> of this Directory.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Provider of this Directory.</returns>
        Task<IProvider> GetProviderAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a queryable list of all groups in this directory.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IGroup}"/> that may be used to asynchronously list or search groups.</returns>
        IAsyncQueryable<IGroup> GetGroups();
    }
}
