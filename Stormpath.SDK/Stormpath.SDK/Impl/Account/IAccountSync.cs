// <copyright file="IAccountSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.Account
{
    internal interface IAccountSync : ISaveableSync<IAccount>, IDeletableSync
    {
        /// <summary>
        /// Gets the account's parent <see cref="IDirectory"/> (where the account is stored).
        /// </summary>
        /// <returns>The directory.</returns>
        IDirectory GetDirectory();

        /// <summary>
        /// Gets the Stormpath <see cref="ITenant"/> that owns this Account resource.
        /// </summary>
        /// <returns>The tenant.</returns>
        ITenant GetTenant();
    }
}
