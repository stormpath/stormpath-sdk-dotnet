// <copyright file="ConverterResult.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.DataStore.FieldConverters
{
    internal sealed class ConverterResult : ImmutableValueObject<ConverterResult>
    {
        private readonly bool success = false;
        private readonly object result = null;
        private readonly Type type = null;

        public static readonly ConverterResult Failed = new ConverterResult(false);

        private ConverterResult(bool success)
        {
            if (success == true)
                throw new ApplicationException("Use this constructor only for failed results. For successful results, use ConverterResult(success: true, result: object)");

            this.success = success;
        }

        public ConverterResult(bool success, object result)
        {
            this.success = success;
            this.result = result;
        }

        public ConverterResult(bool success, object result, Type type)
            : this(success, result)
        {
            this.type = type;
        }

        public bool Success => this.success;

        public object Result => this.result;

        public Type Type => this.type;

        public T ResultAs<T>()
            where T : class
        {
            if (typeof(T) != this.type)
                throw new InvalidCastException($"Result is of type '{this.type?.Name}', not '{typeof(T).Name}'");

            return this.result as T;
        }
    }
}
