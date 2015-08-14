// <copyright file="Offset.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Offset : Linq_tests
    {
        private CollectionTestHarness<IAccount> harness;

        public Offset() : base()
        {
            harness = CollectionTestHarness<IAccount>.Create<IAccount>(Url, Resource);
        }

        [Fact]
        public void Filter_with_simple_parameter()
        {
            Assertly.Fail("todo");
        }

        [Fact]
        public void Filter_multiple_calls_are_LIFO()
        {
            Assertly.Fail("todo");
        }
    }
}
