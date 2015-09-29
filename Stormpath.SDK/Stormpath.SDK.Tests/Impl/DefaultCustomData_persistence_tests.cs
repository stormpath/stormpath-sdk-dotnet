// <copyright file="DefaultCustomData_persistence_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultCustomData_persistence_tests
    {
        [Fact]
        public async Task Only_diff_is_sent_for_updates()
        {
            IInternalDataStore dataStore = new StubDataStore(FakeJson.CustomData, "http://api.foo.bar");

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);
            customData.Count().ShouldBe(5);

            customData.Put("newprop", "foo");
            await customData.SaveAsync();

            // Sent the update as a diff
            dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == @"{""newprop"":""foo""}"),
                Arg.Any<CancellationToken>()).IgnoreAwait();
        }

        [Fact]
        public async Task Deletes_are_performed_with_updates()
        {
            IInternalDataStore dataStore = new StubDataStore(FakeJson.CustomData, "http://api.foo.bar");

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);

            customData.Put("testingIsUseful", true);
            customData.Remove("membershipType");
            await customData.SaveAsync();

            // Delete
            dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Delete &&
                    request.CanonicalUri.ToString() == "https://api.stormpath.com/v1/accounts/foobarAccount/customData/membershipType"),
                Arg.Any<CancellationToken>()).IgnoreAwait();

            // Post
            dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == @"{""testingIsUseful"":true}"),
                Arg.Any<CancellationToken>()).IgnoreAwait();
        }

        [Fact]
        public async Task Simultaneous_delete_and_update_only_performs_update()
        {
            IInternalDataStore dataStore = new StubDataStore(FakeJson.CustomData, "http://api.foo.bar");

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);

            customData.Remove("membershipType");
            customData.Put("membershipType", "unknown");
            await customData.SaveAsync();

            // Delete
            dataStore.RequestExecutor.DidNotReceive().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Delete &&
                    request.CanonicalUri.ToString() == "https://api.stormpath.com/v1/accounts/foobarAccount/customData/membershipType"),
                Arg.Any<CancellationToken>()).IgnoreAwait();

            // Update
            dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == @"{""membershipType"":""unknown""}"),
                Arg.Any<CancellationToken>()).IgnoreAwait();
        }
    }
}
