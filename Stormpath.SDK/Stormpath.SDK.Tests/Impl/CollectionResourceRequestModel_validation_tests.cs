// <copyright file="CollectionResourceRequestModel_validation_tests.cs" company="Stormpath, Inc.">
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

using Shouldly;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class CollectionResourceRequestModel_validation_tests
    {
        public class Expansion_term
        {
            [Fact]
            public void With_null_subparameters_is_valid()
            {
                var expansion = new ExpansionTerm("foo");

                expansion.IsValid().ShouldBeTrue();
            }

            [Fact]
            public void With_subparameters_is_valid()
            {
                var expansion = new ExpansionTerm("foo", 10, 10);

                expansion.IsValid().ShouldBeTrue();
            }

            [Fact]
            public void With_empty_fieldname_is_invalid()
            {
                var expansion = new ExpansionTerm(string.Empty);

                expansion.IsValid().ShouldBeFalse();
            }

            [Fact]
            public void With_offset_zero_is_valid()
            {
                var expansion = new ExpansionTerm("foo", offset: 0);

                expansion.IsValid().ShouldBeTrue();
            }

            [Fact]
            public void With_offset_negative_is_invalid()
            {
                var expansion = new ExpansionTerm("foo", offset: -1);

                expansion.IsValid().ShouldBeFalse();
            }

            [Fact]
            public void With_limit_zero_is_invalid()
            {
                var expansion = new ExpansionTerm("foo", limit: 0);

                expansion.IsValid().ShouldBeFalse();
            }

            [Fact]
            public void With_limit_negative_is_invalid()
            {
                var expansion = new ExpansionTerm("foo", limit: -1);

                expansion.IsValid().ShouldBeFalse();
            }
        }
    }
}
