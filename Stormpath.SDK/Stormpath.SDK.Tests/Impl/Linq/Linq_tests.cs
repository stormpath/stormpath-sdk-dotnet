// <copyright file="Linq_tests.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Tests.Helpers;

namespace Stormpath.SDK.Tests.Impl.Linq
{
    public class Linq_tests
    {
        private static string href = "http://f.oo/bar";
        private CollectionTestHarness<IAccount> harness;

        public Linq_tests()
        {
            // Default test harness. Child classes can overwrite
            this.harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href);
        }

        internal Linq_tests(IInternalDataStore ds)
        {
            this.harness = CollectionTestHarness<IAccount>.Create<IAccount>(this.Href, ds as IInternalAsyncDataStore);
        }

        protected string Href => href;

        protected CollectionTestHarness<IAccount> Harness
        {
            get { return this.harness; }

            set { this.harness = value; }
        }
    }
}
