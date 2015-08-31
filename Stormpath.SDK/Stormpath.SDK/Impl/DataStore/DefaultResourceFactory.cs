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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class DefaultResourceFactory : IResourceFactory
    {
        private readonly Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>()
        {
            { typeof(IAccount), typeof(DefaultAccount) },
            { typeof(IApplication), typeof(DefaultApplication) },
            { typeof(ITenant), typeof(DefaultTenant) },
            { typeof(IDirectory), typeof(DefaultDirectory) },
            { typeof(IGroup), typeof(DefaultGroup) },
        };

        private readonly IInternalDataStore dataStore;

        public DefaultResourceFactory(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        private IResourceFactory IThis => this;

        T IResourceFactory.Create<T>()
        {
            return this.IThis.Create<T>(null);
        }

        T IResourceFactory.Create<T>(Hashtable properties)
        {
            bool isCollection = typeof(T).IsGenericType
                && typeof(T).GetGenericTypeDefinition() == typeof(CollectionResponsePage<>);
            if (isCollection)
                return InstantiateCollection<T>(properties);

            return InstantiateSingle<T>(properties);
        }

        private T InstantiateSingle<T>(Hashtable properties)
        {
            return (T)this.InstantiateSingle(properties, typeof(T));
        }

        private object InstantiateSingle(Hashtable properties, Type type)
        {
            Type targetType;
            if (!this.typeMap.TryGetValue(type, out targetType))
                throw new ApplicationException($"Unknown resource type {type.Name}");

            object targetObject;
            try
            {
                if (properties == null)
                    targetObject = Activator.CreateInstance(targetType, new object[] { this.dataStore });
                else
                    targetObject = Activator.CreateInstance(targetType, new object[] { this.dataStore, properties });
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error creating resource type {targetType.Name}", e);
            }

            return targetObject;
        }

        private T InstantiateCollection<T>(Hashtable properties)
        {
            var outerType = typeof(T); // CollectionResponsePage<TInner>

            Type innerType = outerType.GetGenericArguments().SingleOrDefault();
            if (innerType == null || !this.typeMap.ContainsKey(innerType))
                throw new ApplicationException($"Error creating collection resource: unknown inner type {outerType.GetGenericArguments().SingleOrDefault()?.Name}.");

            if (properties == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: no properties to materialize with.");

            int offset, limit, size;
            if (!int.TryParse(properties["offset"]?.ToString(), out offset))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'offset' value.");
            if (!int.TryParse(properties["limit"]?.ToString(), out limit))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'limit' value.");
            if (!int.TryParse(properties["size"]?.ToString(), out size))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'size' value.");

            var href = properties["href"]?.ToString();
            if (string.IsNullOrEmpty(href))
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: invalid 'href' value.");

            var items = properties["items"] as List<Hashtable>;
            if (items == null)
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: items subcollection is invalid.");

            try
            {
                Type listOfInnerType = typeof(List<>).MakeGenericType(innerType);
                var materializedItems = listOfInnerType.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
                var addMethod = listOfInnerType.GetMethod("Add", new Type[] { innerType });

                foreach (var item in items)
                {
                    var materialized = this.InstantiateSingle(item, innerType);
                    addMethod.Invoke(materializedItems, new object[] { materialized });
                }

                object targetObject;
                targetObject = Activator.CreateInstance(outerType, new object[] { href, offset, limit, size, materializedItems });

                return (T)targetObject;
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Unable to create collection resource of type {innerType.Name}: failed to add items to collection.", e);
            }
        }
    }
}
