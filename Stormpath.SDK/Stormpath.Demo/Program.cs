using System;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using System.Threading;
using Stormpath.SDK.Application;
using System.Linq;
using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.Error;

namespace Stormpath.Demo
{
    class Program
    {
        private static List<IAccount> addedUsers;

        static void Main(string[] args)
        {
            // Wire up the console cancel event (Ctrl+C) to cancel async tasks
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (source, evt) =>
            {
                evt.Cancel = true;
                cts.Cancel();
            };

            // Keep track of the accounts we've created so we can clean them up later
            addedUsers = new List<IAccount>();

            // Logically equivalent to MainAsync(...).Wait() but allows exceptions to bubble up unwrapped
            MainAsync(cts.Token).GetAwaiter().GetResult();

            // Clean up
            Console.WriteLine("\nCleaning up!");
            CleanupAsync().GetAwaiter().GetResult();

            Console.WriteLine("\nFinished! Press any key to exit...");
            Console.ReadKey(false);
        }

        static async Task MainAsync(CancellationToken cancellationToken)
        {
            var apiKey = ClientApiKeys.Builder()
                // This is actually unnecessary, because this is already the default search path
                .SetFileLocation("~\\.stormpath\\apiKey.properties")
                .Build();

            // Create an IClient object. Everything starts here!
            var client = Clients.Builder()
                .SetApiKey(apiKey)
                .Build();
            
            // Get current tenant
            var tenant = await client.GetCurrentTenantAsync(cancellationToken);
            Console.WriteLine($"Current tenant is: {tenant.Name}");

            // List applications
            Console.WriteLine("Tenant applications:");
            var applications = await tenant
                .GetApplications()
                .ToListAsync(cancellationToken);
            foreach (var app in applications)
            {
                Console.WriteLine($"{app.Name} ({(app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled")})");
            }
            if (!SpacebarToContinue(cancellationToken)) return;

            // Add some users
            var myApp = applications.First();
            Console.WriteLine($"\nAdding users to '{myApp.Name}'...");
            addedUsers.Add(
                await myApp.CreateAccountAsync("tk421@galacticempire.co", "Joe", "Stormtrooper", "Changeme123!", cancellationToken));
            addedUsers.Add(
                await myApp.CreateAccountAsync("lando@bespin.co", "Lando", "Calrissian", "Changeme123!", cancellationToken));

            // Another way to add users. Disable the default registration email workflow
            var vader = client.Instantiate<IAccount>();
            vader.SetEmail("vader@galacticempire.co");
            vader.SetGivenName("Darth");
            vader.SetSurname("Vader");
            vader.SetPassword("1findyourlackofsecuritydisturbing!");
            addedUsers.Add(
                await myApp.CreateAccountAsync(vader,
                options => options.RegistrationWorkflowEnabled = false,
                cancellationToken));
            if (!SpacebarToContinue(cancellationToken)) return;

            // List all accounts (this time with an asynchronous foreach)
            Console.WriteLine("\nApplication accounts:");
            await myApp.GetAccounts().ForEachAsync(account => Console.WriteLine($"{account.Email} {account.FullName} - {account.Status}"), cancellationToken);
            if (!SpacebarToContinue(cancellationToken)) return;

            // Authenticate a user
            var loginAs = addedUsers.First();
            Console.WriteLine($"\nLogging in as lando@bespin.co...");
            try
            {
                var loginSuccessful = await myApp.AuthenticateAccountAsync("lando@bespin.co", "Changeme123!", cancellationToken);
                Console.WriteLine($"Success! {loginAs.FullName} logged in.");
            }
            catch (ResourceException rex)
            {
                Console.WriteLine($"Could not log in. Error: {rex.Message}");
            }
            if (!SpacebarToContinue(cancellationToken)) return;
        }

        static async Task CleanupAsync()
        {
            Console.WriteLine("Deleting accounts:");
            foreach (var account in addedUsers)
            {
                try
                {
                    await account.DeleteAsync();
                    Console.WriteLine($"Deleted {account.Email}!");
                }
                catch (ResourceException rex)
                {
                    Console.WriteLine($"Could not delete {account.Email}. Error: {rex.Message}");
                }
            }
        }

        private static bool SpacebarToContinue(CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
                return false;

            Console.WriteLine("Press spacebar to continue...\n");
            var key = Console.ReadKey(true);
            return (key.KeyChar == ' ');
        }
    }
}
