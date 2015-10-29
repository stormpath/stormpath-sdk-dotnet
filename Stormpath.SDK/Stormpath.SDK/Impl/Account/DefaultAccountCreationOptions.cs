// <copyright file="DefaultAccountCreationOptions.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultAccountCreationOptions : IAccountCreationOptions
    {
        private readonly bool? registrationWorkflowEnabled;
        private readonly Action<IRetrievalOptions<IAccount>> responseOptionsAction;

        public DefaultAccountCreationOptions(bool? registrationWorkflowEnabled, Action<IRetrievalOptions<IAccount>> responseOptionsAction)
        {
            this.registrationWorkflowEnabled = registrationWorkflowEnabled;
            this.responseOptionsAction = responseOptionsAction;
        }

        bool? IAccountCreationOptions.RegistrationWorkflowEnabled => this.registrationWorkflowEnabled;

        public string GetQueryString()
        {
            if (this.registrationWorkflowEnabled == null &&
                this.responseOptionsAction == null)
                return string.Empty;

            var arguments = new List<string>(2);

            if (this.registrationWorkflowEnabled != null)
                arguments.Add("registrationWorkflowEnabled=" + (this.registrationWorkflowEnabled.Value ? "true" : "false"));

            if (this.responseOptionsAction != null)
            {
                var responseOptions = new DefaultRetrivalOptions<IAccount>();
                this.responseOptionsAction(responseOptions);

                arguments.Add(responseOptions.ToString());
            }

            return arguments
                .Where(x => !string.IsNullOrEmpty(x))
                .Join("&");
        }
    }
}
