// <copyright file="ICacheConfiguration.cs" company="Stormpath, Inc.">
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

using System;

namespace Stormpath.SDK.Cache
{
    /// <summary>
    /// Represents configuration settings for a particular <see cref="ICache">Cache</see> region.
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Gets the name of the <see cref="ICache">Cache</see> for which this configuration applies.
        /// </summary>
        /// <value>The name of the <see cref="ICache">Cache</see> for which this configuration applies.</value>
        string Name { get; }

        /// <summary>
        /// Gets the Time to Live (TTL) setting to apply for all entries in the associated <see cref="ICache">Cache</see>.
        /// </summary>
        /// <value>The Time to Live (TTL) setting to apply for all entries in the associated <see cref="ICache">Cache</see>.</value>
        TimeSpan? TimeToLive { get; }

        /// <summary>
        /// Gets the Time to Live (TTI) setting to apply for all entries in the associated <see cref="ICache">Cache</see>.
        /// </summary>
        /// <value>The Time to Live (TTI) setting to apply for all entries in the associated <see cref="ICache">Cache</see>.</value>
        TimeSpan? TimeToIdle { get; }
    }
}
