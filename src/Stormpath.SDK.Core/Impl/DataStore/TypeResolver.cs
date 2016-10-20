// <copyright file="TypeResolver.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Organization;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class TypeResolver
    {
        public Type Resolve(Type type, Map properties)
        {
            Type resolvedType = null;

            resolvedType = ResolveAccountStoreMappingTypes(type);
            if (resolvedType != null)
            {
                return resolvedType;
            }

            resolvedType = ResolveFactorPolymorphism(type, properties);
            if (resolvedType != null)
            {
                return resolvedType;
            }

            // The original type is fine!
            return type;
        }

        private Type ResolveAccountStoreMappingTypes(Type type)
        {
            if (type == typeof(IAccountStoreMapping<IApplicationAccountStoreMapping>) || type == typeof(IAccountStoreMapping))
            {
                return typeof(IApplicationAccountStoreMapping);
            }
            else if (type == typeof(IAccountStoreMapping<IOrganizationAccountStoreMapping>))
            {
                return typeof(IOrganizationAccountStoreMapping);
            }

            return null;
        }

        // TODO refactor this so it's not hardcoded into TypeResolver
        private Type ResolveFactorPolymorphism(Type type, Map properties)
        {
            if (type != typeof(IFactor))
            {
                return null;
            }

            var hasItems = properties?.Any() ?? false;
            if (!hasItems)
            {
                return null;
            }

            object rawFactorType;
            if (!properties.TryGetValue("type", out rawFactorType))
            {
                return null;
            }

            var factorType = rawFactorType.ToString();
            if (factorType.Equals("sms", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(ISmsFactor);
            }
            else if (factorType.Equals("google-authenticator", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(IGoogleAuthenticatorFactor);
            }

            return null;
        }
    }
}
