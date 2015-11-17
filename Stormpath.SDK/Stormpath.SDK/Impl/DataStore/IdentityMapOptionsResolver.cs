// <copyright file="IdentityMapOptionsResolver.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class IdentityMapOptionsResolver
    {
        private static readonly List<Type> DoNotStore
            = new List<Type>()
            {
                typeof(SDK.Provider.IProviderAccountResult),
                typeof(SDK.Account.IEmailVerificationToken),
                typeof(SDK.Auth.IAuthenticationResult),
            };

        private static readonly List<Type> DoNotExpire
            = new List<Type>()
            {
                typeof(SDK.Tenant.ITenant),
            };

        public IdentityMapOptions GetOptions(Type interfaceType)
        {
            bool skip = DoNotStore.Contains(interfaceType);
            bool storeInfinitely = DoNotExpire.Contains(interfaceType);

            var options = new IdentityMapOptions(skip, storeInfinitely);
            if (!options.IsValid())
                throw new ApplicationException($"Bad identity map options specified for type {interfaceType.Name}");

            return options;
        }
    }
}
