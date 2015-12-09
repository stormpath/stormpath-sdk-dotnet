// <copyright file="TypeNameResolver.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Organization;

namespace Stormpath.SDK.Impl.DataStore
{
    [Obsolete("Remove after IAccountStoreMapping refactoring.")]
    internal sealed class TypeNameResolver
    {
        public string GetTypeName(Type type)
        {
            if (type == typeof(IAccountStoreMapping<IApplicationAccountStoreMapping>))
            {
                return nameof(IApplicationAccountStoreMapping);
            }
            else if (type == typeof(IAccountStoreMapping<IOrganizationAccountStoreMapping>))
            {
                return nameof(IOrganizationAccountStoreMapping);
            }

            return type.Name;
        }
    }
}
