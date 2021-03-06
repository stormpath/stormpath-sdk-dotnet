﻿// <copyright file="IProviderAccountResult.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Represents the result of the attempt to access the Provider's account. Stormpath maps common fields of the
    /// Provider user to the <see cref="IAccount">Account</see> Resource.
    /// </summary>
    /// <remarks>
    /// If the user retrieved from the Provider did not previously exist in Stormpath as an Account, common Provider user fields
    /// will be used to create a new <see cref="IAccount">Account</see> in Stormpath.
    /// </remarks>
    /// <seealso cref="Application.IApplication.GetAccountAsync(IProviderAccountRequest, System.Threading.CancellationToken)"/>
    public interface IProviderAccountResult : IResource
    {
        /// <summary>
        /// Gets the <see cref="IAccount">Account</see> Resource containing common fields of the Provider user in Stormpath.
        /// </summary>
        /// <value>The <see cref="IAccount">Account</see> Resource containing common fields of the Provider user in Stormpath.</value>
        IAccount Account { get; }

        /// <summary>
        /// Gets a value indicating whether this request generated a new account in Stormpath.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a new <see cref="IAccount">Account</see> was generated in Stormpath as a result of the request;
        /// <see langword="false"/> otherwise.
        /// </value>
        bool IsNewAccount { get; }
    }
}
