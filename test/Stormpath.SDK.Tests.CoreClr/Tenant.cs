// <copyright file="Tenant.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;
using Xunit;

namespace Stormpath.SDK.Tests.Dnx
{
    public class Tenant
    {
        [Fact]
        public async Task Getting_current_tenant()
        {
            var client = Clients.Builder()
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .Build();

            var tenant = await client.GetCurrentTenantAsync();

            Assert.NotNull(tenant);
        }
    }
}
