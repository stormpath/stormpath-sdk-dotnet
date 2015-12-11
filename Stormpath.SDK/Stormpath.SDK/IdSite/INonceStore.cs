// <copyright file="INonceStore.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    ///  Store <see cref="INonce"/> values and provides methods to check if a nonce has already been used, and to
    /// add it to the <see cref="INonceStore"/> to ensure that the same nonce cannot be used again.
    /// </summary>
    /// <remarks>
    /// <see cref="INonceStore"/> implementations <b>MUST</b> support time to live (TTL) policies and automatic eviction of entries
    /// * that are older than a configured TTL.If an implementation does not support TTL eviction, the store will fill up
    /// indefinitely over time, likely causing storage errors.
    /// <para>
    /// Because of the TTL requirement, most NonceStore implementations delegate to a Caching API that supports TTL eviction.
    /// </para>
    /// <para>
    /// <b>NOTE:</b>If you enable caching in the Stormpath SDK, the SDK will automatically enable a default cache-based
    /// NonceStore implementation for you. Just ensure that your caching configuration uses a default cache TTL slightly
    /// greater than 1 minute (the valid lifespan of a ID Site reply message).
    /// </para>
    /// </remarks>
    public interface INonceStore
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="INonceStore"/> instance supports synchronous operations.
        /// </summary>
        /// <value>
        /// For any objects implementing <see cref="ISynchronousNonceStore"/>, this should return <see langword="true"/>.
        /// </value>
        bool IsSynchronousSupported { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="INonceStore"/> instance supports asynchronous operations.
        /// </summary>
        /// <value>
        /// For any objects implementing <see cref="IAsynchronousNonceStore"/>, this should return <see langword="true"/>.
        /// </value>
        bool IsAsynchronousSupported { get; }
    }
}
