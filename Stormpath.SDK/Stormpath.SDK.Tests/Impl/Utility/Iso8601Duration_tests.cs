// <copyright file="Iso8601Duration_tests.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using Shouldly;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class Iso8601Duration_tests
    {
        public static IEnumerable<object[]> ValidDurations()
        {
            yield return new object[] { string.Empty, new SerializableTimeSpan(TimeSpan.Zero) };
            yield return new object[] { "P4W", new SerializableTimeSpan(TimeSpan.FromDays(28)) };
            yield return new object[] { "P7D", new SerializableTimeSpan(TimeSpan.FromDays(7)) };
            yield return new object[] { "PT23H", new SerializableTimeSpan(TimeSpan.FromHours(23)) };
            yield return new object[] { "PT5M", new SerializableTimeSpan(TimeSpan.FromMinutes(5)) };
            yield return new object[] { "PT10S", new SerializableTimeSpan(TimeSpan.FromSeconds(10)) };
            yield return new object[] { "P4W1D", new SerializableTimeSpan(TimeSpan.FromDays(29)) };
            yield return new object[] { "PT1H", new SerializableTimeSpan(TimeSpan.FromHours(1)) };
            yield return new object[] { "P8W4D", new SerializableTimeSpan(TimeSpan.FromDays(60)) };
            yield return new object[] { "P1DT1H", new SerializableTimeSpan(TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1))) };
            yield return new object[] { "P1DT1M", new SerializableTimeSpan(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(1))) };
            yield return new object[] { "PT1H30M", new SerializableTimeSpan(TimeSpan.FromHours(1.5)) };
            yield return new object[] { "PT1.5S", new SerializableTimeSpan(TimeSpan.FromSeconds(1.5)) };
            yield return new object[] { "P25W5D", new SerializableTimeSpan(TimeSpan.FromDays(180)) };
            yield return new object[] { "P1W4DT12H30M5S", new SerializableTimeSpan(TimeSpan.FromDays(11).Add(TimeSpan.FromHours(12)).Add(TimeSpan.FromMinutes(30)).Add(TimeSpan.FromSeconds(5))) };
        }

        /// <summary>
        /// Additional valid test cases not included in <see cref="TestCases.ValidDurations"/>.
        /// </summary>
        /// <returns>Enumerable test cases for an xUnit <see cref="Xunit.TheoryAttribute">Theory</see>.</returns>
        public static IEnumerable<object[]> AdditionalValidDurations()
        {
            yield return new object[] { "PT300S", new SerializableTimeSpan(TimeSpan.FromSeconds(300)) };
            yield return new object[] { "PT60M", new SerializableTimeSpan(TimeSpan.FromMinutes(60)) };
            yield return new object[] { "P60D", new SerializableTimeSpan(TimeSpan.FromDays(60)) };
            yield return new object[] { "P61D", new SerializableTimeSpan(TimeSpan.FromDays(61)) };
            yield return new object[] { "P4WT", new SerializableTimeSpan(TimeSpan.FromDays(28)) };
            yield return new object[] { "PT1.5H", new SerializableTimeSpan(TimeSpan.FromHours(1.5)) };
            yield return new object[] { "P180D", new SerializableTimeSpan(TimeSpan.FromDays(180)) };
        }

        /// <summary>
        /// Tests for valid ISO 8601 duration strings.
        /// </summary>
        /// <param name="iso8601">The ISO 8601 duration string.</param>
        /// <param name="expected">The expected <see cref="TimeSpan"/>.</param>
        [Theory]
        [MemberData(nameof(ValidDurations))]
        [MemberData(nameof(AdditionalValidDurations))]
        public void Valid_duration(string iso8601, SerializableTimeSpan expected)
        {
            TimeSpan result;

            Iso8601Duration.TryParse(iso8601, out result).ShouldBe(true);

            result.ShouldBe(expected);
        }

        /// <summary>
        /// Regression tests for unsupported ISO 8601 designators.
        /// </summary>
        /// <param name="unsupported">An ISO 8601 duration string containing unsupported designators.</param>
        [Theory]
        [InlineData("P1Y")]
        [InlineData("P1M")]
        [InlineData("P1MT1M")]
        public void Unsupported_duration(string unsupported)
        {
            TimeSpan dummy;
            Iso8601Duration.TryParse(unsupported, out dummy).ShouldBe(false);
        }

        /// <summary>
        /// Tests for invalid ISO 8601 duration strings.
        /// </summary>
        /// <param name="invalid">An ISO 8601 duration containing invalid syntax.</param>
        [Theory]
        [InlineData("P")]
        [InlineData("foobar")]
        [InlineData("PfoobarT")]
        [InlineData("3W")]
        [InlineData("P3")]
        [InlineData("P3W2")]
        [InlineData("PW")]
        [InlineData("P3WTM")]
        [InlineData("P3fooD")]
        [InlineData("PT1xM")]
        public void Invalid_duration(string invalid)
        {
            TimeSpan dummy;
            Iso8601Duration.TryParse(invalid, out dummy).ShouldBe(false);
        }

        /// <summary>
        /// Asserts that the ISO 8601-formatted output is as expected
        /// for a given input <see cref="System.TimeSpan"/>.
        /// </summary>
        /// <param name="expected">The expected output string.</param>
        /// <param name="timeSpan">The time span.</param>
        [Theory]
        [MemberData(nameof(ValidDurations))]
        public void Formatting(string expected, SerializableTimeSpan timeSpan)
        {
            Iso8601Duration.Format(timeSpan).ShouldBe(expected);
        }
    }
}
