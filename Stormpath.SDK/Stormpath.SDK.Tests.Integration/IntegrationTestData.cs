// <copyright file="IntegrationTestCollection.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;

namespace Stormpath.SDK.Tests.Integration
{
    public static class IntegrationTestData
    {
        public static List<IApplication> GetTestApplications(IClient client)
        {
            return new List<IApplication>()
            {
                {
                    client.Instantiate<IApplication>()
                        .SetName($".NET ITs {DateTimeOffset.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}")
                        .SetDescription("The Battle of Endor")
                        .SetStatus(ApplicationStatus.Enabled)
                }
            };
        }

        public static List<IAccount> GetTestAccounts(IClient client)
        {
            return new List<IAccount>()
            {
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Luke")
                        .SetSurname("Skywalker")
                        .SetEmail("lskywalker@tattooine.rim")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Han")
                        .SetSurname("Solo")
                        .SetEmail("han.solo@corellia.core")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Leia")
                        .SetSurname("Organa")
                        .SetEmail("princessleia@alderaan.core")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Chewbacca")
                        .SetSurname("the Wookie")
                        .SetEmail("chewie@kashyyyk.rim")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Lando")
                        .SetSurname("Calrissian")
                        .SetEmail("lcalrissian@socorro.rim")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Gial")
                        .SetSurname("Ackbar")
                        .SetEmail("ackbar@dac.rim")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Darth")
                        .SetSurname("Vader")
                        .SetEmail("vader@empire.co")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Emporer")
                        .SetSurname("Palpatine")
                        .SetEmail("emporer@empire.co")
                },
            };
        }
    }
}
