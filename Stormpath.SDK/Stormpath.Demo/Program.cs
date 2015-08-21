using System;
using System.Threading.Tasks;
using System.Linq;
using Stormpath.SDK;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;
using System.Threading;
using Stormpath.SDK.Application;

namespace Stormpath.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Wire up the console cancel event (Ctrl+C) to cancel async tasks
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (source, evt) =>
            {
                evt.Cancel = true;
                cts.Cancel();
            };

            // Logically equivalent to MainAsync(...).Wait() but allows exceptions to bubble up unwrapped
            MainAsync(cts.Token).GetAwaiter().GetResult();

            Console.WriteLine("Finished! Press any key to exit...");
            Console.ReadKey(false);
        }

        static async Task MainAsync(CancellationToken ct)
        {
            var apiKey = ClientApiKeys.Builder()
                // This is actually unnecessary, because this is the default search path
                .SetFileLocation("~\\.stormpath\\apiKey.properties")
                .Build();

            // Create an IClient object. Everything starts here!
            var client = Clients.Builder()
                .SetApiKey(apiKey)
                .Build();
            
            // Get current tenant
            var tenant = await client.GetCurrentTenantAsync();
            Console.WriteLine($"Current tenant is: {tenant.Name}");
            if (!SpacebarToContinue(ct)) return;

            // List applications
            Console.WriteLine("Tenant applications:");
            var applications = await tenant.GetApplications().ToListAsync();
            foreach (var app in applications)
            {
                Console.WriteLine("{0} ({1})", app.Name, app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled");
            }
            if (!SpacebarToContinue(ct)) return;

            //// Add some users
            //Console.WriteLine("\nAdding users to '{0}'...", myApp.Name);
            //var addedUsers = new List<string>();
            //var result = await myApp.CreateAccountAsync("tk421@stormpath.com", "Joe", "Stormtrooper", "tk421", "Changeme1");
            //if (result.Status == AccountStatus.Enabled)
            //    addedUsers.Add(result.Email);

            //result = await myApp.CreateAccountAsync("lando@bespin.co", "Lando", "Calrissian", "lcalrissian", "Changeme1", new
            //{
            //    quote = "You got a lotta nerve showin' your face around here, after what you pulled."
            //});
            //if (result.Status == AccountStatus.Enabled)
            //    addedUsers.Add(result.Email);

            //// Print them
            //Console.WriteLine("\nApplication accounts:");
            //var accounts = await myApp.GetAccountsAsync();
            //foreach (var account in accounts)
            //{
            //    Console.WriteLine("{0} {1} - {2}", account.Email, account.FullName, account.Status.ToString());
            //}

            //// Authenticate a user
            //var loginAs = accounts.First();
            //Console.WriteLine("\nLogging in as {0}...", loginAs.UserName);
            //var didLogin = await myApp.AuthenticateAccountAsync(loginAs.UserName, "Changeme1");
            //Console.WriteLine("{0}", didLogin ? "Success!" : "Error :(");

            //// Create a group
            //// todo

            //// Assign user to group
            //// todo

            //// Clean up!
            //Console.WriteLine("\nCleaning up!");
            //Console.WriteLine("Deleting accounts:");
            //foreach (var email in addedUsers)
            //{
            //    var account = accounts
            //        .Where(x => x.Email == email)
            //        .Single();
            //    var deleted = await account.DeleteAsync();
            //    Console.WriteLine("Deleted {0} - {1}", account.Email, deleted ? "done" : "error");
            //}

            //Console.Write("\nPress any key to exit...");
            //Console.ReadKey(false);
        }

        private static bool SpacebarToContinue(CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
                return false;

            Console.WriteLine("\nPress spacebar to continue...\n");
            var key = Console.ReadKey(true);
            return (key.KeyChar == ' ');
        }
    }
}
