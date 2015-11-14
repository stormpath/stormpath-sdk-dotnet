// <copyright file="BasicDemo.cs" company="Stormpath, Inc.">
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
    // Disabling some StyleCop rules
#pragma warning disable SA1101 // Prefix local calls with this
#pragma warning disable SA1117 // Place all parameters on one line
#pragma warning disable SA1305 // Variables must not use Hungarian notation

    public class BasicDemo : AbstractDemo
    {
        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            var apiKey = ClientApiKeys.Builder()
                .SetFileLocation("~\\.stormpath\\apiKey.properties") // This is actually unnecessary, because this is already the default search path
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
                Console.WriteLine($"{Helpers.TrimWithEllipse(app.Name, 15), -15} {(app.Status == ApplicationStatus.Enabled ? "enabled" : "disabled")}");
            }

            if (!Helpers.SpacebarToContinue(cancellationToken))
                return;

            // Add some users
            var myApp = applications.Where(x => x.Name == "My Application").Single();
            Console.WriteLine($"{Strings.NL}Adding users to '{myApp.Name}'...");

            this.createdAccounts.Add(
                await myApp.CreateAccountAsync("Joe", "Stormtrooper", "tk421@galacticempire.co", "Changeme123!", customData: cancellationToken));
            this.createdAccounts.Add(
                await myApp.CreateAccountAsync("Lando", "Calrissian", "lando@bespin.co", "Changeme123!",
                                                new { phrase = "You got a lotta nerve, showing your face after what you pulled." }, cancellationToken));

            // Another way to add users. Disable the default registration email workflow
            var vader = client.Instantiate<IAccount>();
            vader.SetEmail("vader@galacticempire.co");
            vader.SetGivenName("Darth");
            vader.SetSurname("Vader");
            vader.SetPassword("Togetherw3willrulethegalaxy!");
            vader.CustomData.Put("phrase", "I find your lack of faith disturbing.");

            this.createdAccounts.Add(await myApp.CreateAccountAsync(
                    vader,
                    options => options.RegistrationWorkflowEnabled = false,
                cancellationToken));

            // List all accounts (this time with an asynchronous foreach)
            Console.WriteLine($"{Strings.NL}Application accounts:");
            await myApp.GetAccounts().ForEachAsync(account => Console.WriteLine($"{Helpers.TrimWithEllipse(account.Email, 25), -25} {Helpers.TrimWithEllipse(account.FullName, 20), -20} {account.Status.ToString().ToLower()}"), cancellationToken);
            if (!Helpers.SpacebarToContinue(cancellationToken))
                return;

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

            if (!Helpers.SpacebarToContinue(cancellationToken))
                return;
        }

        public override async Task CleanupAsync()
        {
            await RemoveAccountsAsync();
        }
    }

#pragma warning restore SA1101 // Prefix local calls with this
#pragma warning restore SA1117 // Place all parameters on one line
#pragma warning restore SA1305 // Variables must not use Hungarian notation
}
