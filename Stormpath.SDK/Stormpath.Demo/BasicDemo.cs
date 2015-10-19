using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Error;

namespace Stormpath.Demo
{
    public class BasicDemo : IDemo
    {
        private readonly List<IAccount> addedUsers;

        public BasicDemo()
        {
            addedUsers = new List<IAccount>();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
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
            Console.WriteLine($"{Strings.NL}Tenant applications:");
            var applications = await tenant
                .GetApplications()
                .ToListAsync(cancellationToken);
            foreach (var app in applications)
            {
                Console.WriteLine($"{Helpers.TrimWithEllipse(app.Name, 15),-15} {(app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled")}");
            }
            if (!Helpers.SpacebarToContinue(cancellationToken)) return;

            // Add some users
            var myApp = applications.Where(x => x.Name == "My Application").Single();
            Console.WriteLine($"{Strings.NL}Adding users to '{myApp.Name}'...");
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
            Console.WriteLine($"{Strings.NL}Application accounts:");
            await myApp.GetAccounts().ForEachAsync(account => Console.WriteLine($"{Helpers.TrimWithEllipse(account.Email, 25),-25} {Helpers.TrimWithEllipse(account.FullName, 20),-20} {account.Status.ToString().ToLower()}"), cancellationToken);
            if (!Helpers.SpacebarToContinue(cancellationToken)) return;

            // Authenticate a user
            Console.WriteLine($"{Strings.NL}Logging in as vader@galacticempire.co");
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
            if (!Helpers.SpacebarToContinue(cancellationToken)) return;
        }

        public async Task CleanupAsync()
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
    }
}
