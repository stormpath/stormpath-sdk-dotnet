// <copyright file="DefaultResourceFactory_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.IdentityMap;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultResourceFactory_tests : IDisposable
    {
        private readonly IResourceFactory factory;

        public DefaultResourceFactory_tests()
        {
            var dataStore = new StubDataStore(null, "http://api.foo.bar");
            var identityMap = new MemoryCacheIdentityMap<ResourceData>(TimeSpan.FromSeconds(10), Substitute.For<ILogger>()); // arbitrary expiration time
            this.factory = new DefaultResourceFactory(dataStore, identityMap);
        }

        [Fact]
        public void Should_throw_for_unsupported_type()
        {
            Should.Throw<ApplicationException>(() =>
            {
                var bad = this.factory.Create<IResource>();
            });
        }

        [Fact]
        public void Create_with_type_parameter_returns_concrete_type()
        {
            this.factory.Create<IAccount>().ShouldBeOfType<DefaultAccount>();
        }

        [Fact]
        public void Create_with_properties_passes_properties_into_object()
        {
            var properties = new Dictionary<string, object>()
            {
                { "href", "https://secure.api.foo.bar/baz" }
            };
            var instance = this.factory.Create<IAccount>(properties);

            instance.Href.ShouldBe("https://secure.api.foo.bar/baz");
        }

        [Theory]
        [MemberData(nameof(ResourceTypes))]
        public void Create_returns_concrete_type(Type @interface, Type expected)
        {
            var createMethod = typeof(IResourceFactory)
                .GetMethod("Create", new Type[] { typeof(ILinkable) })
                .MakeGenericMethod(new Type[] { @interface });

            var result = createMethod.Invoke(this.factory, new object[] { null });

            result.ShouldBeOfType(expected);
        }

        public static IEnumerable<object[]> ResourceTypes()
        {
            yield return new object[] { typeof(IAccount), typeof(DefaultAccount) };
            yield return new object[] { typeof(IApplication), typeof(DefaultApplication) };
            yield return new object[] { typeof(ITenant), typeof(DefaultTenant) };
            yield return new object[] { typeof(IDirectory), typeof(DefaultDirectory) };
        }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}
