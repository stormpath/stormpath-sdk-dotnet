// <copyright file="Sync_expansion_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using Stormpath.SDK.Sync;
using Xunit;

//namespace Stormpath.SDK.Tests.Integration.Sync
//{
//    [Collection("Live tenant tests")]
//    public class Sync_expansion_tests
//    {
//        private readonly IntegrationTestFixture fixture;

//        public Sync_expansion_tests(IntegrationTestFixture fixture)
//        {
//            this.fixture = fixture;
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_custom_data(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetCustomData)
//                .FirstOrDefault();
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_directory(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetDirectory)
//                .FirstOrDefault();
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_group_memberships(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetGroupMemberships)
//                .FirstOrDefault();
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_groups(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetGroups)
//                .FirstOrDefault();
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_provider_data(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetProviderData)
//                .FirstOrDefault();
//        }

//        [Theory]
//        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
//        public void Expanding_tenant(TestClientBuilder clientBuilder)
//        {
//            var client = clientBuilder.Build();
//            var tenant = client.GetCurrentTenant();

//            var account = tenant
//                .GetAccounts()
//                .Synchronously()
//                .Where(x => x.Email.StartsWith("lskywalker"))
//                .Expand(x => x.GetTenant)
//                .FirstOrDefault();
//        }
//    }
//}
