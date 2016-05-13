// <copyright file="DefaultPasswordPolicy.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    internal sealed class DefaultPasswordStrengthPolicy :
        AbstractInstanceResource,
        IPasswordStrengthPolicy,
        IPasswordStrengthPolicySync
    {
        private readonly string MinimumSymbolPropertyName = "minSymbol";
        private readonly string MinimumDiacriticPropertyName = "minDiacritic";
        private readonly string MinimumUppercasePropertyName = "minUpperCase";
        private readonly string MinimumLengthPropertyName = "minLength";
        private readonly string MinimumLowercasePropertyName = "minLowerCase";
        private readonly string MinimumNumericPropertyName = "minNumeric";
        private readonly string MaximumLengthPropertyName = "maxLength";
        private readonly string PreventReusePropertyName = "preventReuse";

        public DefaultPasswordStrengthPolicy(ResourceData data)
            : base(data)
        {
        }

        int IPasswordStrengthPolicy.MinimumSymbols
            => this.GetIntProperty(MinimumSymbolPropertyName);

        int IPasswordStrengthPolicy.MinimumDiacritic
            => this.GetIntProperty(MinimumDiacriticPropertyName);

        int IPasswordStrengthPolicy.MinimumUppercase
            => this.GetIntProperty(MinimumUppercasePropertyName);

        int IPasswordStrengthPolicy.MinimumLowercase
            => this.GetIntProperty(MinimumLowercasePropertyName);

        int IPasswordStrengthPolicy.MinimumNumeric
            => this.GetIntProperty(MinimumNumericPropertyName);

        int IPasswordStrengthPolicy.MinimumLength
            => this.GetIntProperty(MinimumLengthPropertyName);

        int IPasswordStrengthPolicy.MaximumLength
            => this.GetIntProperty(MaximumLengthPropertyName);

        int IPasswordStrengthPolicy.PreventReuse
            => this.GetIntProperty(PreventReusePropertyName);

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumSymbols(int minimumSymbols)
        {
            if (minimumSymbols < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumSymbols), "Value must not be negative.");
            }

            this.SetProperty(MinimumSymbolPropertyName, minimumSymbols);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumDiacritic(int minimumDiacritics)
        {
            if (minimumDiacritics < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumDiacritics), "Value must not be negative.");
            }

            this.SetProperty(MinimumDiacriticPropertyName, minimumDiacritics);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumUppercase(int minimumUppercase)
        {
            if (minimumUppercase < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumUppercase), "Value must not be negative.");
            }

            this.SetProperty(MinimumUppercasePropertyName, minimumUppercase);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumLowercase(int minimumLowercase)
        {
            if (minimumLowercase < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumLowercase), "Value must not be negative.");
            }

            this.SetProperty(MinimumLowercasePropertyName, minimumLowercase);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumNumeric(int minimumNumeric)
        {
            if (minimumNumeric < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumNumeric), "Value must not be negative.");
            }

            this.SetProperty(MinimumNumericPropertyName, minimumNumeric);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMinimumLength(int minimumCharacters)
        {
            if (minimumCharacters < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumCharacters), "Value must not be negative.");
            }

            this.SetProperty(MinimumLengthPropertyName, minimumCharacters);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetMaximumLength(int maximumCharacters)
        {
            if (maximumCharacters < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCharacters), "Value must not be negative.");
            }

            this.SetProperty(MaximumLengthPropertyName, maximumCharacters);
            return this;
        }

        IPasswordStrengthPolicy IPasswordStrengthPolicy.SetPreventReuse(int historyLength)
        {
            if (historyLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(historyLength), "Value must not be negative.");
            }

            this.SetProperty(PreventReusePropertyName, historyLength);
            return this;
        }

        Task<IPasswordStrengthPolicy> ISaveable<IPasswordStrengthPolicy>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IPasswordStrengthPolicy>(cancellationToken);

        IPasswordStrengthPolicy ISaveableSync<IPasswordStrengthPolicy>.Save()
            => this.Save<IPasswordStrengthPolicy>();
    }
}
