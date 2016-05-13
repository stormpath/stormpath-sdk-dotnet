// <copyright file="RangedDatetimeWhereTermParser.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Linq.Parsing.RangedTerms
{
    internal sealed class RangedDatetimeWhereTermParser : AbstractRangedWhereTermParser<DateTimeOffset>
    {
        protected override DateTimeOffset? Coerce(object value)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return (DateTimeOffset)value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not parse ranged datetime term '{value}'. See the inner exception for details.", ex);
            }
        }

        protected override string Format(DateTimeOffset value)
            => Iso8601.Format(value, includeMilliseconds: false);
    }
}
