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
            createdAccounts = new List<IAccount>();
        }

        public abstract Task CleanupAsync();
        public abstract Task RunAsync(CancellationToken cancellationToken);

        protected async Task RemoveAccountsAsync()
        {
            Console.WriteLine("Deleting accounts:");
            foreach (var account in createdAccounts)
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
