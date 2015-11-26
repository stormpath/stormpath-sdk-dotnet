// <copyright file="RandomPassword.cs" company="Stormpath, Inc.">
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
using System.Text;

namespace Stormpath.SDK.Tests.Common.RandomData
{
    public class RandomPassword
    {
        private static readonly string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string Symbols = "!@#$%&|?";
        private static readonly string Numbers = "0123456789";

        private static readonly int MinimumUpper = 1;
        private static readonly int MinimumLower = 1;
        private static readonly int MinimumSymbols = 1;
        private static readonly int MinimumNumbers = 1;

        private readonly int minimumLength;
        private readonly string generated;
        private readonly Random rnd;

        public RandomPassword(int minimumLength)
        {
            this.minimumLength = minimumLength;
            this.rnd = new Random(Environment.TickCount);

            this.generated = this.Generate();
        }

        public static implicit operator string(RandomPassword obj)
        {
            return obj.ToString();
        }

        public override string ToString()
        {
            return this.generated;
        }

        private string Generate()
        {
            var generated = new StringBuilder();

            bool meetsRequirements = false;
            bool enoughUppercase = false;
            bool enoughLowercase = false;
            bool enoughNumbers = false;
            bool enoughSymbols = false;

            while (!meetsRequirements)
            {
                char next = char.MinValue;
                var typeSuggestion = this.rnd.Next(4);

                if (typeSuggestion == 0 && !enoughUppercase)
                {
                    next = this.GetRandomChar(Uppercase);
                }
                else if (typeSuggestion == 1 && !enoughLowercase)
                {
                    next = this.GetRandomChar(Lowercase);
                }
                else if (typeSuggestion == 2 && !enoughSymbols)
                {
                    next = this.GetRandomChar(Symbols);
                }
                else if (typeSuggestion == 3 && !enoughNumbers)
                {
                    next = this.GetRandomChar(Numbers);
                }
                else
                {
                    next = this.GetRandomChar(Uppercase + Lowercase + Numbers + Symbols);
                }

                generated.Append(next);

                var test = generated.ToString();
                if (!enoughUppercase)
                    enoughUppercase = test.Count(c => Uppercase.Contains(c)) >= MinimumUpper;
                if (!enoughLowercase)
                    enoughLowercase = test.Count(c => Lowercase.Contains(c)) >= MinimumLower;
                if (!enoughSymbols)
                    enoughSymbols = test.Count(c => Symbols.Contains(c)) >= MinimumSymbols;
                if (!enoughNumbers)
                    enoughNumbers = test.Count(c => Numbers.Contains(c)) >= MinimumNumbers;
                meetsRequirements =
                    enoughUppercase &&
                    enoughLowercase &&
                    enoughSymbols &&
                    enoughNumbers &&
                    test.Length >= this.minimumLength;
            }

            return generated.ToString();
        }

        private char GetRandomChar(string source)
        {
            var charSelector = this.rnd.Next(source.Length);
            return source[charSelector];
        }
    }
}
