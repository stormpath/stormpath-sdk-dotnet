// <copyright file="ResourceTypeLookup_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.AccountStore;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Tenant;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class ResourceTypeLookup_tests
    {
        private readonly ResourceTypeLookup lookup;

        public ResourceTypeLookup_tests()
        {
            this.lookup = new ResourceTypeLookup();
        }

        [Fact]
        public void Returns_null_for_unknown_concrete_type()
        {
            this.lookup.GetConcrete(typeof(ResourceTypeLookup)).ShouldBeNull();
        }

        [Fact]
        public void Returns_null_for_unknown_interface_type()
        {
            this.lookup.GetInterface(typeof(IAppDomainSetup)).ShouldBeNull();
        }

        [Fact]
        public void Returns_null_for_unknown_collection_type()
        {
            this.lookup.GetInnerCollectionInterface(typeof(ResourceTypeLookup)).ShouldBeNull();
            this.lookup.GetInnerCollectionInterface(typeof(CollectionResponsePage<>)).ShouldBeNull();
            this.lookup.GetInnerCollectionInterface(typeof(CollectionResponsePage<IBasicLoginAttempt>)).ShouldBeNull();
        }

        [Fact]
        public void Getting_concrete_type_from_type_parameter()
        {
            this.lookup.GetConcrete<IAccount>().ShouldBe(typeof(DefaultAccount));
        }

        [Fact]
        public void Getting_interface_type_from_type_parameter()
        {
            this.lookup.GetInterface<DefaultAccount>().ShouldBe(typeof(IAccount));
        }

        [Theory]
        [MemberData(nameof(InterfaceToConcreteMapping))]
        public void Getting_concrete_type_from_interface(Type @interface, Type expectedConcrete)
        {
            this.lookup.GetConcrete(@interface).ShouldBe(expectedConcrete);
        }

        [Theory]
        [MemberData(nameof(InterfaceToConcreteMapping))]
        public void Getting_interface_from_concrete_type(Type expectedInterface, Type concrete)
        {
            this.lookup.GetInterface(concrete).ShouldBe(expectedInterface);
        }

        [Theory]
        [MemberData(nameof(CollectionInterfaceTypes))]
        public void Getting_inner_interface_from_collection(Type collectionType, Type expectedInterface)
        {
            this.lookup.GetInnerCollectionInterface(collectionType).ShouldBe(expectedInterface);
        }

        public static IEnumerable<object[]> InterfaceToConcreteMapping()
        {
            yield return new object[] { typeof(IAccount), typeof(DefaultAccount) };
            yield return new object[] { typeof(IApplication), typeof(DefaultApplication) };
            yield return new object[] { typeof(ITenant), typeof(DefaultTenant) };
            yield return new object[] { typeof(IDirectory), typeof(DefaultDirectory) };
            yield return new object[] { typeof(IAccountStoreMapping), typeof(DefaultAccountStoreMapping) };
            yield return new object[] { typeof(IAccountStore), typeof(DefaultAccountStore) };
            yield return new object[] { typeof(IBasicLoginAttempt), typeof(DefaultBasicLoginAttempt) };
            yield return new object[] { typeof(IAuthenticationResult), typeof(DefaultAuthenticationResult) };
            yield return new object[] { typeof(IPasswordResetToken), typeof(DefaultPasswordResetToken) };
        }

        public static IEnumerable<object[]> CollectionInterfaceTypes()
        {
            yield return new object[] { typeof(CollectionResponsePage<IAccount>), typeof(IAccount) };
            yield return new object[] { typeof(CollectionResponsePage<IApplication>), typeof(IApplication) };
            yield return new object[] { typeof(CollectionResponsePage<IDirectory>), typeof(IDirectory) };
        }
    }
}
