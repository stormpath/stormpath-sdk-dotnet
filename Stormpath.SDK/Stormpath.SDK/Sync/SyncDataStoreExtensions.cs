// <copyright file="SyncDataStoreExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IDataStore"/>.
    /// </summary>
    public static class SyncDataStoreExtensions
    {
        /// <summary>
        /// Synchronously retrieves the resource at the specified <paramref name="href"/> URL synchronously and returns the resource
        /// as an instance of the specified class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="Resource.IResource">Resource</see> value.</typeparam>
        /// <param name="dataStore">The object implementing the <see cref="IDataStore"/> interface.</param>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        public static T GetResource<T>(this IDataStore dataStore, string href)
            => (dataStore as IDataStoreSync).GetResource<T>(href);

        /// <summary>
        /// Synchronously retrieves the resource at the specified <paramref name="href"/> URL with the specified <paramref name="responseOptions"/>,
        /// and returns the resource as an instance of the specified class <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned <see cref="IResource">Resource</see> value.</typeparam>
        /// <param name="dataStore">The object implementing the <see cref="IDataStore"/> interface.</param>
        /// <param name="href">The resource URL of the resource to retrieve.</param>
        /// <param name="responseOptions">The options to apply to this request.</param>
        /// <returns>An instance of the specified class based on data returned from the specified <paramref name="href"/> URL.</returns>
        public static T GetResource<T>(this IDataStore dataStore, string href, Action<IRetrievalOptions<T>> responseOptions)
            => (dataStore as IDataStoreSync).GetResource<T>(href, responseOptions);
    }
}
