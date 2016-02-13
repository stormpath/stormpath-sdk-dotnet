// <copyright file="AbstractJsonSerializerBuilder_tests.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Impl.Serialization;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class AbstractJsonSerializerBuilder_tests
    {
        [Fact]
        public void Constructs_instance_from_specified_type()
        {
            ISerializerBuilder builder = new AbstractSerializerBuilder<DummySerializer>();

            var instance = builder.Build();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void Constructs_instance_from_default_library()
        {
            ISerializerBuilder builder = new AbstractSerializerBuilder<IJsonSerializer>(DefaultSerializerLoader.Load());

            var instance = builder.Build();
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void Throws_when_type_is_null()
        {
            Should.Throw<NotSupportedException>(() => new AbstractSerializerBuilder<IJsonSerializer>(null));
        }

        public class DummySerializer : IJsonSerializer
        {
            public IDictionary<string, object> Deserialize(string json)
            {
                throw new NotImplementedException();
            }

            public string Serialize(IDictionary<string, object> map)
            {
                throw new NotImplementedException();
            }
        }
    }
}
