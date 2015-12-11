// <copyright file="SourceLanguage.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Reflection;

namespace Stormpath.SDK.Impl.Introspection
{
    internal sealed class SourceLanguage : ISourceLanguage
    {
        private static readonly SourceLanguage CSharp = new SourceLanguage("CSharp");
        private static readonly SourceLanguage Vb = new SourceLanguage("VB");

        private readonly string language;

        private SourceLanguage(string language)
        {
            this.language = language;
        }

        public override string ToString()
            => this.language;

        public static SourceLanguage Analyze(Assembly assembly)
        {
            var referencedAssemblies = assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name)
                .ToList();

            var types = assembly
                .GetTypes()
                .ToList();

            bool referenceToMSVB = referencedAssemblies.Contains("Microsoft.VisualBasic");
            bool areMyTypesPresent = types.Select(x => x.FullName).Where(x => x.Contains(".My.My")).Any();
            bool generatedVbNames = types.Select(x => x.Name).Where(x => x.StartsWith("VB$")).Any();

            bool referenceToMSCS = referencedAssemblies.Contains("Microsoft.CSharp");
            bool generatedCsNames = types.Select(x => x.Name).Where(x => x.StartsWith("<>")).Any();

            var evidenceForVb = new bool[] { referenceToMSVB, areMyTypesPresent, generatedVbNames };
            var evidenceForCsharp = new bool[] { true, referenceToMSCS, generatedCsNames };

            var scoreForVb = evidenceForVb.Count(x => x) - evidenceForCsharp.Count(x => x);

            return scoreForVb > 0
                ? Vb
                : CSharp;
        }
    }
}
