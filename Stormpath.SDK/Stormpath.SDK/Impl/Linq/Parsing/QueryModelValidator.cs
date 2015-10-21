// <copyright file="QueryModelValidator.cs" company="Stormpath, Inc.">
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
using System.Linq;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal class QueryModelValidator
    {
        private readonly CollectionResourceQueryModel queryModel;

        public QueryModelValidator(CollectionResourceQueryModel queryModel)
        {
            this.queryModel = queryModel;
        }

        public void Validate()
        {
            this.ValidateLimit();
            this.ValidateOffset();
            this.ValidateWheres();
            this.ValidateDatetimeWheres();
        }

        private void ValidateLimit()
        {
            if (!this.queryModel.Limit.HasValue)
                return;

            if (this.queryModel.Limit.Value < 0)
                throw new ArgumentOutOfRangeException("Take must be greater than zero.");
        }

        private void ValidateOffset()
        {
            if (!this.queryModel.Offset.HasValue)
                return;

            if (this.queryModel.Offset.Value < 0)
                throw new ArgumentOutOfRangeException("Skip must be greater than zero.");
        }

        private void ValidateWheres()
        {
            if (!this.queryModel.WhereTerms.Any())
                return;

            bool termsUseValidComparisonOperators = this.queryModel.WhereTerms
                .Where(x => x.Type != typeof(DateTimeOffset))
                .All(x => x.Comparison == WhereComparison.Contains ||
                        x.Comparison == WhereComparison.StartsWith ||
                        x.Comparison == WhereComparison.EndsWith ||
                        x.Comparison == WhereComparison.Equal);

            if (!termsUseValidComparisonOperators)
                throw new NotSupportedException($"One or more Where terms use unsupported comparison operators.");
        }

        private void ValidateDatetimeWheres()
        {
            if (!this.queryModel.WhereTerms.Any())
                return;

            var datetimeTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DateTimeOffset))
                .ToList();

            var shorthandTerms = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DatetimeShorthandModel))
                .ToList();

            bool datetimeTermsUseValidComparisonOperators = datetimeTerms
                .All(x => x.Comparison == WhereComparison.GreaterThan ||
                        x.Comparison == WhereComparison.GreaterThanOrEqual ||
                        x.Comparison == WhereComparison.LessThan ||
                        x.Comparison == WhereComparison.LessThanOrEqual);

            if (!datetimeTermsUseValidComparisonOperators)
                throw new NotSupportedException($"One or more Where terms use unsupported comparison operators.");

            var shorthandAndDatetimeTermsCollisions = shorthandTerms
                .Where(x => datetimeTerms.Any(y => y.FieldName == x.FieldName));

            if (shorthandAndDatetimeTermsCollisions.Any())
                throw new NotSupportedException($"Multiple date constraints on field {shorthandAndDatetimeTermsCollisions.First().FieldName} are not supported");
        }
    }
}
