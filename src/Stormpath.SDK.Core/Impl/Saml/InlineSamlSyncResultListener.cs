// <copyright file="InlineSamlSyncResultListener.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.Impl.Saml
{
    internal sealed class InlineSamlSyncResultListener : ISamlSyncResultListener
    {
        private readonly Action<ISamlAccountResult> onAuthenticated;
        private readonly Action<ISamlAccountResult> onLogout;

        public InlineSamlSyncResultListener(
            Action<ISamlAccountResult> onAuthenticated,
            Action<ISamlAccountResult> onLogout)
        {
            this.onAuthenticated = onAuthenticated;
            this.onLogout = onLogout;
        }

        void ISamlSyncResultListener.OnAuthenticated(ISamlAccountResult result)
        {
            if (this.onAuthenticated != null)
            {
                this.onAuthenticated(result);
            }
        }

        void ISamlSyncResultListener.OnLogout(ISamlAccountResult result)
        {
            if (this.onLogout != null)
            {
                this.onLogout(result);
            }
        }
    }
}
