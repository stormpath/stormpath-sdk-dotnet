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
            if (addedUsers.Any())
            {
                Console.WriteLine("\nCleaning up!");
                CleanupAsync().GetAwaiter().GetResult();
            }

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
            Console.WriteLine("\nTenant applications:");
            var applications = await tenant
                .GetApplications()
                .ToListAsync(cancellationToken);
            foreach (var app in applications)
            {
                Console.WriteLine($"{TrimWithEllipse(app.Name, 15), -15} {(app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled")}");
            }
            if (!SpacebarToContinue(cancellationToken)) return;

            // Add some users
            var myApp = applications.First();
            Console.WriteLine($"\nAdding users to '{myApp.Name}'...");
            addedUsers.Add(
                await myApp.CreateAccountAsync("Joe", "Stormtrooper", "tk421@galacticempire.co", "Changeme123!", customData: cancellationToken));
            addedUsers.Add(
                await myApp.CreateAccountAsync("Lando", "Calrissian", "lando@bespin.co", "Changeme123!", 
                                                new { phrase = "You got a lotta nerve, showing your face after what you pulled." }, cancellationToken));

            // Another way to add users. Disable the default registration email workflow
            var vader = client.Instantiate<IAccount>();
            vader.SetEmail("vader@galacticempire.co");
            vader.SetGivenName("Darth");
            vader.SetSurname("Vader");
            vader.SetPassword("Togetherw3willrulethegalaxy!");
            vader.CustomData.Put("phrase", "I find your lack of faith disturbing.");
            addedUsers.Add(
                await myApp.CreateAccountAsync(vader,
                    options => options.RegistrationWorkflowEnabled = false,
                cancellationToken));

            // List all accounts (this time with an asynchronous foreach)
            Console.WriteLine("\nApplication accounts:");
            await myApp.GetAccounts().ForEachAsync(account => Console.WriteLine($"{TrimWithEllipse(account.Email, 25), -25} {TrimWithEllipse(account.FullName, 20), -20} {account.Status.ToString().ToLower()}"), cancellationToken);
            if (!SpacebarToContinue(cancellationToken)) return;

            // Authenticate a user
            Console.WriteLine($"\nLogging in as vader@galacticempire.co");
            try
            {
                var result = await myApp.AuthenticateAccountAsync("vader@galacticempire.co", "Togetherw3willrulethegalaxy!", cancellationToken);
                var returnedAccount = await result.GetAccountAsync();
                var customData = await returnedAccount.GetCustomDataAsync();
                Console.WriteLine($"Success! {returnedAccount.FullName} logged in.");
                Console.WriteLine($"Famous phrase: '{customData["phrase"]}'");
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
                    Console.WriteLine($"Deleted {account.Email}");
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

            Console.Write("\nPress spacebar to continue");
            var key = Console.ReadKey(true);

            if (cancelToken.IsCancellationRequested)
                return false;

            ClearCurrentLine(Console.CursorTop);
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            return (key.KeyChar == ' ');
        }

        private static string TrimWithEllipse(string input, int maxLength)
        {
            if (input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength - 3) + "...";
        }

        private static void ClearCurrentLine(int line)
        {
            Console.SetCursorPosition(0, line);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(0, line);
        }
    }
}
