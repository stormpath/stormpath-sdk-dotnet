// <copyright file="FuncFieldConverter.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Serialization.FieldConverters
{
    internal sealed class FuncFieldConverter : AbstractFieldConverter
    {
        private readonly Func<KeyValuePair<string, object>, FieldConverterResult> convertAction;

        public FuncFieldConverter(Func<KeyValuePair<string, object>, FieldConverterResult> convertAction, string converterName, Type appliesToTargetType = AnyType)
            : base(converterName, appliesToTargetType)
        {
            this.convertAction = convertAction;
        }

        public FuncFieldConverter(Func<KeyValuePair<string, object>, FieldConverterResult> convertAction, string converterName, params Type[] appliesToTargetTypes)
            : base(converterName, appliesToTargetTypes)
        {
            this.convertAction = convertAction;
        }

        protected override FieldConverterResult ConvertImpl(KeyValuePair<string, object> token, Func<IDictionary<string, object>, Type, IDictionary<string, object>> recursiveConverter)
        {
            return this.convertAction(token);
        }
    }
}
