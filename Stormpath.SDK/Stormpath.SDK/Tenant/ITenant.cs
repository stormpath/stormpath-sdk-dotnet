// <copyright file="ITenant.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Tenant
{
    /// <summary>
    /// A tenant represents a customer's private data 'space' within Stormpath that contains all of the customer's Stormpath-stored resources, like <see cref="Application.IApplication"/>s, <see cref="Directory.IDirectory"/>s, and <see cref="Account.IAccount"/>s.
    /// </summary>
    public interface ITenant : IResource, IAuditable, IExtendable, ITenantActions
    {
        /// <summary>
        /// Gets the tenant's globally-unique human-readable key in Stormpath.
        /// <para><b>This can change in the future. Do not rely on it as a permanent identifier.</b> If you need a permanent ID, use the href as the permanent ID (this is true for all resources, not just Tenant resources).</para>
        /// </summary>
        /// <value>This tenant's globally-unique key. <b>This can change. Do not rely on it as a permanent identifier.</b></value>
        string Key { get; }

        /// <summary>
        /// Gets the tenant's globally-unique name in Stormpath.
        /// </summary>
        /// <value>This tenant's globally-unique name. <b>This can change. Do not rely on it as a permanent identifier.</b></value>
        string Name { get; }
    }
}
