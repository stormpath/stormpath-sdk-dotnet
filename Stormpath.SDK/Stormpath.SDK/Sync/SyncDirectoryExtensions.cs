// <copyright file="SyncDirectoryExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncDirectoryExtensions
    {
        /// <summary>
        /// Synchronously creates a new <see cref="IGroup"/> instance in this directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="group">The group to create/persist.</param>
        /// <returns>The newly-created <see cref="IGroup"/>.</returns>
        public static IGroup CreateGroup(this IDirectory directory, IGroup group)
            => (directory as IDirectorySync).CreateGroup(group);

        /// <summary>
        /// Synchronously gets the <see cref="IProvider"/> of this Directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>The Provider of this Directory.</returns>
        public static IProvider GetProvider(this IDirectory directory)
            => (directory as IDirectorySync).GetProvider();

        /// <summary>
        /// Synchronously gets the Stormpath <see cref="ITenant"/> that owns this Directory resource.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>This account's tenant.</returns>
        public static ITenant GetTenant(this IDirectory directory)
            => (directory as IDirectorySync).GetTenant();
    }
}
