// <copyright file="DefaultEmailVerificationToken.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultEmailVerificationToken : AbstractInstanceResource, IEmailVerificationToken
    {
        private static readonly string TokenDelimiter = "/emailVerificationTokens/";

        public DefaultEmailVerificationToken(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        private IEmailVerificationToken AsInterface => this;

        string IEmailVerificationToken.GetValue()
        {
            var thisHref = this.AsInterface.Href;

            if (string.IsNullOrEmpty(thisHref))
                return null;

            // Return everything after /emailVerificationTokens/
            return thisHref.Substring(thisHref.IndexOf(TokenDelimiter) + TokenDelimiter.Length);
        }

        Task<IEmailVerificationToken> ISaveable<IEmailVerificationToken>.SaveAsync(CancellationToken cancellationToken)
            => this.SaveAsync<IEmailVerificationToken>(cancellationToken);

        public override string ToString() => this.AsInterface.GetValue();
    }
}
