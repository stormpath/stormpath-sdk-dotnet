// <copyright file="FakeAccounts.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Account;

namespace Stormpath.SDK.Tests.Fakes
{
    public static class FakeAccounts
    {
        public static IAccount LukeSkywalker = new FakeAccount()
        {
            GivenName = "Luke",
            Surname = "Skywalker"
        };

        public static IAccount HanSolo = new FakeAccount()
        {
            GivenName = "Han",
            Surname = "Solo"
        };

        public static IAccount PrincessLeia = new FakeAccount()
        {
            GivenName = "Princess Leia",
            Surname = "Organa"
        };

        public static IAccount Chewbacca = new FakeAccount()
        {
            GivenName = "Chewbacca"
        };

        public static IAccount C3PO = new FakeAccount()
        {
            GivenName = "C-3PO"
        };

        public static IAccount R2D2 = new FakeAccount()
        {
            GivenName = "R2-D2"
        };

        public static IAccount LandoCalrissian = new FakeAccount()
        {
            GivenName = "Lando",
            Surname = "Calrissian"
        };

        public static IAccount AdmiralAckbar = new FakeAccount()
        {
            GivenName = "Admiral Gial",
            Surname = "Ackbar"
        };

        public static IAccount WedgeAntilles = new FakeAccount()
        {
            GivenName = "Wedge",
            Surname = "Antilles"
        };

        public static IAccount MonMothma = new FakeAccount()
        {
            GivenName = "Mon",
            Surname = "Mothma"
        };

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

        public static IAccount DarthVader = new FakeAccount()
        {
            GivenName = "Darth",
            Surname = "Vader"
        };

        public static IAccount EmperorPalpatine = new FakeAccount()
        {
            GivenName = "Emperor",
            Surname = "Palpatine"
        };

        public static IAccount BobaFett = new FakeAccount()
        {
            GivenName = "Boba",
            Surname = "Fett"
        };

        public static IAccount GrandMoffTarkin = new FakeAccount()
        {
            GivenName = "Grand Moff",
            Surname = "Tarkin"
        };

        public static IAccount JabbaTheHutt = new FakeAccount()
        {
            GivenName = "Jabba",
            Surname = "the Hutt"
        };

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
