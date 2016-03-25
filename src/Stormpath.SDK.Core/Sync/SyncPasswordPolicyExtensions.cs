// <copyright file="SyncPasswordPolicyExtensions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Directory;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on a <see cref="IPasswordPolicy">Password Policy</see>.
    /// </summary>
    public static class SyncPasswordPolicyExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IPasswordStrengthPolicy">Password Strength Policy</see> for this <see cref="IDirectory"/>.
        /// </summary>
        /// <param name="passwordPolicy">The directory's <see cref="IPasswordPolicy">Password Policy</see>.</param>
        /// <returns>The Password Strength Policy for this Directory.</returns>
        public static IPasswordStrengthPolicy GetPasswordStrengthPolicy(this IPasswordPolicy passwordPolicy)
            => (passwordPolicy as IPasswordPolicySync).GetPasswordStrengthPolicy();
    }
}
