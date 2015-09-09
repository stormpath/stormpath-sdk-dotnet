// <copyright file="ResourceTypeLookup.cs" company="Stormpath, Inc.">
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
        private static readonly Type AccountInterface = typeof(IAccount);
        private static readonly Type ApplicationInterface = typeof(IApplication);
        private static readonly Type TenantInterface = typeof(ITenant);
        private static readonly Type DirectoryInterface = typeof(IDirectory);
        private static readonly Type GroupInterface = typeof(IGroup);
        private static readonly Type AccountStoreMappingInterface = typeof(IAccountStoreMapping);
        private static readonly Type AccountStoreInterface = typeof(IAccountStore);
        private static readonly Type BasicLoginAttemptInterface = typeof(IBasicLoginAttempt);
        private static readonly Type AuthenticationResultInterface = typeof(IAuthenticationResult);
        private static readonly Type PasswordResetTokenInterface = typeof(IPasswordResetToken);

        private static readonly Type AccountConcrete = typeof(DefaultAccount);
        private static readonly Type ApplicationConcrete = typeof(DefaultApplication);
        private static readonly Type TenantConcrete = typeof(DefaultTenant);
        private static readonly Type DirectoryConcrete = typeof(DefaultDirectory);
        private static readonly Type GroupConcrete = typeof(DefaultGroup);
        private static readonly Type AccountStoreMappingConcrete = typeof(DefaultAccountStoreMapping);
        private static readonly Type AccountStoreConcrete = typeof(DefaultAccountStore);
        private static readonly Type BasicLoginAttemptConcrete = typeof(DefaultBasicLoginAttempt);
        private static readonly Type AuthenticationResultConcrete = typeof(DefaultAuthenticationResult);
        private static readonly Type PasswordResetTokenConcrete = typeof(DefaultPasswordResetToken);

        private static readonly Type CollectionPageOfAccount = typeof(CollectionResponsePage<IAccount>);
        private static readonly Type CollectionPageOfApplication = typeof(CollectionResponsePage<IApplication>);
        private static readonly Type CollectionPageOfDirectory = typeof(CollectionResponsePage<IDirectory>);
        private static readonly Type CollectionPageOfGroup = typeof(CollectionResponsePage<IGroup>);

        /// <summary>
        /// Fast lookups of concrete types from their interfaces.
        /// </summary>
        /// <param name="iface">A resource interface (e.g., <see cref="IAccount"/>)</param>
        /// <returns>The associated concrete instance class type (e.g., <see cref="DefaultAccount"/>)</returns>
        private static Type GetConcreteTypeForInterface(Type iface)
        {
            if (iface == AccountInterface)
                return AccountConcrete;

            if (iface == ApplicationInterface)
                return ApplicationConcrete;

            if (iface == TenantInterface)
                return TenantConcrete;

            if (iface == DirectoryInterface)
                return DirectoryConcrete;

            if (iface == GroupInterface)
                return GroupConcrete;

            if (iface == AccountStoreMappingInterface)
                return AccountStoreMappingConcrete;

            if (iface == AccountStoreInterface)
                return AccountStoreConcrete;

            if (iface == BasicLoginAttemptInterface)
                return BasicLoginAttemptConcrete;

            if (iface == AuthenticationResultInterface)
                return AuthenticationResultConcrete;

            if (iface == PasswordResetTokenInterface)
                return PasswordResetTokenConcrete;

            return null; // unknown
        }

        /// <summary>
        /// Fast lookups of concrete types from their interfaces.
        /// </summary>
        /// <param name="concrete">A concrete instance class type (e.g., <see cref="DefaultAccount"/>)</param>
        /// <returns>The associated interface type (e.g., <see cref="IAccount"/>)</returns>
        private static Type GetInterfaceForConcreteType(Type concrete)
        {
            if (concrete == AccountConcrete)
                return AccountInterface;

            if (concrete == ApplicationConcrete)
                return ApplicationInterface;

            if (concrete == TenantConcrete)
                return TenantInterface;

            if (concrete == DirectoryConcrete)
                return DirectoryInterface;

            if (concrete == GroupConcrete)
                return GroupInterface;

            if (concrete == AccountStoreMappingConcrete)
                return AccountStoreMappingInterface;

            if (concrete == AccountStoreConcrete)
                return AccountStoreInterface;

            if (concrete == BasicLoginAttemptConcrete)
                return BasicLoginAttemptInterface;

            if (concrete == AuthenticationResultConcrete)
                return AuthenticationResultInterface;

            if (concrete == PasswordResetTokenConcrete)
                return PasswordResetTokenInterface;

            return null; // unknown
        }

        /// <summary>
        /// Checks whether this type is a known resource interface (e.g. <see cref="IAccount"/>).
        /// </summary>
        /// <param name="possiblyInterface">The type to check</param>
        /// <returns>True if this type is a known resource interface</returns>
        private static bool IsInterface(Type possiblyInterface)
        {
            return
                possiblyInterface == AccountInterface ||
                possiblyInterface == ApplicationInterface ||
                possiblyInterface == TenantInterface ||
                possiblyInterface == DirectoryInterface ||
                possiblyInterface == GroupInterface ||
                possiblyInterface == AccountStoreMappingInterface ||
                possiblyInterface == AccountStoreInterface ||
                possiblyInterface == BasicLoginAttemptInterface ||
                possiblyInterface == AuthenticationResultInterface ||
                possiblyInterface == PasswordResetTokenInterface;
        }

        /// <summary>
        /// Checks whether this type is a known concrete instance class (e.g., <see cref="DefaultAccount"/>).
        /// </summary>
        /// <param name="possiblyConcrete">The type to check</param>
        /// <returns>True if this type is a known concrete type</returns>
        private static bool IsConcrete(Type possiblyConcrete)
        {
            return
                possiblyConcrete == AccountConcrete ||
                possiblyConcrete == ApplicationConcrete ||
                possiblyConcrete == TenantConcrete ||
                possiblyConcrete == DirectoryConcrete ||
                possiblyConcrete == GroupConcrete ||
                possiblyConcrete == AccountStoreMappingConcrete ||
                possiblyConcrete == AccountStoreConcrete ||
                possiblyConcrete == BasicLoginAttemptConcrete ||
                possiblyConcrete == AuthenticationResultConcrete ||
                possiblyConcrete == PasswordResetTokenConcrete;
        }

        /// <summary>
        /// Fast lookups of the inner interface from a collection type.
        /// </summary>
        /// <param name="collectionType">A <see cref="CollectionResponsePage{T}"/> containing some inner <see cref="IResource"/> interface (e.g. <see cref="CollectionResponsePage{IAccount}"/>).</param>
        /// <returns>The inner interface (e.g. <see cref="IAccount"/>)</returns>
        private static Type GetCollectionInnerTypeImpl(Type collectionType)
        {
            if (collectionType == CollectionPageOfAccount)
                return AccountInterface;

            if (collectionType == CollectionPageOfApplication)
                return ApplicationInterface;

            if (collectionType == CollectionPageOfDirectory)
                return DirectoryInterface;

            if (collectionType == CollectionPageOfGroup)
                return GroupInterface;

            return null; // unknown
        }

        public Type GetInterface<T>()
            where T : IResource
        {
            return this.GetInterface(typeof(T));
        }

        public Type GetInterface(Type possiblyConcrete)
        {
            if (IsInterface(possiblyConcrete))
                return possiblyConcrete;

            return GetInterfaceForConcreteType(possiblyConcrete);
        }

        public Type GetConcrete<T>()
            where T : IResource
        {
            return this.GetConcrete(typeof(T));
        }

        public Type GetConcrete(Type possiblyInterface)
        {
            if (IsConcrete(possiblyInterface))
                return possiblyInterface;

            return GetConcreteTypeForInterface(possiblyInterface);
        }

        public Type GetInnerCollectionInterface(Type collectionType)
        {
            return GetCollectionInnerTypeImpl(collectionType);
        }
    }
}
