// <copyright file="DefaultApplication.EmailVerificationSync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        void IApplicationSync.SendVerificationEmail(string usernameOrEmail)
            => this.SendVerificationEmail(request => request.Login = usernameOrEmail);

        void IApplicationSync.SendVerificationEmail(Action<EmailVerificationRequestBuilder> requestBuilderAction)
        {
            var builder = new EmailVerificationRequestBuilder(this.GetInternalAsyncDataStore());
            requestBuilderAction(builder);

            if (string.IsNullOrEmpty(builder.Login))
            {
                throw new ArgumentNullException(nameof(builder.Login));
            }

            var href = $"{(this as IResource).Href}/verificationEmails";

            this.GetInternalSyncDataStore().Create(href, builder.Build());
        }
    }
}
