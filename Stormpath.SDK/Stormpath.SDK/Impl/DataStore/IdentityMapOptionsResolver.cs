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
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Impl.DataStore
{
    internal sealed class IdentityMapOptionsResolver
    {
        private static readonly IdentityMapOptions DefaultOptions
            = new IdentityMapOptions()
            {
                SkipIdentityMap = false,
                StoreWithInfiniteExpiration = false
            };

        private static readonly List<Type> DoNotStore
            = new List<Type>()
            {
                typeof(IProviderAccountResult),
            };

        private static readonly List<Type> DoNotExpire
            = new List<Type>()
            {
                typeof(ITenant),
            };

        public IdentityMapOptions GetOptions(Type interfaceType)
        {
            var options = DefaultOptions;

            if (DoNotStore.Contains(interfaceType))
                options.SkipIdentityMap = true;

            if (DoNotExpire.Contains(interfaceType))
                options.StoreWithInfiniteExpiration = true;

            if (!options.IsValid())
                throw new ApplicationException("Bad identity map options specified.");

            return options;
        }
    }
}
