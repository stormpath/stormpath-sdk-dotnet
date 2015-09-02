// <copyright file="PasswordResetToken_tests.cs" company="Stormpath, Inc.">
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

using System.Collections;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class PasswordResetToken_tests
    {
        [Fact]
        public void GetValue_returns_token_value()
        {
            var ds = new DefaultDataStore(Substitute.For<IRequestExecutor>(), "http://api.foo.bar");
            var href = "https://api.foobar.com/v1/applications/WpM9nyZ2TbaEzfbRvLk9KA/passwordResetTokens/my-token-value-here";
            var properties = new Hashtable();
            properties.Add("href", href);
            IPasswordResetToken passwordResetToken = new DefaultPasswordResetToken(ds, properties);

            passwordResetToken.GetValue().ShouldBe("my-token-value-here");
        }
    }
}
