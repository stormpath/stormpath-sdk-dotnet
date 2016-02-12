// <copyright file="InlineIdSiteAsyncResultListener_tests.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.IdSite;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.IdSite
{
    public class InlineIdSiteAsyncResultListener_tests
    {
        [Fact]
        public async Task When_setting_all_actions()
        {
            bool onRegistered = false,
                onAuthenticated = false,
                onLogout = false;

            IIdSiteAsyncResultListener listener = new InlineIdSiteAsyncResultListener(
                onAuthenticated: (result, ct) =>
                {
                    onAuthenticated = true;
                    return Task.FromResult(true);
                },
                onLogout: (result, ct) =>
                {
                    onLogout = true;
                    return Task.FromResult(true);
                },
                onRegistered: (result, ct) =>
                {
                    onRegistered = true;
                    return Task.FromResult(true);
                });

            var dummyAccountResult = Substitute.For<IAccountResult>();

            await listener.OnAuthenticatedAsync(dummyAccountResult, CancellationToken.None);
            await listener.OnLogoutAsync(dummyAccountResult, CancellationToken.None);
            await listener.OnRegisteredAsync(dummyAccountResult, CancellationToken.None);

            onRegistered.ShouldBeTrue();
            onAuthenticated.ShouldBeTrue();
            onLogout.ShouldBeTrue();
        }

        [Fact]
        public async Task When_setting_one_action()
        {
            bool onRegistered = false,
                onAuthenticated = false,
                onLogout = false;

            IIdSiteAsyncResultListener listener = new InlineIdSiteAsyncResultListener(
                onAuthenticated: null,
                onLogout: (result, ct) =>
                {
                    onLogout = true;
                    return Task.FromResult(true);
                },
                onRegistered: null);

            var dummyAccountResult = Substitute.For<IAccountResult>();

            await listener.OnAuthenticatedAsync(dummyAccountResult, CancellationToken.None);
            await listener.OnLogoutAsync(dummyAccountResult, CancellationToken.None);
            await listener.OnRegisteredAsync(dummyAccountResult, CancellationToken.None);

            onRegistered.ShouldBeFalse();
            onAuthenticated.ShouldBeFalse();
            onLogout.ShouldBeTrue();
        }
    }
}
