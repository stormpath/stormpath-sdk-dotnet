// <copyright file="SyncSamlServiceProviderExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Saml;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="ISamlServiceProvider"/>.
    /// </summary>
    public static class SyncSamlServiceProviderExtensions
    {
        /// <summary>
        /// Synchronously gets the endpoint resource used to initiate SAML-based Single Sign-On.
        /// </summary>
        /// <param name="samlServiceProvider">The <see cref="ISamlServiceProvider"/>.</param>
        /// <returns>The endpoint resource used to initiate SAML-based Single Sign-On.</returns>
        public static ISsoInitiationEndpoint GetSsoInitiationEndpoint(this ISamlServiceProvider samlServiceProvider)
            => (samlServiceProvider as ISamlServiceProviderSync).GetSsoInitiationEndpoint();
    }
}
