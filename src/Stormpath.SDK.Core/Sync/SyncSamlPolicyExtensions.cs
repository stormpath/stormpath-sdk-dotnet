// <copyright file="SyncSamlPolicyExtensions.cs" company="Stormpath, Inc.">
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
    /// Provides synchronous access to the methods available on <see cref="ISamlPolicy"/>.
    /// </summary>
    public static class SyncSamlPolicyExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="ISamlServiceProvider">SAML Service Provider</see> resource associated with this <see cref="ISamlPolicy">SAML Policy</see>.
        /// </summary>
        /// <param name="samlPolicy">The SAML Policy.</param>
        /// <returns>The <see cref="ISamlServiceProvider">SAML Service Provider</see>.</returns>
        public static ISamlServiceProvider GetSamlServiceProvider(this ISamlPolicy samlPolicy)
            => (samlPolicy as ISamlPolicySync).GetSamlServiceProvider();
    }
}
