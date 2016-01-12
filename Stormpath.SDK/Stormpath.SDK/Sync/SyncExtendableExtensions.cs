// <copyright file="SyncExtendableExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IExtendable"/>.
    /// </summary>
    public static class SyncExtendableExtensions
    {
        /// <summary>
        /// Synchronously gets the custom data associated with this resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>The <see cref="ICustomData"/> associated with this resource.</returns>
        /// <exception cref="Error.ResourceException">The custom data could not be loaded.</exception>
        public static ICustomData GetCustomData(this IExtendable resource)
            => (resource as IExtendableSync).GetCustomData();
    }
}
