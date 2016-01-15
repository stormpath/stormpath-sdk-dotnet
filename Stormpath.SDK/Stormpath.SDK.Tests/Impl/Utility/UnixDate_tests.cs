// <copyright file="UnixDate_tests.cs" company="Stormpath, Inc.">
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

using System;
using Shouldly;
using Stormpath.SDK.Impl.Utility;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class UnixDate_tests
    {
        [Fact]
        public void Start_of_Unix_epoch_is_zero()
        {
            var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

            UnixDate.ToLong(epoch).ShouldBe(0);
        }

        [Fact]
        public void Dec_31_2012_is_1356134399()
        {
            var endOfMayanLongCountCycle = new DateTimeOffset(2012, 12, 21, 23, 59, 59, TimeSpan.Zero);

            // Editor's note: World did not cataclysmically end on this date.
            UnixDate.ToLong(endOfMayanLongCountCycle).ShouldBe(1356134399);
        }

        [Fact]
        public void Unix_1435665600_is_noon_June_30_2015()
        {
            var june30Noon = UnixDate.FromLong(1435665600);

            june30Noon.Year.ShouldBe(2015);
            june30Noon.Month.ShouldBe(6);
            june30Noon.Day.ShouldBe(30);
            june30Noon.Hour.ShouldBe(12);
            june30Noon.Minute.ShouldBe(00);
            june30Noon.Second.ShouldBe(00);
        }
    }
}
