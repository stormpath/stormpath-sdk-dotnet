// <copyright file="IIdSiteSyncCallbackHandler.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    /// Handles HTTP replies sent from your ID Site to your application's <c>callbackUri</c>
    /// and returns an <see cref="IAccountResult"/>.
    /// </summary>
    public interface IIdSiteSyncCallbackHandler
    {
        /// <summary>
        /// Sets a <see cref="INonceStore"/> that will retain ID Site message identifiers as
        /// cryptographic nonces, eliminating the possibility of a replay attack. This ensures
        /// any ID Site message cannot be intercepted and used again later.
        /// <para>
        /// Because this ensures strong security, a {@code NonceStore} is enabled by default if
        /// you have caching enabled for the SDK: a cache region will be used to store nonces over time, and those nonces
        /// will be used to assert that ID Site replies are not used more than once.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Default Nonce Store: A <see cref="Cache.ICache{K, V}"/>-based <see cref="INonceStore"/> is enabled by default
        /// if you have caching enabled in the SDK. Because nonces are stored in a cache region, it is very important to
        /// ensure that the nonce store cache region has an entry TTL <em>longer</em> than the response message valid life span.
        /// For Stormpath, response messages are valid for 1 minute, so your default cache region settings must use a
        /// TTL longer than 1 minute (most caching system defaults are ~ 30 minutes or an hour, so odds are high that you're 'ok').
        /// </para>
        /// <para>
        /// Custom Nonce Store: If you have not enabled caching in the SDK, or you don't want to use your SDK cache as a nonce store,
        /// you can specify a custom instance via this method, and all ID Site reply identifiers will be stored there to
        /// prevent reuse, but note: your custom <see cref="INonceStore"/> implementation <em>MUST</em> support the notion of a
        /// TTL (Time-to-Live) and automatically evict entries older than the max age lifespan(again, 1 minute).
        /// </para>
        /// <para>
        /// If your <see cref="INonceStore"/> implementation does not support TTL auto-eviction, your store will fill up
        /// indefinitely, likely causing storage errors.
        /// </para>
        /// </remarks>
        /// <param name="nonceStore">The <see cref="INonceStore"/> implementation to use when processing this request.</param>
        /// <returns>This instance for method chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="nonceStore"/> is <c>null</c>.</exception>
        IIdSiteSyncCallbackHandler SetNonceStore(INonceStore nonceStore);

        /// <summary>
        /// Sets the <see cref="IIdSiteResultSyncListener"/> that will be notified about the actual operation
        /// of the ID Site invokation: registration, authentication, or logout.
        /// <para>
        /// The listener must be set before the method <see cref="GetAccountResult()"/> is invoked.
        /// </para>
        /// </summary>
        /// <param name="resultListener">The result listener to notify.</param>
        /// <returns>This instance for method chaining.</returns>
        IIdSiteSyncCallbackHandler SetResultListener(IIdSiteResultSyncListener resultListener);

        /// <summary>
        /// Synchronously processes the request and returns an <see cref="IAccountResult"/> object that reflects
        /// the account that logged in or registered.
        /// </summary>
        /// <returns>The resolved identity in the form of an <see cref="IAccountResult"/>.</returns>
        /// <exception cref="Jwt.InvalidJwtException">The returned token is invalid.</exception>
        /// <exception cref="ApplicationException">The current nonce store does not support synchronous operations.</exception>
        IAccountResult GetAccountResult();
    }
}
