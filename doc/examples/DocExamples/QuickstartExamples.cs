// <copyright file="QuickstartExamples.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;
using Stormpath.SDK;
using Stormpath.SDK.Client;
using Stormpath.SDK.Error;

namespace examples
{
    /// <summary>
    /// Content from README.md and the Product Guide
    /// </summary>
    public class QuickstartExamples
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            // Create an IClient. Everything starts here!
            IClient client = Clients.Builder()
                .SetApiKeyFilePath("path\\to\\apiKey.properties")
                .Build();

            var myApp = await client.GetApplications()
                .Where(x => x.Name == "My Application")
                .SingleAsync();

            var joe = await myApp.CreateAccountAsync("Joe", "Stormtrooper",
                                                     "tk421@galacticempire.co",
                                                     "Changeme123!",
                                                     new { isAtPost = false });
            Console.WriteLine("User " + joe.FullName + " created");

            var usernameOrEmail = "tk421@galacticempire.co";
            var password = "Changeme123!";

            try
            {
                var loginResult = await myApp.AuthenticateAccountAsync(usernameOrEmail, password);
                var loggedInAccount = await loginResult.GetAccountAsync();
                var accountCustomData = await loggedInAccount.GetCustomDataAsync();
                Console.WriteLine("User {0} logged in. Is at post: {1}",
                                  loggedInAccount.FullName,
                                  accountCustomData["isAtPost"]);
            }
            catch (ResourceException rex)
            {
                Console.WriteLine("Could not log in. Error: " + rex.Message);
            }

            try
            {
                await joe.DeleteAsync();
            }
            catch (ResourceException rex)
            {
                Console.WriteLine("Unexpected error when deleting " + joe.Email + ". Error: " + rex.Message);
            }
            Console.WriteLine("Done!");

            Console.ReadKey(false);
        }
    }
}
