// <copyright file="DefaultJsonSerializerBuilder_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultJsonSerializerBuilder_tests
    {
        private ITypeLoader<IJsonSerializer> GetFailingLoader()
        {
            IJsonSerializer dummy = null;
            var failingLoader = Substitute.For<ITypeLoader<IJsonSerializer>>();
            failingLoader.TryLoad(out dummy).Returns(false);

            return failingLoader;
        }

        [Fact]
        public void Returns_specified_instance()
        {
            var instance = Substitute.For<IJsonSerializer>();

            IJsonSerializerBuilder builder = new DefaultJsonSerializerBuilder(this.GetFailingLoader());
            builder.UseSerializer(instance);

            builder.Build().ShouldBe(instance);
        }

        [Fact]
        public void Tries_to_use_default_library_loader_if_no_instance_is_specified()
        {
            IJsonSerializer fakeSerializer = Substitute.For<IJsonSerializer>();
            IJsonSerializer dummy = null;

            var fakeLoader = Substitute.For<ITypeLoader<IJsonSerializer>>();
            fakeLoader.TryLoad(out dummy).Returns(call =>
            {
                call[0] = fakeSerializer;
                return true;
            });

            IJsonSerializerBuilder builder = new DefaultJsonSerializerBuilder(fakeLoader);

            builder.Build().ShouldBe(fakeSerializer);
        }

        [Fact]
        public void Throws_when_no_supported_type_can_be_found()
        {
            IJsonSerializerBuilder builder = new DefaultJsonSerializerBuilder(this.GetFailingLoader());

            Assert.Throws<ApplicationException>(() =>
            {
                var instance = builder.Build();
            });
        }
    }
}
