// <copyright file="TestClientBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Http;
using Stormpath.SDK.Extensions.Serialization;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests.Integration.Helpers
{
    [Serializable]
    public class TestClientBuilder : IXunitSerializable
    {
        private string clientName;

        // required for deserializer
        public TestClientBuilder()
        {
        }

        public TestClientBuilder(string authScheme)
        {
            this.clientName = authScheme;
        }

        public IClient Build()
        {
            switch (this.clientName)
            {
                case nameof(IntegrationTestClients.Basic):
                    return IntegrationTestClients.Basic.Value;

                case nameof(IntegrationTestClients.SAuthc1):
                    return IntegrationTestClients.SAuthc1.Value;
            }

            throw new NotImplementedException($"Client '{this.clientName}' is not supported.");
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            this.clientName = info.GetValue<string>("clientName");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("clientName", this.clientName, typeof(string));
        }

        public override string ToString()
        {
            return $"Client: {this.clientName}";
        }
    }
}
