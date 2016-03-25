// <copyright file="IPasswordStrengthPolicy.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// Represents the password strength policy for a <see cref="IDirectory">Directory</see>.
    /// </summary>
    public interface IPasswordStrengthPolicy : IResource, ISaveable<IPasswordStrengthPolicy>
    {
        /// <summary>
        /// Gets the minimum quantity of symbols required by this policy.
        /// </summary>
        /// <remarks>
        /// The supported symbols are: space, !, ", #, $, %, &, ', (, ), *, +, comma, -, ., /, :, ;, <, =, >, @, [, \, ], ^, _, {, |, }, ~, ¡, §, ©, «, ¬, ® , ±, µ, ¶, ·, », ½, ¿, ×, ÷
        /// </remarks>
        /// <value>
        /// The minimum quantity of symbols required by this policy.
        /// </value>
        int MinimumSymbols { get; }

        /// <summary>
        /// Gets the minimum quantity of diacritic characters required by this policy.
        /// </summary>
        /// <remarks>
        /// The supported diacritic characters are: À, Á, Â, Ã, Ä, Å, Æ, Ç, È, É, Ê, Ë, Ì, Í, Î, Ï, Ð, Ñ, Ò, Ó, Ô, Õ, Ö, Ø, Ù, Ú, Û, Ü, Ý, Þ, ß, à, á, â, ã, ä, å, æ, ç, è, é, ê, ë, ì, í, î, ï, ð, ñ, ò, ó, ô, õ, ö, ø, ù, ú, û, ü, ý, þ, ÿ
        /// </remarks>
        /// <value>
        /// The minimum quantity of diacritic characters required by this policy.
        /// </value>
        int MinimumDiacritic { get; }

        /// <summary>
        /// Gets the minimum quantity of uppercase characters required by this policy.
        /// </summary>
        /// <value>
        /// The minimum quantity of uppercase characters required by this policy.
        /// </value>
        int MinimumUppercase { get; }

        /// <summary>
        /// Gets the minimum quantity of lowercase characters required by this policy.
        /// </summary>
        /// <value>
        /// The minimum quantity of lowercase characters required by this policy.
        /// </value>
        int MinimumLowercase { get; }

        /// <summary>
        /// Gets the minimum quantity of numeric characters required by this policy.
        /// </summary>
        /// <value>
        /// The minimum quantity of numeric characters required by this policy.
        /// </value>
        int MinimumNumeric { get; }

        /// <summary>
        /// Gets the minimum quantity of total characters required by this policy.
        /// </summary>
        /// <value>
        /// The minimum quantity of total characters required by this policy.
        /// </value>
        int MinimumLength { get; }

        /// <summary>
        /// Gets the maximum quantity of total characters required by this policy.
        /// </summary>
        /// <remarks>
        /// The maximum allowed password length is 1024 characters.
        /// </remarks>
        /// <value>
        /// The maximum quantity of total characters required by this policy.
        /// </value>
        int MaximumLength { get; }

        /// <summary>
        /// Sets the minimum quantity of symbols required by this policy.
        /// </summary>
        /// <remarks>
        /// The supported symbols are: space, !, ", #, $, %, &, ', (, ), *, +, comma, -, ., /, :, ;, <, =, >, @, [, \, ], ^, _, {, |, }, ~, ¡, §, ©, «, ¬, ® , ±, µ, ¶, ·, », ½, ¿, ×, ÷
        /// </remarks>
        /// <param name="minimumSymbols">The minimum quantity of symbols required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumSymbols(int minimumSymbols);

        /// <summary>
        /// Sets the minimum quantity of diacritic characters required by this policy.
        /// </summary>
        /// <remarks>
        /// The supported diacritic characters are: À, Á, Â, Ã, Ä, Å, Æ, Ç, È, É, Ê, Ë, Ì, Í, Î, Ï, Ð, Ñ, Ò, Ó, Ô, Õ, Ö, Ø, Ù, Ú, Û, Ü, Ý, Þ, ß, à, á, â, ã, ä, å, æ, ç, è, é, ê, ë, ì, í, î, ï, ð, ñ, ò, ó, ô, õ, ö, ø, ù, ú, û, ü, ý, þ, ÿ
        /// </remarks>
        /// <param name="minimumDiacritics">The minimum quantity of diacritic characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumDiacritic(int minimumDiacritics);

        /// <summary>
        /// Sets the minimum quantity of uppercase characters required by this policy.
        /// </summary>
        /// <param name="minimumUppercase">The minimum quantity of uppercase characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumUppercase(int minimumUppercase);

        /// <summary>
        /// Sets the minimum quantity of lowercase characters required by this policy.
        /// </summary>
        /// <param name="minimumLowercase">The minimum quantity of lowercase characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumLowercase(int minimumLowercase);

        /// <summary>
        /// Sets the minimum quantity of numeric characters required by this policy.
        /// </summary>
        /// <param name="minimumNumeric">The minimum quantity of numeric characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumNumeric(int minimumNumeric);

        /// <summary>
        /// Sets the minimum quantity of total characters required by this policy.
        /// </summary>
        /// <param name="minimumCharacters">The minimum quantity of characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMinimumLength(int minimumCharacters);

        /// <summary>
        /// Sets the maximum quantity of total characters required by this policy. The maximum allowed password length is 1024 characters.
        /// </summary>
        /// <param name="maximumCharacters">The maximum quantity of characters required.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordStrengthPolicy SetMaximumLength(int maximumCharacters);
    }
}
