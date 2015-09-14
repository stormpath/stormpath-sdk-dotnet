// <copyright file="DefaultHttpClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Extensions.Http;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultHttpClientBuilder_tests
    {
        private ITypeLoader<IHttpClient> GetFailingLoader()
        {
            IHttpClient dummy = null;
            var failingLoader = Substitute.For<ITypeLoader<IHttpClient>>();
            failingLoader.TryLoad(out dummy).Returns(false);

            return failingLoader;
        }

        [Fact]
        public void Constructs_instance_from_specified_type()
        {
            IHttpClientBuilder builder = new DefaultHttpClientBuilder(this.GetFailingLoader());

            builder.UseHttpClient(new RestSharpClient("http://api.foo.bar", 0, Substitute.For<ILogger>()));

            var instance = builder.Build();
            instance.ShouldBeAssignableTo<IHttpClient>();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void Constructs_instance_from_default_library()
        {
            IHttpClientBuilder builder = new DefaultHttpClientBuilder();

            builder.SetConnectionTimeout(0);
            builder.SetLogger(Substitute.For<ILogger>());

            var instance = builder.Build();
            instance.ShouldBeAssignableTo<IHttpClient>();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void Throws_when_no_supported_type_can_be_found()
        {
            IHttpClientBuilder builder = new DefaultHttpClientBuilder(this.GetFailingLoader());

            Assert.Throws<ApplicationException>(() =>
            {
                var instance = builder.Build();
            });
        }
    }
}
