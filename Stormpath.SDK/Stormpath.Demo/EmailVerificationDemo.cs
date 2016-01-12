// <copyright file="EmailVerificationDemo.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Api;
using Stormpath.SDK.Client;

namespace Stormpath.Demo
{
    // Disabling some StyleCop rules
#pragma warning disable SA1101 // Prefix local calls with this

    public class EmailVerificationDemo : AbstractDemo
    {
        private static bool ShouldContinue()
        {
            Console.WriteLine("This demo assumes you have an application called 'EmailWorkflowDemo' in your tenant.");
            Console.WriteLine("This application must be configured with the Verification Email workflow turned on.");
            Console.Write("Do you want to continue? [no] ");

            var input = Console.ReadLine();

            return input.Equals("yes", StringComparison.CurrentCultureIgnoreCase);
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!ShouldContinue())
            {
                return;
            }

            var apiKey = ClientApiKeys.Builder()
                .SetFileLocation("~\\.stormpath\\apiKey.properties")
                .Build();

            // Create an IClient object. Everything starts here!
            var client = Clients.Builder()
                .SetApiKey(apiKey)
                .Build();

            var application = await client.GetApplications()
                .Where(x => x.Name == "EmailWorkflowDemo")
                .SingleOrDefaultAsync(cancellationToken);

            if (application == null)
            {
                Console.WriteLine("Error: Could not find application. Skipping demo.");
                return;
            }

            var createdAccount = await application.CreateAccountAsync("Email", "Test", "test@foo.co", "Changeme123!", cancellationToken);
            Console.WriteLine($"Account {createdAccount.Email} created");

            var accountStore = await application.GetDefaultAccountStoreAsync();

            await application.SendVerificationEmailAsync(req =>
            {
                req.Login = "test@foo.co";
                req.AccountStore = accountStore;
            });
            Console.WriteLine($"Verification email resent to {createdAccount.Email}");

            var token = createdAccount.EmailVerificationToken.GetValue();
            var verifiedAccount = await client.VerifyAccountEmailAsync(token);
            Console.WriteLine($"{createdAccount.Email} verified with token {token}");

            // Clean up
            await createdAccount.DeleteAsync();
        }

        public override async Task CleanupAsync()
        {
            await RemoveAccountsAsync();
        }
    }

#pragma warning restore SA1101 // Prefix local calls with this
}
