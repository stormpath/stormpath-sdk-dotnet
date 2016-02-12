// <copyright file="DefaultRequestAuthenticatorFactory_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.Http.Authentication;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Authentication
{
    public class DefaultRequestAuthenticatorFactory_tests
    {
        private readonly IRequestAuthenticatorFactory factory;

        public DefaultRequestAuthenticatorFactory_tests()
        {
            this.factory = new DefaultRequestAuthenticatorFactory();
        }

        [Fact]
        public void Returns_SAuthc1_authenticator_for_null_scheme()
        {
            var authenticator = this.factory.Create(null);

            authenticator.ShouldBeOfType<SAuthc1RequestAuthenticator>();
        }

        [Fact]
        public void Returns_SAuthc1_authenticator()
        {
            var authenticator = this.factory.Create(AuthenticationScheme.SAuthc1);

            authenticator.ShouldBeOfType<SAuthc1RequestAuthenticator>();
        }

        [Fact]
        public void Returns_Basic_authenticator()
        {
            var authenticator = this.factory.Create(AuthenticationScheme.Basic);

            authenticator.ShouldBeOfType<BasicRequestAuthenticator>();
        }
    }
}
