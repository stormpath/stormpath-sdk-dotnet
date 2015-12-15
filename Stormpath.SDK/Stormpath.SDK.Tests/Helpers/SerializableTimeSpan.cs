// <copyright file="SerializableTimeSpan.cs" company="Stormpath, Inc.">
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
using Xunit.Abstractions;

namespace Stormpath.SDK.Tests.Helpers
{
    /// <summary>
    /// Represents a <see cref="TimeSpan"/> that xUnit can serialize.
    /// </summary>
    public class SerializableTimeSpan : IXunitSerializable
    {
        private TimeSpan? timeSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSpan"/> class.
        /// </summary>
        public SerializableTimeSpan()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSpan"/> class with
        /// the specified <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public SerializableTimeSpan(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        /// <summary>
        /// Implicit cast to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="instance">The <see cref="SerializableTimeSpan"/> to cast.</param>
        public static implicit operator TimeSpan(SerializableTimeSpan instance)
            => instance.timeSpan.Value;

        /// <inheritdoc/>
        public void Deserialize(IXunitSerializationInfo info)
        {
            this.timeSpan = TimeSpan.FromTicks(info.GetValue<long>("ticks"));
        }

        /// <inheritdoc/>
        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("ticks", this.timeSpan.Value.Ticks);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.timeSpan == null
                ? "null"
                : this.timeSpan.Value.ToString();
        }
    }
}
