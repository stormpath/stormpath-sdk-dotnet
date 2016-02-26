// <copyright file="IEntryLink.cs" company="Stormpath, Inc.">
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
//
// Contains code modified from aspnet/Caching. Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

#if NET451
using System;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    /// <summary>
    /// Used to flow expiration information from one entry to another. Minimum absolute
    /// expiration will be copied from the dependent entry to the parent entry. The parent entry will not expire if the
    /// dependent entry is removed manually, removed due to memory pressure, or expires due to sliding expiration.
    /// </summary>
    internal interface IEntryLink : IDisposable
    {
        /// <summary>
        /// Gets the minimum absolute expiration for all dependent entries, or null if not set.
        /// </summary>
        DateTimeOffset? AbsoluteExpiration { get; }

        /// <summary>
        /// Sets the absolute expiration for from a dependent entry. The minimum value across all dependent entries
        /// will be used.
        /// </summary>
        /// <param name="absoluteExpiration"></param>
        void SetAbsoluteExpiration(DateTimeOffset absoluteExpiration);
    }
}
#endif