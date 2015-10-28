// <copyright file="Expand_resource_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Expand_account_tests : Linq_tests<IAccount>
    {
        [Fact]
        public void Delete_throws()
        {
            var query = this.Harness.Queryable.Expand(x => x.DeleteAsync);

            Should.Throw<NotSupportedException>(() =>
            {
                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            });
        }

        [Fact]
        public void CustomData_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetCustomDataAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=customData");
        }

        [Fact]
        public void Directory_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetDirectoryAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=directory");
        }

        [Fact]
        public void GroupMemberships_are_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetGroupMemberships);

            query.GeneratedArgumentsWere(this.Href, "expand=groupMemberships");
        }

        [Fact]
        public void GroupMemberships_are_expanded_with_options()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetGroupMemberships, offset: 7331, limit: 1337);

            query.GeneratedArgumentsWere(this.Href, "expand=groupMemberships(offset:7331,limit:1337)");
        }

        [Fact]
        public void Groups_are_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetGroups);

            query.GeneratedArgumentsWere(this.Href, "expand=groups");
        }

        [Fact]
        public void Groups_are_expanded_with_options()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetGroups, offset: 123, limit: 321);

            query.GeneratedArgumentsWere(this.Href, "expand=groups(offset:123,limit:321)");
        }

        [Fact]
        public void ProviderData_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetProviderDataAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=providerData");
        }

        [Fact]
        public void Tenant_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetTenantAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=tenant");
        }

        [Fact]
        public void Save_throws()
        {
            var query = this.Harness.Queryable.Expand(x => x.SaveAsync);

            Should.Throw<NotSupportedException>(() =>
            {
                query.GeneratedArgumentsWere(this.Href, "<not evaluated>");
            });
        }
    }
}