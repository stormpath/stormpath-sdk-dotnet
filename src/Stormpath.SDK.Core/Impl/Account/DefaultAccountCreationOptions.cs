// <copyright file="DefaultAccountCreationOptions.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.Shared.Extensions;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultAccountCreationOptions : IAccountCreationOptions
    {
        private readonly bool? registrationWorkflowEnabled;
        private readonly IRetrievalOptions<IAccount> responseOptions;

        public DefaultAccountCreationOptions(bool? registrationWorkflowEnabled, IRetrievalOptions<IAccount> responseOptions)
        {
            this.registrationWorkflowEnabled = registrationWorkflowEnabled;
            this.responseOptions = responseOptions;
        }

        bool? IAccountCreationOptions.RegistrationWorkflowEnabled => this.registrationWorkflowEnabled;

        public string GetQueryString()
        {
            if (this.registrationWorkflowEnabled == null &&
                this.responseOptions == null)
            {
                return string.Empty;
            }

            var arguments = new List<string>(2);

            if (this.registrationWorkflowEnabled != null)
            {
                arguments.Add("registrationWorkflowEnabled=" + (this.registrationWorkflowEnabled.Value ? "true" : "false"));
            }

            if (this.responseOptions != null)
            {
                arguments.Add(this.responseOptions.ToString());
            }

            return arguments
                .Where(x => !string.IsNullOrEmpty(x))
                .Join("&");
        }
    }
}
