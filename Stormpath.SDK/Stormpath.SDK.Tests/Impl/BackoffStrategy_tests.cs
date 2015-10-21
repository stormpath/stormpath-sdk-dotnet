// <copyright file="BackoffStrategy_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Http.Support;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class BackoffStrategy_tests
    {
        private static readonly int MaxBackoffMilliseconds = 20 * 1000;

        [Theory]
        [InlineData(600, 1)]
        [InlineData(1200, 2)]
        [InlineData(2400, 3)]
        [InlineData(4800, 4)]
        [InlineData(9600, 5)]
        [InlineData(19200, 6)]
        [InlineData(20000, 7)]
        [InlineData(20000, 8)]
        public void Default_strategy_is(int expectedDelay, int forRetry)
        {
            var backoffStrategy = new DefaultBackoffStrategy(MaxBackoffMilliseconds);

            backoffStrategy
                .GetDelayMilliseconds(forRetry)
                .ShouldBe(expectedDelay);
        }

        [Theory]
        [InlineData(1000, 1)]
        [InlineData(2000, 2)]
        [InlineData(4000, 3)]
        [InlineData(8000, 4)]
        [InlineData(16000, 5)]
        [InlineData(20000, 6)]
        [InlineData(20000, 7)]
        public void Throttling_strategy_is(int expectedDelay, int forRetry)
        {
            var backoffStrategy = new ThrottlingBackoffStrategy(MaxBackoffMilliseconds);

            backoffStrategy
                .GetDelayMilliseconds(forRetry)
                .ShouldBeGreaterThanOrEqualTo(expectedDelay);
        }
    }
}
