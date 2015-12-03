// <copyright file="IProviderRequestFactory{A,P}.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Interface describing the builder capabilities that Providers implement in Stormpath.
    /// </summary>
    /// <typeparam name="A">An implementation of <see cref="IProviderAccountRequestBuilder{T}"/>.</typeparam>
    /// <typeparam name="P">An implementation of <see cref="ICreateProviderRequestBuilder{T}"/>.</typeparam>
    public interface IProviderRequestFactory<A, P>
        where A : IProviderAccountRequestBuilder<A>
        where P : ICreateProviderRequestBuilder<P>
    {
        /// <summary>
        /// Returns a builder to generate an attempt to create or retrieve a Provider <see cref="Account.IAccount"/> from Stormpath.
        /// </summary>
        /// <returns>A builder to generate an attempt to create or retrieve a Provider <see cref="Account.IAccount"/> from Stormpath.</returns>
        A Account();

        /// <summary>
        /// Returns a builder to generate an attempt to create a Provider-based <see cref="Directory.IDirectory"/> in Stormpath.
        /// </summary>
        /// <returns>A builder to generate an attempt to create a Provider-based <see cref="Directory.IDirectory"/> in Stormpath.</returns>
        P Builder();
    }
}
