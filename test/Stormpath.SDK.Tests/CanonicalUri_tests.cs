// <copyright file="CanonicalUri_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Http;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class CanonicalUri_tests
    {
        [Fact]
        public void Throws_when_URI_is_empty()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var bad = new CanonicalUri(string.Empty);
            });
        }

        [Fact]
        public void Throws_when_URI_is_not_fully_qualified()
        {
            var notFullyQualified = "/path/to/resource";

            Should.Throw<ArgumentException>(() =>
            {
                var bad = new CanonicalUri(notFullyQualified);
            });

            Should.Throw<ArgumentException>(() =>
            {
                var bad = new CanonicalUri(notFullyQualified, new QueryString("foo=bar&baz=123"));
            });
        }
    }
}
