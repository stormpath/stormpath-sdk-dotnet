// <copyright file="WindowsVersion_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Introspection;
using Xunit;

namespace Stormpath.SDK.Tests.Introspection
{
    public class WindowsVersion_tests
    {
        [Fact]
        public void Identical_major_minor_versions_are_equal()
        {
            var wv1 = new WindowsVersion(5, 2);
            var wv2 = new WindowsVersion(5, 2);

            wv1.ShouldBe(wv2);
            (wv1 == wv2).ShouldBeTrue();
        }

        [Fact]
        public void Identical_major_minor_and_product_flags_are_equal()
        {
            var wv1 = new WindowsVersion(5, 2, 3);
            var wv2 = new WindowsVersion(5, 2, 3);

            wv1.ShouldBe(wv2);
            (wv1 == wv2).ShouldBeTrue();
        }

        [Fact]
        public void Different_major_minor_versions_are_unequal()
        {
            var wv1 = new WindowsVersion(5, 0);
            var wv2 = new WindowsVersion(5, 2);

            wv1.ShouldNotBe(wv2);
            (wv1 != wv2).ShouldBeTrue();
        }

        [Fact]
        public void Different_product_flags_are_unequal()
        {
            var wv1 = new WindowsVersion(5, 0, 0);
            var wv2 = new WindowsVersion(5, 2, 5);

            wv1.ShouldNotBe(wv2);
            (wv1 != wv2).ShouldBeTrue();
        }

        [Fact]
        public void Uneven_product_flags_are_unequal()
        {
            var wv1 = new WindowsVersion(5, 0);
            var wv2 = new WindowsVersion(5, 2, 5);

            wv1.ShouldNotBe(wv2);
            (wv1 != wv2).ShouldBeTrue();
        }
    }
}
