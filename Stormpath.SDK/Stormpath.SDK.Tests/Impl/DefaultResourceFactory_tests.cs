// <copyright file="DefaultResourceFactory_tests.cs" company="Stormpath, Inc.">
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
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Application;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Directory;
using Stormpath.SDK.Impl.Group;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultResourceFactory_tests
    {
        private readonly IInternalDataStore fakeDataStore;
        private readonly IResourceFactory factory;

        public DefaultResourceFactory_tests()
        {
            this.fakeDataStore = Substitute.For<IInternalDataStore>();

            this.factory = new DefaultResourceFactory(this.fakeDataStore);
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
        public void Creating_IAccount_returns_DefaultAccount()
        {
            IAccount account = this.factory.Create<IAccount>();
            account.ShouldBeOfType<DefaultAccount>();
        }

        [Fact]
        public void Creating_IApplication_returns_DefaultApplication()
        {
            IApplication account = this.factory.Create<IApplication>();
            account.ShouldBeOfType<DefaultApplication>();
        }

        [Fact]
        public void Creating_ITenant_returns_DefaultTenant()
        {
            ITenant account = this.factory.Create<ITenant>();
            account.ShouldBeOfType<DefaultTenant>();
        }

        [Fact]
        public void Creating_IDirectory_returns_DefaultDirectory()
        {
            IDirectory account = this.factory.Create<IDirectory>();
            account.ShouldBeOfType<DefaultDirectory>();
        }

        [Fact]
        public void Creating_IGroup_returns_DefaultGroup()
        {
            IGroup account = this.factory.Create<IGroup>();
            account.ShouldBeOfType<DefaultGroup>();
        }
    }
}
