// <copyright file="TestAccounts.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Account;
using Stormpath.SDK.Client;

namespace Stormpath.SDK.Tests.Common.Fakes
{
    public static class TestAccounts
    {
        private static readonly IClient TestClient =
            Clients.Builder().Build();

        public static IAccount LukeSkywalker = TestClient.Instantiate<IAccount>()
            .SetGivenName("Luke")
            .SetSurname("Skywalker");

        public static IAccount HanSolo = TestClient.Instantiate<IAccount>()
            .SetGivenName("Han")
            .SetSurname("Solo");

        public static IAccount PrincessLeia = TestClient.Instantiate<IAccount>()
            .SetGivenName("Princess Leia")
            .SetSurname("Organa");

        public static IAccount Chewbacca = TestClient.Instantiate<IAccount>()
            .SetGivenName("Chewbacca");

        public static IAccount C3PO = TestClient.Instantiate<IAccount>()
            .SetGivenName("C-3PO");

        public static IAccount R2D2 = TestClient.Instantiate<IAccount>()
            .SetGivenName("R2-D2");

        public static IAccount LandoCalrissian = TestClient.Instantiate<IAccount>()
            .SetGivenName("Lando")
            .SetSurname("Calrissian");

        public static IAccount AdmiralAckbar = TestClient.Instantiate<IAccount>()
            .SetGivenName("Admiral Gial")
            .SetSurname("Ackbar");

        public static IAccount WedgeAntilles = TestClient.Instantiate<IAccount>()
            .SetGivenName("Wedge")
            .SetSurname("Antilles");

        public static IAccount MonMothma = TestClient.Instantiate<IAccount>()
            .SetGivenName("Mon")
            .SetSurname("Mothma");

        public static List<IAccount> RebelAlliance = new List<IAccount>()
        {
            LukeSkywalker,
            HanSolo,
            PrincessLeia,
            Chewbacca,
            C3PO,
            R2D2,
            LandoCalrissian,
            AdmiralAckbar,
            WedgeAntilles,
            MonMothma
        };

        public static IAccount DarthVader = TestClient.Instantiate<IAccount>()
            .SetGivenName("Darth")
            .SetSurname("Vader");

        public static IAccount EmperorPalpatine = TestClient.Instantiate<IAccount>()
            .SetGivenName("Emperor")
            .SetSurname("Palpatine");

        public static IAccount BobaFett = TestClient.Instantiate<IAccount>()
            .SetGivenName("Boba")
            .SetSurname("Fett");

        public static IAccount GrandMoffTarkin = TestClient.Instantiate<IAccount>()
            .SetGivenName("Grand Moff")
            .SetSurname("Tarkin");

        public static IAccount JabbaTheHutt = TestClient.Instantiate<IAccount>()
            .SetGivenName("Jabba")
            .SetSurname("the Hutt");

        public static List<IAccount> GalacticEmpire = new List<IAccount>()
        {
            DarthVader,
            EmperorPalpatine,
            BobaFett,
            GrandMoffTarkin,
            JabbaTheHutt
        };
    }
}
