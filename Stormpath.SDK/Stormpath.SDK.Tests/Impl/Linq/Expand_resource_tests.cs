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
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
#pragma warning disable SA1402 // File may only contain a single class
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

    public class Expand_application_tests : Linq_tests<IApplication>
    {
        [Fact]
        public void Accounts_are_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccounts);

            query.GeneratedArgumentsWere(this.Href, "expand=accounts");
        }

        [Fact]
        public void Accounts_are_expanded_with_options()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccounts, offset: 123, limit: 321);

            query.GeneratedArgumentsWere(this.Href, "expand=accounts(offset:123,limit:321)");
        }

        [Fact]
        public void AccountStoreMappings_are_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccountStoreMappings);

            query.GeneratedArgumentsWere(this.Href, "expand=accountStoreMappings");
        }

        [Fact]
        public void AccountStoreMappings_are_expanded_with_options()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccountStoreMappings, offset: 123, limit: 321);

            query.GeneratedArgumentsWere(this.Href, "expand=accountStoreMappings(offset:123,limit:321)");
        }

        [Fact]
        public void DefaultAccountStore_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetDefaultAccountStoreAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=defaultAccountStoreMapping");
        }

        [Fact]
        public void DefaultGroupStore_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetDefaultGroupStoreAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=defaultGroupStoreMapping");
        }
    }

    public class Expand_directory_tests : Linq_tests<IDirectory>
    {
        [Fact]
        public void Provider_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetProviderAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=provider");
        }
    }

    public class Expand_group_tests : Linq_tests<IGroup>
    {
        [Fact]
        public void AccountMemberships_are_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccountMemberships);

            query.GeneratedArgumentsWere(this.Href, "expand=accountMemberships");
        }

        [Fact]
        public void AccountMemberships_are_expanded_with_options()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccountMemberships, offset: 7331, limit: 1337);

            query.GeneratedArgumentsWere(this.Href, "expand=accountMemberships(offset:7331,limit:1337)");
        }
    }

    public class Expand_group_membership_tests : Linq_tests<IGroupMembership>
    {
        [Fact]
        public void Account_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetAccountAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=account");
        }

        [Fact]
        public void Group_is_expanded()
        {
            var query = this.Harness.Queryable.Expand(x => x.GetGroupAsync);

            query.GeneratedArgumentsWere(this.Href, "expand=group");
        }
    }
#pragma warning restore SA1402 // File may only contain a single class
}