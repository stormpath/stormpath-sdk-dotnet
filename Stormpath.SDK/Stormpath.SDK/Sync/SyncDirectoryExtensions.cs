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
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IDirectory"/>.
    /// </summary>
    public static class SyncDirectoryExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IProvider"/> of this Directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>The Provider of this Directory.</returns>
        public static IProvider GetProvider(this IDirectory directory)
            => (directory as IDirectorySync).GetProvider();
    }
}
