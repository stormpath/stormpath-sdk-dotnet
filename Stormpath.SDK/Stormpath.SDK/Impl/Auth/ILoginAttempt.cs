// <copyright file="ILoginAttempt.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Auth
{
    /// <summary>
    /// Represents an abstract login attempt against an Account Store.
    /// </summary>
    internal interface ILoginAttempt : IResource
    {
        /// <summary>
        /// Gets the login attempt type.
        /// </summary>
        /// <value>The login attempt type.</value>
        string Type { get; }

        /// <summary>
        /// Gets the <see cref="IAccountStore">Account Store</see> this login will be performed against.
        /// </summary>
        /// <value>The Account Store this login will be performed against.
        /// If <see langword="null"/>, the login request will use the default Stormpath login flow.</value>
        IEmbeddedProperty AccountStore { get; }

        /// <summary>
        /// Sets the <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        void SetType(string type);

        /// <summary>
        /// Sets the <see cref="AccountStore"/>.
        /// </summary>
        /// <param name="accountStore">The Account Store.</param>
        void SetAccountStore(IAccountStore accountStore);

        /// <summary>
        /// Sets the <see cref="AccountStore"/> by <c>href</c> or <c>nameKey</c>.
        /// </summary>
        /// <param name="hrefOrNameKey">The AccountStore href or Organization nameKey.</param>
        void SetAccountStore(string hrefOrNameKey);
    }
}
