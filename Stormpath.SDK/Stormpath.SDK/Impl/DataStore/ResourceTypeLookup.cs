// <copyright file="ResourceLookup.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class ResourceTypeLookup
    {
        /// <summary>
        /// Fast lookups of concrete types from an interface.
        /// </summary>
        private static readonly Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>()
        {
            { typeof(IAccount), typeof(DefaultAccount) },
            { typeof(IApplication), typeof(DefaultApplication) },
            { typeof(ITenant), typeof(DefaultTenant) },
            { typeof(IDirectory), typeof(DefaultDirectory) },
            { typeof(IGroup), typeof(DefaultGroup) },
            { typeof(IAccountStoreMapping), typeof(DefaultAccountStoreMapping) },
            { typeof(IAccountStore), typeof(DefaultAccountStore) },
            { typeof(IBasicLoginAttempt), typeof(DefaultBasicLoginAttempt) },
            { typeof(IAuthenticationResult), typeof(DefaultAuthenticationResult) },
            { typeof(IPasswordResetToken), typeof(DefaultPasswordResetToken) },
        };

        /// <summary>
        /// Fast lookups of the inner interface from a collection type.
        /// </summary>
        private static readonly Dictionary<Type, Type> CollectionTypeMap = new Dictionary<Type, Type>()
        {
            { typeof(CollectionResponsePage<IAccount>), typeof(IAccount) },
            { typeof(CollectionResponsePage<IApplication>), typeof(IApplication) },
            { typeof(CollectionResponsePage<IDirectory>), typeof(IDirectory) },
            { typeof(CollectionResponsePage<IGroup>), typeof(IGroup) },
        };

        public Type GetInterface<T>()
            where T : IResource
        {
            return this.GetInterface(typeof(T));
        }

        public Type GetInterface(Type possiblyConcrete)
        {
            bool alreadyIsInterface = TypeMap.ContainsKey(possiblyConcrete);
            if (alreadyIsInterface)
                return possiblyConcrete;

            bool isUnsupportedConcreteType = !TypeMap.ContainsValue(possiblyConcrete);
            if (isUnsupportedConcreteType)
                return null;

            var mapped = TypeMap
                .Where(x => x.Value == possiblyConcrete)
                .Single();
            return mapped.Key;
        }

        public Type GetConcrete<T>()
            where T : IResource
        {
            return this.GetConcrete(typeof(T));
        }

        public Type GetConcrete(Type possiblyInterface)
        {
            bool alreadyIsConcrete = TypeMap.ContainsValue(possiblyInterface);
            if (alreadyIsConcrete)
                return possiblyInterface;

            Type concrete = null;
            if (!TypeMap.TryGetValue(possiblyInterface, out concrete))
                return null;

            return concrete;
        }

        public Type GetInnerCollectionInterface(Type collectionType)
        {
            Type inner = null;
            if (!CollectionTypeMap.TryGetValue(collectionType, out inner))
            {
                return null;
            }

            return inner;
        }
    }
}
