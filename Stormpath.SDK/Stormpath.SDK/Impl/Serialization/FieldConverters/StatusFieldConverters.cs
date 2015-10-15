// <copyright file="StatusFieldConverters.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Serialization.FieldConverters
{
    internal static class StatusFieldConverters
    {
        private static bool IsStatusField(KeyValuePair<string, object> token)
        {
            return string.Equals(token.Key, "status", StringComparison.InvariantCultureIgnoreCase)
                    && !string.IsNullOrEmpty(token.Value.ToString());
        }

        internal sealed class AccountStatusConverter : AbstractFieldConverter
        {
            public AccountStatusConverter()
                : base(nameof(AccountStatusConverter), typeof(SDK.Account.IAccount), typeof(SDK.Provider.IProviderAccountResult))
            {
            }

            protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token)
            {
                if (!IsStatusField(token))
                    return FieldConverterResult.Failed;

                return new FieldConverterResult(true, SDK.Account.AccountStatus.Parse(token.Value.ToString()));
            }
        }

        internal sealed class ApplicationStatusConverter : AbstractFieldConverter
        {
            public ApplicationStatusConverter()
                : base(nameof(ApplicationStatusConverter), appliesToTargetType: typeof(SDK.Application.IApplication))
            {
            }

            protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token)
            {
                if (!IsStatusField(token))
                    return FieldConverterResult.Failed;

                return new FieldConverterResult(true, SDK.Application.ApplicationStatus.Parse(token.Value.ToString()));
            }
        }

        internal sealed class DirectoryStatusConverter : AbstractFieldConverter
        {
            public DirectoryStatusConverter()
                : base(nameof(DirectoryStatusConverter), appliesToTargetType: typeof(SDK.Directory.IDirectory))
            {
            }

            protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token)
            {
                if (!IsStatusField(token))
                    return FieldConverterResult.Failed;

                return new FieldConverterResult(true, SDK.Directory.DirectoryStatus.Parse(token.Value.ToString()));
            }
        }

        internal sealed class GroupStatusConverter : AbstractFieldConverter
        {
            public GroupStatusConverter()
                : base(nameof(GroupStatusConverter), appliesToTargetType: typeof(SDK.Group.IGroup))
            {
            }

            protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token)
            {
                if (!IsStatusField(token))
                    return FieldConverterResult.Failed;

                return new FieldConverterResult(true, SDK.Group.GroupStatus.Parse(token.Value.ToString()));
            }
        }
    }
}
