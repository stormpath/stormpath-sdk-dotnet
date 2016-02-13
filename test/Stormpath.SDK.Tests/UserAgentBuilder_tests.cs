// <copyright file="UserAgentBuilder_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Client;
using Stormpath.SDK.Impl.Introspection;
using Xunit;
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests
{
    public class UserAgentBuilder_tests
    {
        private readonly ITestOutputHelper output;

        public UserAgentBuilder_tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void UserAgent_is_known()
        {
            var userAgent = new UserAgentBuilder(
                Platform.Analyze(),
                Sdk.Anaylze(),
                SourceLanguage.Analyze(this.GetType().Assembly)).GetUserAgent();

            userAgent.ShouldNotContain("unknown");

            this.output.WriteLine($"UserAgent: {userAgent}");
        }
    }
}
