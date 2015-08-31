// <copyright file="FieldConverter.cs" company="Stormpath, Inc.">
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
using Newtonsoft.Json.Linq;

namespace Stormpath.SDK.Impl.DataStore.FieldConverters
{
    internal sealed class FieldConverter
    {
        public const Type AnyType = null;

        private readonly Func<JProperty, ConverterResult> convertAction;
        private readonly Type appliesToTargetType;
        private readonly JTokenType? appliesToTokenType;

        public FieldConverter(Func<JProperty, ConverterResult> convertAction, Type appliesToTargetType = AnyType, JTokenType? appliesToTokenType = null)
        {
            this.convertAction = convertAction;
            this.appliesToTargetType = appliesToTargetType;
            this.appliesToTokenType = appliesToTokenType;
        }

        public ConverterResult TryConvertField(JProperty token, Type targetType)
        {
            bool isSupportedTokenType = true;
            if (this.appliesToTokenType.HasValue)
                isSupportedTokenType = this.appliesToTokenType == token.Value.Type;
            if (!isSupportedTokenType)
                return ConverterResult.Failed;

            bool isSupportedTargetType = this.appliesToTargetType == AnyType
                ? true
                : (targetType == this.appliesToTargetType);
            if (!isSupportedTargetType)
                return ConverterResult.Failed;

            var result = this.convertAction(token);
            return result;
        }
    }
}
