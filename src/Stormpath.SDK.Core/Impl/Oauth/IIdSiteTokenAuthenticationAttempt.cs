// <copyright file="IIdSiteTokenAuthenticationAttempt.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Oauth
{
    /// <summary>
    /// Represents the information required to build an ID Site Token Authentication request.
    /// </summary>
    [Obsolete("Remove in 1.0")]
    internal interface IIdSiteTokenAuthenticationAttempt : IResource
    {
        /// <summary>
        /// Gets the token that will be used for the exchange request.
        /// </summary>
        /// <value>The token that will be used for the exchange request.</value>
        string Token { get; }

        /// <summary>
        /// Sets the token that will be used for the exchange request.
        /// </summary>
        /// <param name="token">The string representation of the token.</param>
        void SetToken(string token);
    }
}