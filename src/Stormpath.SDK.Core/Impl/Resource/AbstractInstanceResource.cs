// <copyright file="AbstractInstanceResource.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Resource
{
    internal abstract class AbstractInstanceResource : AbstractResource, IAuditable
    {
        public static readonly string CreatedAtPropertyName = "createdAt";
        public static readonly string ModifiedAtPropertyName = "modifiedAt";

        protected AbstractInstanceResource(ResourceData data)
            : base(data)
        {
        }

        public DateTimeOffset CreatedAt => this.GetDateTimeProperty(CreatedAtPropertyName).Value;

        public DateTimeOffset ModifiedAt => this.GetDateTimeProperty(ModifiedAtPropertyName).Value;

        protected virtual Task<T> SaveAsync<T>(CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>
        {
            return this.GetInternalAsyncDataStore().SaveAsync(this as T, cancellationToken);
        }

        protected virtual Task<T> SaveAsync<T>(Action<IRetrievalOptions<T>> options, CancellationToken cancellationToken)
            where T : class, IResource, ISaveable<T>
        {
            var optionsInstance = new DefaultRetrievalOptions<T>();
            options(optionsInstance);
            var queryString = optionsInstance.ToString();

            return this.GetInternalAsyncDataStore().SaveAsync(this as T, queryString, cancellationToken);
        }

        protected virtual T Save<T>()
            where T : class, IResource, ISaveable<T>
        {
            return this.GetInternalSyncDataStore().Save(this as T);
        }

        protected virtual T Save<T>(Action<IRetrievalOptions<T>> options)
            where T : class, IResource, ISaveable<T>
        {
            var optionsInstance = new DefaultRetrievalOptions<T>();
            options(optionsInstance);
            var queryString = optionsInstance.ToString();

            return this.GetInternalSyncDataStore().Save(this as T, queryString);
        }
    }
}
