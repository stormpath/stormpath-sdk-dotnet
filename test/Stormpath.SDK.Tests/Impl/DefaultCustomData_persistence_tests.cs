// <copyright file="DefaultCustomData_persistence_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Http;
using Stormpath.SDK.Tests.Common.Fakes;
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
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.CustomData).Object);

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);
            customData.Count().ShouldBe(5);

            customData.Put("newprop", "foo");
            await customData.SaveAsync();

            var expectedBody = @"{""newprop"":""foo""}";

            // Sent the update as a diff
            await dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == expectedBody),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deletes_are_performed_with_updates()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.CustomData).Object);

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);

            customData.Put("testingIsUseful", true);
            customData.Remove("membershipType");
            await customData.SaveAsync();

            // Delete
            await dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Delete &&
                    request.CanonicalUri.ToString() == "https://api.stormpath.com/v1/accounts/foobarAccount/customData/membershipType"),
                Arg.Any<CancellationToken>());

            // Post
            await dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == @"{""testingIsUseful"":true}"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Simultaneous_delete_and_update_only_performs_update()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.CustomData).Object);

            var customData = await dataStore.GetResourceAsync<ICustomData>("/customData", CancellationToken.None);

            customData.Remove("membershipType");
            customData.Put("membershipType", "unknown");
            await customData.SaveAsync();

            // Delete
            await dataStore.RequestExecutor.DidNotReceive().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Delete &&
                    request.CanonicalUri.ToString() == "https://api.stormpath.com/v1/accounts/foobarAccount/customData/membershipType"),
                Arg.Any<CancellationToken>());

            // Update
            await dataStore.RequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Method == HttpMethod.Post &&
                    request.Body == @"{""membershipType"":""unknown""}"),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Embedded_custom_data_is_cleared_after_reload()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.Account).Object);

            var account = await dataStore.GetResourceAsync<IAccount>("/account", CancellationToken.None);
            account.CustomData.Put("foo", 123);

            await dataStore.GetResourceAsync<IAccount>("/account", CancellationToken.None); // reload
            await account.SaveAsync();

            // Empty save, should not POST anything
            await dataStore.RequestExecutor.DidNotReceive().ExecuteAsync(
                Arg.Is<IHttpRequest>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Embedded_custom_data_is_cleared_after_save()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.Account).Object);

            var account = await dataStore.GetResourceAsync<IAccount>("/account", CancellationToken.None);
            account.CustomData.Put("foo", 123);

            await account.SaveAsync();

            var expectedBody = @"{""customData"":{""foo"":123}}";
            var body = dataStore.RequestExecutor
                .ReceivedCalls()
                .Last()
                .GetArguments()
                .OfType<IHttpRequest>()
                .First()
                .Body;
            body.ShouldBe(expectedBody);

            dataStore.RequestExecutor.ClearReceivedCalls();
            await account.SaveAsync();

            // Empty save, should not POST anything
            await dataStore.RequestExecutor.DidNotReceive().ExecuteAsync(
                Arg.Is<IHttpRequest>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Embedded_custom_data_is_cleared_for_all_instances_after_save()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.Account).Object);

            var account = await dataStore.GetResourceAsync<IAccount>("/account", CancellationToken.None);
            var account2 = await dataStore.GetResourceAsync<IAccount>("/account", CancellationToken.None);
            account.CustomData.Put("foo", 123);

            await account.SaveAsync();

            var expectedBody = @"{""customData"":{""foo"":123}}";
            var body = dataStore.RequestExecutor
                .ReceivedCalls()
                .Last()
                .GetArguments()
                .OfType<IHttpRequest>()
                .First()
                .Body;
            body.ShouldBe(expectedBody);

            dataStore.RequestExecutor.ClearReceivedCalls();
            await account2.SaveAsync();

            // Empty save, should not POST anything
            await dataStore.RequestExecutor.DidNotReceive().ExecuteAsync(
                Arg.Is<IHttpRequest>(req =>
                    req.Method == HttpMethod.Post),
                Arg.Any<CancellationToken>());
        }
    }
}
