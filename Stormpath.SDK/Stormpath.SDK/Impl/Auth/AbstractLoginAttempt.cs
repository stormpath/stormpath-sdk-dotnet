// <copyright file="AbstractLoginAttempt.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Auth
{
    internal abstract class AbstractLoginAttempt : AbstractResource, ILoginAttempt
    {
        private static readonly string TypePropertyName = "type";
        private static readonly string AccountStorePropertyName = "accountStore";

        public AbstractLoginAttempt(ResourceData data)
            : base(data)
        {
        }

        string ILoginAttempt.Type => this.GetProperty<string>(TypePropertyName);

        IEmbeddedProperty ILoginAttempt.AccountStore => this.GetLinkProperty(AccountStorePropertyName);

        void ILoginAttempt.SetType(string type) => this.SetProperty(TypePropertyName, type);

        void ILoginAttempt.SetAccountStore(IAccountStore accountStore)
        {
            if (string.IsNullOrEmpty(accountStore?.Href))
                throw new ArgumentNullException(nameof(accountStore.Href));

            this.SetLinkProperty(AccountStorePropertyName, accountStore.Href);
        }

        void ILoginAttempt.SetAccountStore(string hrefOrNameKey)
        {
            if (string.IsNullOrEmpty(hrefOrNameKey))
                throw new ArgumentNullException(nameof(hrefOrNameKey));

            bool looksLikeHref = hrefOrNameKey.Split('/').Length > 4;
            if (looksLikeHref)
            {
                this.SetLinkProperty(AccountStorePropertyName, hrefOrNameKey);
            }
            else
            {
                this.SetProperty(
                AccountStorePropertyName,
                new Dictionary<string, object>() { ["nameKey"] = hrefOrNameKey });
            }
        }
    }
}
