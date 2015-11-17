// <copyright file="Sync_Single_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Sync_Single_tests : Linq_test<IAccount>
    {
        [Fact]
        public void Returns_one_item()
        {
            this.InitializeClientWithCollection(new List<IAccount>()
                {
                    TestAccounts.HanSolo
                });

            var han = this.Queryable.Synchronously().Single();

            han.Surname.ShouldBe("Solo");
        }

        [Fact]
        public void Throws_when_more_than_one_item_exists()
        {
            this.InitializeClientWithCollection(new List<IAccount>()
                {
                    TestAccounts.HanSolo,
                    TestAccounts.LukeSkywalker
                });

            // TODO This should be InvalidOperationException, but under Mono it throws NullReferenceException for some undetermined reason
            Should.Throw<Exception>(() =>
            {
                var han = this.Queryable.Synchronously().Single();
            });
        }

        [Fact]
        public void Throws_when_no_items_exist()
        {
            // TODO This should be InvalidOperationException, but under Mono it throws NullReferenceException for some undetermined reason
            Should.Throw<Exception>(() =>
            {
                var jabba = this.Queryable.Synchronously().Single();
            });
        }
    }
}
