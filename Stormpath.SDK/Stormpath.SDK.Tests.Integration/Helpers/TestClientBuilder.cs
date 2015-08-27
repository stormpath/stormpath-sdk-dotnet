// <copyright file="TestClientBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Client;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests.Integration.Helpers
{
    [Serializable]
    public class TestClientBuilder : IXunitSerializable
    {
        private string authSchemeName;

        // required for deserializer
        public TestClientBuilder()
        {
        }

        public TestClientBuilder(string authScheme)
        {
            this.authSchemeName = authScheme;
        }

        public IClient Build()
        {
            var authentication = AuthenticationScheme.Parse(authSchemeName);
            var apiKey = IntegrationTestClients.GetApiKey();
            return Clients.Builder()
                .SetApiKey(apiKey)
                .SetAuthenticationScheme(authentication)
                .Build();
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            authSchemeName = info.GetValue<string>("authSchemeName");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("authSchemeName", authSchemeName, typeof(string));
        }

        public override string ToString()
        {
            return $"Auth = {authSchemeName}";
        }
    }
}
