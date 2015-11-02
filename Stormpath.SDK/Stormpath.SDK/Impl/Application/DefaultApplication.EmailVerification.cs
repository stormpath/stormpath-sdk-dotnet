// <copyright file="DefaultApplication.EmailVerification.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        Task IApplication.SendVerificationEmailAsync(string usernameOrEmail, CancellationToken cancellationToken)
            => this.AsInterface.SendVerificationEmailAsync(request => request.Login = usernameOrEmail, cancellationToken);

        Task IApplication.SendVerificationEmailAsync(Action<EmailVerificationRequestBuilder> requestBuilderAction, CancellationToken cancellationToken)
        {
            var builder = new EmailVerificationRequestBuilder(this.GetInternalAsyncDataStore());
            requestBuilderAction(builder);

            if (string.IsNullOrEmpty(builder.Login))
                throw new ArgumentNullException(nameof(builder.Login));

            var href = $"{(this as IResource).Href}/verificationEmails";

            return this.GetInternalAsyncDataStore().CreateAsync(href, builder.Build(), cancellationToken);
        }
    }
}
