// <copyright file="DurationVisitor.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Text;

namespace Stormpath.SDK.Impl.Utility
{
    internal sealed partial class DurationVisitor
    {
        private readonly char[] tokens;

        private List<char> currentDigits = new List<char>();
        private bool inTimeSection = false;
        private bool isValid = true;
        private double weeks = 0;
        private double days = 0;
        private double hours = 0;
        private double minutes = 0;
        private double seconds = 0;

        private DurationVisitor(string duration)
        {
            this.tokens = duration.ToArray();
        }

        private void Visit()
        {
            if (this.tokens.Length == 0)
            {
                this.isValid = true;
                return;
            }

            if (this.tokens.Length < 2 || this.tokens[0] != 'P')
            {
                this.isValid = false;
                return;
            }

            foreach (char token in this.tokens.Skip(1))
            {
                if (!this.isValid)
                {
                    return;
                }

                if (token == 'Y')
                {
                    this.isValid = false;
                    continue;
                }

                if (token == 'T')
                {
                    this.inTimeSection = true;
                    continue;
                }

                if (token == 'W')
                {
                    this.isValid = this.HandleDateDesignator(ref this.weeks);
                    continue;
                }

                if (token == 'D')
                {
                    this.isValid = this.HandleDateDesignator(ref this.days);
                    continue;
                }

                if (token == 'H')
                {
                    this.isValid = this.HandleTimeDesignator(ref this.hours);
                    continue;
                }

                if (token == 'M')
                {
                    this.isValid = this.HandleTimeDesignator(ref this.minutes);
                    continue;
                }

                if (token == 'S')
                {
                    this.isValid = this.HandleTimeDesignator(ref this.seconds);
                    continue;
                }

                this.currentDigits.Add(token);
            }

            this.isValid &= !this.currentDigits.Any();
        }

        private bool HandleDateDesignator(ref double target)
            => this.HandleDesignator(false, ref target);

        private bool HandleTimeDesignator(ref double target)
            => this.HandleDesignator(true, ref target);

        private bool HandleDesignator(bool timeToken, ref double target)
        {
            if (this.inTimeSection != timeToken || !this.currentDigits.Any())
            {
                return false;
            }

            double result;
            if (!double.TryParse(CharListToString(this.currentDigits), out result))
            {
                return false;
            }

            target = result;
            this.currentDigits.Clear();

            return true;
        }

        private static string CharListToString(IList<char> chars)
            => chars.Aggregate(new StringBuilder(), (builder, c) => builder.Append(c), builder => builder.ToString());
    }
}
