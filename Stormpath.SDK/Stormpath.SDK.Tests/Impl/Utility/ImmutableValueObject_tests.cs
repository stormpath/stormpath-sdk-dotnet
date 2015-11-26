// <copyright file="ImmutableValueObject_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Shared;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Utility
{
    public class ImmutableValueObject_tests
    {
        // Adapted from an example at http://www.mechonomics.com/generic-value-object-equality-updated/
        private class Address : ImmutableValueObject<Address>
        {
            private readonly string address1;
            private readonly string city;
            private readonly string state;

            public Address(string address1, string city, string state)
            {
                this.address1 = address1;
                this.city = city;
                this.state = state;
            }

            public string Address1 => this.address1;

            public string City => this.city;

            public string State => this.state;
        }

        private class ExpandedAddress : Address
        {
            private readonly string address2;

            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                this.address2 = address2;
            }

            public string Address2 => this.address2;
        }

        private class CaseInsensitiveAddress : ImmutableValueObject<CaseInsensitiveAddress>
        {
            private static Func<CaseInsensitiveAddress, CaseInsensitiveAddress, bool> EqualityFunction =>
                (a, b) =>
                {
                    return string.Equals(a?.Address1, b?.Address1, StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(a?.City, b?.City, StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(a?.State, b?.State, StringComparison.InvariantCultureIgnoreCase);
                };

            private readonly string address1;
            private readonly string city;
            private readonly string state;

            public CaseInsensitiveAddress(string address1, string city, string state)
                : base(EqualityFunction)
            {
                this.address1 = address1;
                this.city = city;
                this.state = state;
            }

            public string Address1 => this.address1;

            public string City => this.city;

            public string State => this.state;
        }

        public class Default_comparer_tests
        {
            [Fact]
            public void Equals_works_with_identical_values()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address1", "San Francisco", "CA");

                address.ShouldBe(address2);
            }

            [Fact]
            public void Equals_works_with_non_identical_values()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address2", "San Francisco", "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_works_with_nulls_in_first_object()
            {
                Address address = new Address(null, "San Francisco", "CA");
                Address address2 = new Address("Address2", "San Francisco", "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_works_with_nulls_in_second_object()
            {
                Address address = new Address("Address2", "San Francisco", "CA");
                Address address2 = new Address("Address2", null, "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_is_reflexive()
            {
                Address address = new Address("Address1", "San Francisco", "CA");

                address.ShouldBe(address);
            }

            [Fact]
            public void Equals_is_symmetrical()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address2", "San Francisco", "CA");

                address.ShouldNotBe(address2);
                address2.ShouldNotBe(address);
            }

            [Fact]
            public void Equals_is_transitive()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address1", "San Francisco", "CA");
                Address address3 = new Address("Address1", "San Francisco", "CA");

                address.ShouldBe(address2);
                address2.ShouldBe(address3);
                address.ShouldBe(address3);
            }

            [Fact]
            public void Equality_comparison_operators_work()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address1", "San Francisco", "CA");
                Address address3 = new Address("Address2", "San Francisco", "CA");

                (address == address2).ShouldBeTrue();
                (address2 != address3).ShouldBeTrue();
            }

            [Fact]
            public void Derived_types_behave_correctly()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");

                address.ShouldNotBe(address2);
                (address == address2).ShouldBeFalse();
            }

            [Fact]
            public void Derived_types_behave_correctly_when_fields_are_same()
            {
                var address = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");
                var address2 = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");

                address.ShouldBe(address2);
                (address == address2).ShouldBeTrue();
            }

            [Fact]
            public void Derived_types_behave_correctly_when_fields_are_different()
            {
                var address = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");
                var address2 = new ExpandedAddress("Address1", "Apt 456", "San Francisco", "CA");

                address.ShouldNotBe(address2);
                (address == address2).ShouldBeFalse();
            }

            [Fact]
            public void Derived_types_behave_correctly_when_inherited_fields_are_different()
            {
                var address = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");
                var address2 = new ExpandedAddress("Address2", "Apt 123", "Las Vegas", "NV");

                address.ShouldNotBe(address2);
                (address == address2).ShouldBeFalse();
            }

            [Fact]
            public void Equal_value_objects_have_same_hash_code()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address1", "San Francisco", "CA");

                Assert.Equal(address.GetHashCode(), address2.GetHashCode());
            }

            [Fact]
            public void Different_values_give_different_hash_codes()
            {
                Address address = new Address(null, "San Francisco", "CA");
                Address address2 = new Address("CA", "San Francisco", null);

                Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
            }

            [Fact]
            public void Unequal_value_objects_have_different_hash_codes()
            {
                Address address = new Address("Address1", "San Francisco", "CA");
                Address address2 = new Address("Address2", "San Francisco", "CA");

                Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
            }

            [Fact]
            public void Transposed_values_of_field_names_give_different_hash_codes()
            {
                Address address = new Address("city", null, null);
                Address address2 = new Address(null, "address1", null);

                Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
            }

            [Fact]
            public void Derived_types_hash_codes_behave_correctly()
            {
                ExpandedAddress address = new ExpandedAddress("Address99999", "Apt 123", "Austin", "TX");
                ExpandedAddress address2 = new ExpandedAddress("Address1", "Apt 123", "San Francisco", "CA");

                Assert.NotEqual(address.GetHashCode(), address2.GetHashCode());
            }
        }

        public class Custom_comparer_tests
        {
            [Fact]
            public void Equals_works_with_identical_values()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS1", "SAN FRANCISCO", "ca");

                address.ShouldBe(address2);
            }

            [Fact]
            public void Equals_works_with_non_identical_values()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS2", "SAN FRANCISCO", "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_works_with_nulls_in_first_object()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress(null, "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS1", "SAN FRANCISCO", "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_works_with_nulls_in_second_object()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS1", null, "CA");

                address.ShouldNotBe(address2);
            }

            [Fact]
            public void Equals_is_reflexive()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");

                address.ShouldBe(address);
            }

            [Fact]
            public void Equals_is_symmetrical()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("Address2", "San Francisco", "CA");
                CaseInsensitiveAddress address3 = new CaseInsensitiveAddress("ADDRESS1", "SAN FRANCISCO", "CA");

                address.ShouldNotBe(address2);
                address2.ShouldNotBe(address);

                address.ShouldBe(address3);
                address3.ShouldBe(address);
            }

            [Fact]
            public void Equals_is_transitive()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS1", "SAN FRANCISCO", "CA");
                CaseInsensitiveAddress address3 = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");

                address.ShouldBe(address2);
                address2.ShouldBe(address3);
                address.ShouldBe(address3);
            }

            [Fact]
            public void Equality_comparison_operators_work()
            {
                CaseInsensitiveAddress address = new CaseInsensitiveAddress("Address1", "San Francisco", "CA");
                CaseInsensitiveAddress address2 = new CaseInsensitiveAddress("ADDRESS1", "SAN FRANCISCO", "CA");
                CaseInsensitiveAddress address3 = new CaseInsensitiveAddress("Address2", "San Francisco", "CA");

                (address == address2).ShouldBeTrue();
                (address2 != address3).ShouldBeTrue();
            }
        }
    }
}
