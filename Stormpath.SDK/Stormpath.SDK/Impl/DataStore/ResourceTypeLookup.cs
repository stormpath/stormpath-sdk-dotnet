// <copyright file="ResourceTypeLookup.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.CustomData;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.IdSite;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class ResourceTypeLookup
    {
        private static readonly IReadOnlyDictionary<Type, Type> ConcreteLookup = new Dictionary<Type, Type>()
        {
            [typeof(IAccount)] = typeof(DefaultAccount),
            [typeof(IApplication)] = typeof(DefaultApplication),
            [typeof(ITenant)] = typeof(DefaultTenant),
            [typeof(IDirectory)] = typeof(DefaultDirectory),
            [typeof(IGroup)] = typeof(DefaultGroup),
            [typeof(IGroupMembership)] = typeof(DefaultGroupMembership),
            [typeof(IAccountStoreMapping)] = typeof(DefaultAccountStoreMapping),
            [typeof(IAccountStore)] = typeof(DefaultAccountStore),
            [typeof(IBasicLoginAttempt)] = typeof(DefaultBasicLoginAttempt),
            [typeof(SDK.Auth.IAuthenticationResult)] = typeof(DefaultAuthenticationResult),
            [typeof(IPasswordResetToken)] = typeof(DefaultPasswordResetToken),
            [typeof(ICustomData)] = typeof(DefaultCustomData),
            [typeof(IEmailVerificationToken)] = typeof(DefaultEmailVerificationToken),
            [typeof(IEmailVerificationRequest)] = typeof(DefaultEmailVerificationRequest),
            [typeof(IProviderAccountAccess)] = typeof(DefaultProviderAccountAccess),
            [typeof(IProviderAccountResult)] = typeof(DefaultProviderAccountResult),
            [typeof(IProvider)] = typeof(DefaultProvider),
            [typeof(IProviderData)] = typeof(DefaultProviderData),
            [typeof(IFacebookProvider)] = typeof(DefaultFacebookProvider),
            [typeof(IFacebookProviderData)] = typeof(DefaultFacebookProviderData),
            [typeof(IGithubProvider)] = typeof(DefaultGithubProvider),
            [typeof(IGithubProviderData)] = typeof(DefaultGithubProviderData),
            [typeof(IGoogleProvider)] = typeof(DefaultGoogleProvider),
            [typeof(IGoogleProviderData)] = typeof(DefaultGoogleProviderData),
            [typeof(ILinkedInProvider)] = typeof(DefaultLinkedInProvider),
            [typeof(ILinkedInProviderData)] = typeof(DefaultLinkedInProviderData),
            [typeof(IAccountResult)] = typeof(DefaultAccountResult),
            [typeof(INonce)] = typeof(DefaultNonce),
        };

        private static readonly IReadOnlyDictionary<Type, Type> InterfaceLookup = new Dictionary<Type, Type>()
        {
            [typeof(DefaultAccount)] = typeof(IAccount),
            [typeof(DefaultApplication)] = typeof(IApplication),
            [typeof(DefaultTenant)] = typeof(ITenant),
            [typeof(DefaultDirectory)] = typeof(IDirectory),
            [typeof(DefaultGroup)] = typeof(IGroup),
            [typeof(DefaultGroupMembership)] = typeof(IGroupMembership),
            [typeof(DefaultAccountStoreMapping)] = typeof(IAccountStoreMapping),
            [typeof(DefaultAccountStore)] = typeof(IAccountStore),
            [typeof(DefaultBasicLoginAttempt)] = typeof(IBasicLoginAttempt),
            [typeof(DefaultAuthenticationResult)] = typeof(SDK.Auth.IAuthenticationResult),
            [typeof(DefaultPasswordResetToken)] = typeof(IPasswordResetToken),
            [typeof(DefaultCustomData)] = typeof(ICustomData),
            [typeof(DefaultEmailVerificationToken)] = typeof(IEmailVerificationToken),
            [typeof(DefaultEmailVerificationRequest)] = typeof(IEmailVerificationRequest),
            [typeof(DefaultProviderAccountAccess)] = typeof(IProviderAccountAccess),
            [typeof(DefaultProviderAccountResult)] = typeof(IProviderAccountResult),
            [typeof(DefaultProvider)] = typeof(IProvider),
            [typeof(DefaultProviderData)] = typeof(IProviderData),
            [typeof(DefaultFacebookProvider)] = typeof(IFacebookProvider),
            [typeof(DefaultFacebookProviderData)] = typeof(IFacebookProviderData),
            [typeof(DefaultGithubProvider)] = typeof(IGithubProvider),
            [typeof(DefaultGithubProviderData)] = typeof(IGithubProviderData),
            [typeof(DefaultGoogleProvider)] = typeof(IGoogleProvider),
            [typeof(DefaultGoogleProviderData)] = typeof(IGoogleProviderData),
            [typeof(DefaultLinkedInProvider)] = typeof(ILinkedInProvider),
            [typeof(DefaultLinkedInProviderData)] = typeof(ILinkedInProviderData),
            [typeof(DefaultAccountResult)] = typeof(IAccountResult)
        };

        private static readonly IReadOnlyDictionary<string, Type> InterfaceLookupByAttributeName = new Dictionary<string, Type>()
        {
            ["application"] = typeof(IApplication),
            ["account"] = typeof(IAccount),
            ["directory"] = typeof(IDirectory),
            ["group"] = typeof(IGroup),
            ["customData"] = typeof(ICustomData),
            ["providerData"] = typeof(IProviderData),
            ["provider"] = typeof(IProvider),
            ["tenant"] = typeof(ITenant),
            ["defaultAccountStoreMapping"] = typeof(IAccountStoreMapping),
            ["defaultGroupStoreMapping"] = typeof(IAccountStoreMapping),
            ["accountStore"] = typeof(IAccountStore),

            ["applications"] = typeof(CollectionResponsePage<IApplication>),
            ["directories"] = typeof(CollectionResponsePage<IDirectory>),
            ["accounts"] = typeof(CollectionResponsePage<IAccount>),
            ["accountStoreMappings"] = typeof(CollectionResponsePage<IAccountStoreMapping>),
            ["groups"] = typeof(CollectionResponsePage<IGroup>),
            ["groupMemberships"] = typeof(CollectionResponsePage<IGroupMembership>),
            ["accountMemberships"] = typeof(CollectionResponsePage<IGroupMembership>),
        };

        private static readonly IReadOnlyDictionary<Type, Type> CollectionInterfaceLookup = new Dictionary<Type, Type>()
        {
            [typeof(CollectionResponsePage<IAccount>)] = typeof(IAccount),
            [typeof(CollectionResponsePage<IApplication>)] = typeof(IApplication),
            [typeof(CollectionResponsePage<IDirectory>)] = typeof(IDirectory),
            [typeof(CollectionResponsePage<IGroup>)] = typeof(IGroup),
            [typeof(CollectionResponsePage<IGroupMembership>)] = typeof(IGroupMembership),
            [typeof(CollectionResponsePage<IAccountStoreMapping>)] = typeof(IAccountStoreMapping),
        };

        private static Type GetConcreteTypeForInterface(Type iface)
        {
            Type concrete = null;
            ConcreteLookup.TryGetValue(iface, out concrete);

            return concrete;
        }

        private static Type GetInterfaceForConcreteType(Type concrete)
        {
            Type iface = null;
            InterfaceLookup.TryGetValue(concrete, out iface);

            return iface;
        }

        /// <summary>
        /// Checks whether this type is a known resource interface (e.g. <see cref="IAccount"/>).
        /// </summary>
        /// <param name="possiblyInterface">The type to check</param>
        /// <returns>True if this type is a known resource interface</returns>
        private static bool IsInterface(Type possiblyInterface)
            => ConcreteLookup.ContainsKey(possiblyInterface);

        /// <summary>
        /// Checks whether this type is a known concrete instance class (e.g., <see cref="DefaultAccount"/>).
        /// </summary>
        /// <param name="possiblyConcrete">The type to check</param>
        /// <returns>True if this type is a known concrete type</returns>
        private static bool IsConcrete(Type possiblyConcrete)
            => InterfaceLookup.ContainsKey(possiblyConcrete);

        /// <summary>
        /// Checks whether this type represents a paged collection response.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns><c>true</c> if this type represents a paged collection response; <c>false</c> otherwise.</returns>
        public static bool IsCollectionResponse(Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CollectionResponsePage<>);

        /// <summary>
        /// Looks up an interface from a concrete type.
        /// </summary>
        /// <typeparam name="T">A concrete instance class type (e.g., <see cref="DefaultAccount"/>).</typeparam>
        /// <returns>The associated interface type (e.g., <see cref="IAccount"/>).</returns>
        public Type GetInterface<T>()
            where T : IResource
        {
            return this.GetInterface(typeof(T));
        }

        /// <summary>
        /// Looks up an interface from a concrete type.
        /// </summary>
        /// <param name="possiblyConcrete">A concrete instance class type (e.g., <see cref="DefaultAccount"/>).</param>
        /// <returns>The associated interface type (e.g., <see cref="IAccount"/>).</returns>
        public Type GetInterface(Type possiblyConcrete)
        {
            if (IsInterface(possiblyConcrete))
                return possiblyConcrete;

            return GetInterfaceForConcreteType(possiblyConcrete);
        }

        /// <summary>
        /// Gets a resource interface type given a resource attribute name.
        /// </summary>
        /// <param name="nestedItemKey">A resource attribute name (e.g. "directory").</param>
        /// <returns>The associated interface type (e.g., <see cref="IDirectory"/>, or <c>null</c> if no type could be found.</returns>
        public Type GetInterface(string nestedItemKey)
        {
            Type foundType = null;
            InterfaceLookupByAttributeName.TryGetValue(nestedItemKey, out foundType);

            return foundType;
        }

        /// <summary>
        /// Looks up concrete type from an interface.
        /// </summary>
        /// <typeparam name="T">A resource interface (e.g., <see cref="IAccount"/>).</typeparam>.
        /// <returns>The associated concrete instance class type (e.g., <see cref="DefaultAccount"/>).</returns>
        public Type GetConcrete<T>()
            where T : IResource
        {
            return this.GetConcrete(typeof(T));
        }

        /// <summary>
        /// Looks up concrete type from an interface.
        /// </summary>
        /// <param name="possiblyInterface">A resource interface (e.g., <see cref="IAccount"/>).</param>
        /// <returns>The associated concrete instance class type (e.g., <see cref="DefaultAccount"/>).</returns>
        public Type GetConcrete(Type possiblyInterface)
        {
            if (IsConcrete(possiblyInterface))
                return possiblyInterface;

            return GetConcreteTypeForInterface(possiblyInterface);
        }

        /// <summary>
        /// Looks up the inner interface from a collection type.
        /// </summary>
        /// <param name="collectionType">A <see cref="CollectionResponsePage{T}"/> containing some inner <see cref="IResource"/> interface (e.g. <see cref="CollectionResponsePage{IAccount}"/>).</param>
        /// <returns>The inner interface (e.g. <see cref="IAccount"/>).</returns>
        public Type GetInnerCollectionInterface(Type collectionType)
        {
            Type iface = null;
            CollectionInterfaceLookup.TryGetValue(collectionType, out iface);

            return iface;
        }
    }
}
