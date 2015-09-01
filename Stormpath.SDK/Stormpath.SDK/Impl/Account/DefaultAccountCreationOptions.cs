// <copyright file="DefaultAccountCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultAccountCreationOptions : IAccountCreationOptions, ICreationOptions
    {
        private readonly bool? registrationWorkflowEnabled;

        public DefaultAccountCreationOptions(bool? registrationWorkflowEnabled)
        {
            this.registrationWorkflowEnabled = registrationWorkflowEnabled;
        }

        bool? IAccountCreationOptions.RegistrationWorkflowEnabled => this.registrationWorkflowEnabled;

        public string GetQueryString()
        {
            if (!this.registrationWorkflowEnabled.HasValue)
                return string.Empty;

            return "registrationWorkflowEnabled=" +
                (this.registrationWorkflowEnabled.Value ? "true" : "false");
        }
    }
}
