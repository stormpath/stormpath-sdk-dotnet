// <copyright file="IRequestAuthenticatorFactory.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Client;

namespace Stormpath.SDK.Impl.Http.Authentication
{
    /// <summary>
    /// Represents a factory that can create the appropriate <see cref="IRequestAuthenticator"/>
    /// given an <see cref="AuthenticationScheme"/>.
    /// </summary>
    internal interface IRequestAuthenticatorFactory
    {
        /// <summary>
        /// Creates the appropriate <see cref="IRequestAuthenticator" /> for this <see cref="AuthenticationScheme"/>.
        /// </summary>
        /// <param name="scheme">The authentication scheme.</param>
        /// <returns>A new <see cref="IRequestAuthenticator"/> instance.</returns>
        IRequestAuthenticator Create(AuthenticationScheme scheme);
    }
}
