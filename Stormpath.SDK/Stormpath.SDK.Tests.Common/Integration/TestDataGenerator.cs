// <copyright file="TestDataGenerator.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Group;
using Stormpath.SDK.Organization;
using Stormpath.SDK.Tests.Common.RandomData;

namespace Stormpath.SDK.Tests.Common.Integration
{
    public class TestDataGenerator
    {
        public TestDataGenerator()
        {
            this.Nonce = RandomString.Create().Substring(0, 6);
        }

        public string Nonce { get; private set; }

        public List<IApplication> GetTestApplications(IClient client)
        {
            var timeString = DateTimeOffset.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

            return new List<IApplication>()
            {
                {
                    client.Instantiate<IApplication>()
                        .SetName($".NET IT (disabled) {this.Nonce} - {timeString}")
                        .SetDescription("The Battle of Yavin")
                        .SetStatus(ApplicationStatus.Disabled)
                },
                {
                    client.Instantiate<IApplication>()
                        .SetName($".NET IT (primary) {this.Nonce} - {timeString}")
                        .SetDescription("The Battle of Endor")
                        .SetStatus(ApplicationStatus.Enabled)
                },
            };
        }

        public List<IGroup> GetTestGroups(IClient client)
        {
            var timeString = DateTimeOffset.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

            return new List<IGroup>()
            {
                {
                    client.Instantiate<IGroup>()
                        .SetName($".NET IT Test Group (primary) {this.Nonce} - {timeString}")
                        .SetDescription("Humans")
                },
            };
        }

        public List<IOrganization> GetTestOrganizations(IClient client)
        {
            var timeString = DateTimeOffset.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

            return new List<IOrganization>()
            {
                {
                    client.Instantiate<IOrganization>()
                        .SetName($".NET IT Test Organization (primary) {this.Nonce} - {timeString}")
                        .SetNameKey($"dotnet-it-test-org1-{this.Nonce}")
                        .SetDescription("Star Wars")
                },
            };
        }

        public List<IAccount> GetTestAccounts(IClient client)
        {
            return new List<IAccount>()
            {
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Luke")
                        .SetSurname("Skywalker")
                        .SetEmail("lskywalker@tattooine.rim")
                        .SetPassword("whataPieceofjunk$1138")
                        .SetUsername($"sonofthesuns-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Han")
                        .SetSurname("Solo")
                        .SetEmail("han.solo@corellia.core")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"cptsolo-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Leia")
                        .SetSurname("Organa")
                        .SetEmail("leia.organa@alderaan.core")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"princessleia-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Chewbacca")
                        .SetMiddleName("the")
                        .SetSurname("Wookiee")
                        .SetEmail("chewie@kashyyyk.rim")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"rrwwwggg-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Lando")
                        .SetSurname("Calrissian")
                        .SetEmail("lcalrissian@socorro.rim")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"lottanerve-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Darth")
                        .SetSurname("Vader")
                        .SetEmail("vader@galacticempire.co")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"lordvader-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Emperor")
                        .SetSurname("Palpatine")
                        .SetEmail("emperor@galacticempire.co")
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"rulethegalaxy-{this.Nonce}")
                },
                {
                    client.Instantiate<IAccount>()
                        .SetGivenName("Wilhuff")
                        .SetSurname("Tarkin")
                        .SetEmail("grandmofftarkin@galacticempire.co")
                        .SetStatus(AccountStatus.Disabled)
                        .SetPassword(new RandomPassword(12))
                        .SetUsername($"tarkin-{this.Nonce}")
                }
            };
        }
    }
}
