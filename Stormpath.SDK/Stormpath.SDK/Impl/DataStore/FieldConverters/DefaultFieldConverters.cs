// <copyright file="DefaultFieldConverters.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Newtonsoft.Json.Linq;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.DataStore.FieldConverters
{
    internal static class DefaultFieldConverters
    {
        public static readonly FieldConverter LinkPropertyConverter =
            new FieldConverter(
                name: nameof(LinkPropertyConverter),
                appliesToTokenType: JTokenType.Object,
                convertAction: token =>
            {
                var firstChild = token?.Value?.First as JProperty;

                bool isLinkProperty = token?.Value?.Children().Count() == 1
                    && !string.IsNullOrEmpty(firstChild?.Value?.ToString())
                    && string.Equals(firstChild?.Name, "href", StringComparison.InvariantCultureIgnoreCase);

                if (!isLinkProperty)
                    return null;

                return new ConverterResult(true, new LinkProperty(firstChild.Value.ToString()));
            });

        public static readonly FieldConverter DateTimeOffsetConverter =
            new FieldConverter(
                name: nameof(DateTimeOffsetConverter),
                appliesToTokenType: JTokenType.Date,
                convertAction: token =>
            {
                if (string.IsNullOrEmpty(token?.Value?.ToString()))
                    return ConverterResult.Failed;

                return new ConverterResult(true, token.Value.ToObject<DateTimeOffset>());
            });

        public static readonly FieldConverter AccountStatusConverter =
            new FieldConverter(
                name: nameof(AccountStatusConverter),
                appliesToTokenType: JTokenType.String,
                appliesToTargetType: typeof(SDK.Account.IAccount),
                convertAction: token =>
                {
                    var value = token?.Value?.ToString();

                    if (!string.Equals(token?.Name, "status", StringComparison.InvariantCultureIgnoreCase)
                        || string.IsNullOrEmpty(value))
                        return ConverterResult.Failed;

                    return new ConverterResult(true, SDK.Account.AccountStatus.Parse(value));
                });

        public static readonly FieldConverter ApplicationStatusConverter =
            new FieldConverter(
                name: nameof(ApplicationStatusConverter),
                appliesToTokenType: JTokenType.String,
                appliesToTargetType: typeof(SDK.Application.IApplication),
                convertAction: token =>
                {
                    var value = token?.Value?.ToString();

                    if (!string.Equals(token?.Name, "status", StringComparison.InvariantCultureIgnoreCase)
                        || string.IsNullOrEmpty(value))
                        return ConverterResult.Failed;

                    return new ConverterResult(true, SDK.Application.ApplicationStatus.Parse(value));
                });

        public static readonly FieldConverter DirectoryStatusConverter =
            new FieldConverter(
                name: nameof(DirectoryStatusConverter),
                appliesToTokenType: JTokenType.String,
                appliesToTargetType: typeof(SDK.Directory.IDirectory),
                convertAction: token =>
                {
                    var value = token?.Value?.ToString();

                    if (!string.Equals(token?.Name, "status", StringComparison.InvariantCultureIgnoreCase)
                        || string.IsNullOrEmpty(value))
                        return ConverterResult.Failed;

                    return new ConverterResult(true, SDK.Directory.DirectoryStatus.Parse(value));
                });

        public static readonly FieldConverter GroupStatusConverter =
            new FieldConverter(
                name: nameof(GroupStatusConverter),
                appliesToTokenType: JTokenType.String,
                appliesToTargetType: typeof(SDK.Group.IGroup),
                convertAction: token =>
                {
                    var value = token?.Value?.ToString();

                    if (!string.Equals(token?.Name, "status", StringComparison.InvariantCultureIgnoreCase)
                        || string.IsNullOrEmpty(value))
                        return ConverterResult.Failed;

                    return new ConverterResult(true, SDK.Group.GroupStatus.Parse(value));
                });

        public static readonly FieldConverter StringConverter =
            new FieldConverter(
                name: nameof(StringConverter),
                appliesToTokenType: JTokenType.String,
                convertAction: token =>
                {
                    return new ConverterResult(true, token?.Value?.ToString());
                });

        public static readonly FieldConverter IntConverter =
            new FieldConverter(
                name: nameof(IntConverter),
                appliesToTokenType: JTokenType.Integer,
                convertAction: token =>
                {
                    int result;
                    if (int.TryParse(token?.Value?.ToString(), out result))
                        return new ConverterResult(true, result);
                    else
                        return ConverterResult.Failed;
                });

        public static readonly FieldConverter BoolConverter =
            new FieldConverter(
                name: nameof(BoolConverter),
                appliesToTokenType: JTokenType.Boolean,
                convertAction: token =>
                {
                    bool result;
                    if (bool.TryParse(token?.Value?.ToString(), out result))
                        return new ConverterResult(true, result);
                    else
                        return ConverterResult.Failed;
                });

        public static readonly FieldConverter NullConverter =
            new FieldConverter(
                name: nameof(NullConverter),
                appliesToTokenType: JTokenType.Null,
                convertAction: unused_ => new ConverterResult(true, null));

        public static readonly FieldConverter FallbackConverter =
            new FieldConverter(
                name: nameof(FallbackConverter),
                convertAction: token =>
            {
                return new ConverterResult(true, token?.Value?.ToString());
            });
    }
}
