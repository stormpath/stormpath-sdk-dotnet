// <copyright file="DefaultHttpClientLoader_tests.cs" company="Stormpath, Inc.">
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

using NSubstitute;
using Shouldly;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultHttpClientLoader_tests
    {
        [Fact]
        public void Default_library_is_loaded()
        {
            ITypeLoader<IHttpClient> loader = new DefaultHttpClientLoader();

            // This test project has a reference to Stormpath.SDK.RestSharpClient, so the file lookup will succeed
            IHttpClient instance = null;
            var constructorArgs = new object[] { "http://api.foo.bar", 100, null, Substitute.For<ILogger>() };
            bool loadResult = loader.TryLoad(out instance, constructorArgs);

            loadResult.ShouldBeTrue();
            instance.ShouldNotBe(null);
        }
    }
}
