// <copyright file="DefaultResourceFactory.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceFactory : IResourceFactory
    {
        private readonly IInternalDataStore dataStore;
        private readonly IdentityMap<string, AbstractResource> identityMap;
        private readonly ResourceTypeLookup typeLookup;
        private bool isDisposed = false; // To detect redundant calls

        public DefaultResourceFactory(IInternalDataStore dataStore, IdentityMap<string, AbstractResource> identityMap)
        {
            this.dataStore = dataStore;
            this.identityMap = identityMap;

            this.typeLookup = new ResourceTypeLookup();
        }

        private IResourceFactory AsInterface => this;

        T IResourceFactory.Create<T>()
            => (T)this.AsInterface.Create(typeof(T), null);

        object IResourceFactory.Create(Type type)
            => this.AsInterface.Create(type, null);

        T IResourceFactory.Create<T>(IDictionary<string, object> properties)
            => (T)this.AsInterface.Create(typeof(T), properties);

        object IResourceFactory.Create(Type type, IDictionary<string, object> properties)
        {
            bool isCollection =
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(CollectionResponsePage<>);
            if (isCollection)
                return this.InstantiateCollection(type, properties);

            return this.InstantiateSingle(type, properties);
        }

        private object InstantiateSingle(Type type, IDictionary<string, object> properties)
        {
            var targetType = this.typeLookup.GetConcrete(type);
            if (targetType == null)
                throw new ApplicationException($"Unknown resource type {type.Name}");

            AbstractResource targetObject;
            try
            {
                string id = RandomResourceId();

                object href = null;
                bool propertiesContainsHref =
                    properties != null &&
                    properties.TryGetValue("href", out href) &&
                    href != null;
                if (propertiesContainsHref)
                    id = href.ToString();

                targetObject = this.identityMap.GetOrAdd(id, () => Activator.CreateInstance(targetType, new object[] { this.dataStore }) as AbstractResource);

                if (properties != null)
                    targetObject.ResetAndUpdate(properties);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error creating resource type {targetType.Name}", e);
            }

            return targetObject;
        }

        private object InstantiateCollection(Type collectionType, IDictionary<string, object> properties)
        {
            Type innerType = this.typeLookup.GetInnerCollectionInterface(collectionType);
            var targetType = this.typeLookup.GetConcrete(innerType);
            if (innerType == null || targetType == null)
                throw new ApplicationException($"Error creating collection resource: unknown inner type '{innerType?.Name}'.");

            if (properties == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: no properties to materialize with.");

            long offset, limit, size;
            try
            {
                offset = Convert.ToInt64(properties["offset"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'offset' value.", ex);
            }

            try
            {
                limit = Convert.ToInt64(properties["limit"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'limit' value.", ex);
            }

            try
            {
                size = Convert.ToInt64(properties["size"]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'size' value.", ex);
            }

            var href = properties["href"]?.ToString();
            if (string.IsNullOrEmpty(href))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'href' value.");

            var items = properties["items"] as IEnumerable<IDictionary<string, object>>;
            if (items == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: items subcollection is invalid.");

            try
            {
                Type listOfInnerType = typeof(List<>).MakeGenericType(innerType);
                var materializedItems = listOfInnerType.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
                var addMethod = listOfInnerType.GetMethod("Add", new Type[] { innerType });

                foreach (var itemMap in items)
                {
                    var materialized = this.InstantiateSingle(innerType, itemMap);
                    addMethod.Invoke(materializedItems, new object[] { materialized });
                }

                object targetObject;
                targetObject = Activator.CreateInstance(collectionType, new object[] { href, offset, limit, size, materializedItems });

                return targetObject;
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: failed to add items to collection.", e);
            }
        }

        private static string RandomResourceId()
            => $"autogen://{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", string.Empty)}";

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.identityMap.Dispose();
                }

                this.isDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
