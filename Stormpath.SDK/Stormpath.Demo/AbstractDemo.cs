// <copyright file="AbstractDemo.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Error;

namespace Stormpath.Demo
{
    public abstract class AbstractDemo : IDemo
    {
        protected readonly List<IAccount> createdAccounts;

        public AbstractDemo()
        {
            this.createdAccounts = new List<IAccount>();
        }

        public abstract Task CleanupAsync();

        public abstract Task RunAsync(CancellationToken cancellationToken);

        protected async Task RemoveAccountsAsync()
        {
            Console.WriteLine("Deleting accounts:");
            foreach (var account in this.createdAccounts)
            {
                try
                {
                    await account.DeleteAsync();
                    Console.WriteLine($"Deleted {account.Email}");
                }
                catch (ResourceException rex)
                {
                    Console.WriteLine($"Could not delete {account.Email}. Error: {rex.Message}");
                }
            }
        }
    }
}
