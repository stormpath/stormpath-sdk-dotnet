// <copyright file="IPasswordPolicy.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Mail;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Directory
{
    /// <summary>
    /// Represents the password policy for a <see cref="IDirectory">Directory</see>.
    /// </summary>
    public interface IPasswordPolicy : IResource, ISaveable<IPasswordPolicy>
    {
        /// <summary>
        /// Gets the time-to-live (in hours) for the reset password token.
        /// </summary>
        /// <remarks>The default value is 24 hours.</remarks>
        /// <value>The time-to-live for the reset password token.</value>
        int ResetTokenTtl { get; }

        /// <summary>
        /// Gets the Reset Password Email status.
        /// </summary>
        /// <remarks>
        /// A <see cref="EmailStatus.Disabled"/> value indicates that the Reset Password Workflow is disabled,
        /// and an email will not be sent to an account trying to reset its password.
        /// </remarks>
        /// <value>The Reset Password Email status.</value>
        EmailStatus ResetEmailStatus { get; }

        /// <summary>
        /// Gets the Reset Password Success Email status.
        /// </summary>
        /// <remarks>
        /// A <see cref="EmailStatus.Disabled"/> value indicates that the Reset Success Password Workflow is disabled,
        /// and an email will not be sent after an account resets its password.
        /// </remarks>
        /// <value>The Reset Password Success Email status.</value>
        EmailStatus ResetSuccessEmailStatus { get; }

        /// <summary>
        /// Sets the amount of time (in hours) that a reset token will remain valid. Once that time has password, the token is invalidated.
        /// </summary>
        /// <remarks>The default value is 24 hours.</remarks>
        /// <param name="hoursToLive"></param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordPolicy SetResetTokenTtl(int hoursToLive);

        /// <summary>
        /// Sets whether the Reset Password Email Workflow is enabled or disabled for the parent <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <remarks>
        /// When the status is <see cref="EmailStatus.Disabled"/>, the reset password email will not be sent to an account trying to reset
        /// its password.
        /// </remarks>
        /// <param name="resetEmailStatus">The status of the Reset Password Email Workflow.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordPolicy SetResetEmailStatus(EmailStatus resetEmailStatus);

        /// <summary>
        /// Sets whether the Reset Password Success Email Workflow is enabled or disabled for the parent <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <remarks>
        /// When the status is <see cref="EmailStatus.Disabled"/>, the success email will not be sent after an account
        /// resets its password.
        /// </remarks>
        /// <param name="resetSuccessEmailStatus">The status of the Reset Password Success Email Workflow.</param>
        /// <returns>This instance for method chaining.</returns>
        IPasswordPolicy SetResetEmailSuccessStatus(EmailStatus resetSuccessEmailStatus);

        /// <summary>
        /// Gets the <see cref="IPasswordStrengthPolicy">Password Strength Policy</see> for this <see cref="IDirectory">Directory</see>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Password Strength Policy for this Directory.</returns>
        Task<IPasswordStrengthPolicy> GetPasswordStrengthPolicyAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
