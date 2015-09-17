// <copyright file="IDirectory.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// A directory is a top-level container of <see cref="IAccount"/>s and groups.
    /// Accounts and groups are guaranteed to be unique within a directory, but not across multiple directories.
    /// <para>You can think of a <see cref="IDirectory"/> as an account 'store'.
    /// You can map one or more directories (or groups within a directory) to an <see cref="Application.IApplication"/>.
    /// This forms the application's effective 'user base' of all <see cref="IAccount"/> that may use the application.</para>
    /// </summary>
    public interface IDirectory : IResource, IDeletable, IAuditable
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
        /// Gets a queryable list of all accounts in this directory.
        /// </summary>
        /// <returns>An <see cref="IAsyncQueryable{IAccount}"/> that may be used to asynchronously list or search accounts.</returns>
        /// <example>
        ///     var allDirectoryAccounts = await myDirectory.GetAccounts().ToListAsync();
        /// </example>
        IAsyncQueryable<IAccount> GetAccounts();
    }
}
