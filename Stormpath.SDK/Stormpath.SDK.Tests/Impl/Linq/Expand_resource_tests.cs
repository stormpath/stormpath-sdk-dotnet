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
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Group;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
#pragma warning disable SA1402 // File may only contain a single class
#pragma warning disable SA1649 // File name must match first class
    public class Expand_account_tests : Linq_test<IAccount>
    {
        [Fact]
        public async Task CustomData_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetCustomData())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=customData");
        }

        [Fact]
        public async Task Directory_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetDirectory())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=directory");
        }

        [Fact]
        public async Task GroupMemberships_are_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetGroupMemberships())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=groupMemberships");
        }

        [Fact]
        public async Task GroupMemberships_are_expanded_with_options()
        {
            await this.Queryable
                .Expand(x => x.GetGroupMemberships(7331, 1337))
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=groupMemberships(offset:7331,limit:1337)");
        }

        [Fact]
        public async Task Groups_are_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetGroups())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=groups");
        }

        [Fact]
        public async Task Groups_are_expanded_with_options()
        {
            await this.Queryable
                .Expand(x => x.GetGroups(123, 321))
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=groups(offset:123,limit:321)");
        }

        [Fact]
        public async Task ProviderData_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetProviderData())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=providerData");
        }

        [Fact]
        public async Task Tenant_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetTenant())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=tenant");
        }
    }

    public class Expand_application_tests : Linq_test<IApplication>
    {
        [Fact]
        public async Task Accounts_are_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetAccounts())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accounts");
        }

        [Fact]
        public async Task Accounts_are_expanded_with_options()
        {
            await this.Queryable
                .Expand(x => x.GetAccounts(123, 321))
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accounts(offset:123,limit:321)");
        }

        [Fact]
        public async Task AccountStoreMappings_are_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetAccountStoreMappings())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accountStoreMappings");
        }

        [Fact]
        public async Task AccountStoreMappings_are_expanded_with_options()
        {
            await this.Queryable
                .Expand(x => x.GetAccountStoreMappings(123, 321))
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accountStoreMappings(offset:123,limit:321)");
        }

        [Fact]
        public async Task DefaultAccountStore_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetDefaultAccountStore())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=defaultAccountStoreMapping");
        }

        [Fact]
        public async Task DefaultGroupStore_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetDefaultGroupStore())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=defaultGroupStoreMapping");
        }
    }

    public class Expand_directory_tests : Linq_test<IDirectory>
    {
        [Fact]
        public async Task Provider_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetProvider())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=provider");
        }
    }

    public class Expand_group_tests : Linq_test<IGroup>
    {
        [Fact]
        public async Task AccountMemberships_are_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetAccountMemberships())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accountMemberships");
        }

        [Fact]
        public async Task AccountMemberships_are_expanded_with_options()
        {
            await this.Queryable
                .Expand(x => x.GetAccountMemberships(7331, 1337))
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=accountMemberships(offset:7331,limit:1337)");
        }
    }

    public class Expand_group_membership_tests : Linq_test<IGroupMembership>
    {
        [Fact]
        public async Task Account_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetAccount())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=account");
        }

        [Fact]
        public async Task Group_is_expanded()
        {
            await this.Queryable
                .Expand(x => x.GetGroup())
                .MoveNextAsync();

            this.ShouldBeCalledWithArgument("expand=group");
        }
    }
#pragma warning restore SA1402 // File may only contain a single class
#pragma warning restore SA1649 // File name must match first class
}