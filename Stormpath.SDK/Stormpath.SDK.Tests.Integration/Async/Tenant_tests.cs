// <copyright file="Tenant_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [CollectionDefinition("Live tenant tests")]
    public class Tenant_tests
    {
        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Getting_current_tenant(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var tenant = await client.GetCurrentTenantAsync();

            tenant.ShouldNotBe(null);
            tenant.Href.ShouldNotBe(null);
            tenant.Name.ShouldNotBe(null);
        }
    }
}