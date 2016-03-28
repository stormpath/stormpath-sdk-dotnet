// <copyright file="IAccountCreationPolicy.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Mail;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// Represents the Account Creation Policy for a <see cref="IDirectory">Directory</see>.
    /// </summary>
    public interface IAccountCreationPolicy : IResource, ISaveable<IAccountCreationPolicy>
    {
        /// <summary>
        /// Gets the Account Verification Email status.
        /// </summary>
        /// <remarks>
        /// A <see cref="EmailStatus.Disabled"/> value indicates that the Account Verification Workflow is disabled,
        /// and the account verification email will not be sent to newly-created accounts.
        /// </remarks>
        /// <value>The Account Verification Email status.</value>
        EmailStatus VerificationEmailStatus { get; }

        /// <summary>
        /// Gets the Account Verification Success Email status.
        /// </summary>
        /// <remarks>
        /// A <see cref="EmailStatus.Disabled"/> value indicates that the Account Verification Success Workflow is disabled,
        /// and the account verification success email will not be sent when the email for newly-created accounts is verified.
        /// </remarks>
        /// <value>The Account Verification Success Email status.</value>
        EmailStatus VerificationSuccessEmailStatus { get; }

        /// <summary>
        /// Gets the Welcome Email status.
        /// </summary>
        /// <remarks>
        /// A <see cref="EmailStatus.Disabled"/> value indicates that the Welcome Workflow is disabled,
        /// and a welcome email will not be sent when a new account is created.
        /// </remarks>
        /// <value>The Welcome Email status.</value>
        EmailStatus WelcomeEmailStatus { get; }

        /// <summary>
        /// Sets whether the Account Verification Workflow is enabled or disabled for the parent <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <remarks>
        /// When the status is <see cref="EmailStatus.Disabled"/>, the account verification email will not be sent to newly-created accounts.
        /// </remarks>
        /// <param name="verificationEmailStatus">The status of the Account Verification Workflow.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountCreationPolicy SetVerificationEmailStatus(EmailStatus verificationEmailStatus);

        /// <summary>
        /// Sets whether the Account Verification Success Workflow is enabled or disabled for the parent <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <remarks>
        /// When the status is <see cref="EmailStatus.Disabled"/>, the account verification success email will not be sent when the email for a newly-created account is verified.
        /// </remarks>
        /// <param name="verificationSuccessEmailStatus">The status of the Account Verification Success Workflow.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountCreationPolicy SetVerificationSuccessEmailStatus(EmailStatus verificationSuccessEmailStatus);

        /// <summary>
        /// Sets whether the Welcome Email Workflow is enabled or disabled for the parent <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <remarks>
        /// When the status is <see cref="EmailStatus.Disabled"/>, the welcome email will not be sent for newly-created accounts.
        /// </remarks>
        /// <param name="welcomeEmailStatus">The status of the Welcome Email Workflow.</param>
        /// <returns>This instance for method chaining.</returns>
        IAccountCreationPolicy SetWelcomeEmailStatus(EmailStatus welcomeEmailStatus);
    }
}
