using System;
using System.Threading.Tasks;
using System.Linq;
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
            bool isConfigurationPresent = !string.IsNullOrEmpty(Config.ApiKeyFileLocation);
            if (!isConfigurationPresent) throw new ArgumentNullException("Please add an ApiKeyFileLocation item to app.config!");

            // Wire up the console cancel event (Ctrl+C) to cancel async tasks
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (source, evt) =>
            {
                evt.Cancel = true;
                cts.Cancel();
            };

            MainAsync(cts.Token).GetAwaiter().GetResult();
            // Logically equivalent to MainAsync(...).Wait() but allows exceptions to bubble up unwrapped
        }

        static async Task MainAsync(CancellationToken ct)
        {
            var apiKey = ClientApiKeys.Builder()
                .SetFileLocation(Config.ApiKeyFileLocation)
                .Build();

            if (!apiKey.IsValid()) throw new ArgumentException("The provided API key is not valid.");
            if (apiKey.GetId() == "<your API key ID here>") throw new ArgumentException("Please insert your real Stormpath API key ID.");
            if (apiKey.GetSecret() == "<your API key secret here>") throw new ArgumentException("Please insert your real Stormpath API key secret.");

            // Create an IClient object. Everything starts here!
            var client = Clients.Builder()
                .SetApiKey(apiKey)
                .Build();
            
            // Get current tenant
            var tenant = await client.GetCurrentTenantAsync();
            Console.WriteLine($"Current tenant is: {tenant.Name}");

            // Get 
            var myApp = tenant.GetApplications()
                .Where(x => x.Name == "My Application")
                .First();

            // List applications
            //Console.WriteLine("Tenant applications:");
            //var applications = await tenant.GetApplicationsAsync();
            //foreach (var app in applications)
            //{
            //    Console.WriteLine("{0}\t{1}", app.Name, app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled");
            //}

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

        // TODO
        static Task<bool> SpacebarToContinue(CancellationToken cancelToken)
        {
            do
            {
                if (cancelToken.IsCancellationRequested) return Task.FromResult(false);

                var key = Console.ReadKey(true);

                if (key.KeyChar == ' ') return Task.FromResult(true);

            } while (true);
        }
    }
}
