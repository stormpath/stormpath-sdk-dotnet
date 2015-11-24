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
            this.ValidateExpands();
        }

        private void ValidateLimit()
        {
            if (this.queryModel.Limit == null)
                return;

            if (this.queryModel.Limit.Value < 0)
                throw new ArgumentOutOfRangeException("Take must be greater than zero.");
        }

        private void ValidateOffset()
        {
            if (this.queryModel.Offset == null)
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
                throw new NotSupportedException($"Multiple date constraints on field {shorthandAndDatetimeTermsCollisions.First().FieldName} are not supported.");

            bool shorthandTermCollisions = shorthandTerms
                .GroupBy(x => x.FieldName)
                .Distinct()
                .Count() != shorthandTerms.Count;

            if (shorthandTermCollisions)
                throw new NotSupportedException("Multiple Within constrants on the same field are not supported.");
        }

        private void ValidateExpands()
        {
            if (!this.queryModel.ExpandTerms.Any())
                return;

            if (this.queryModel.ExpandTerms.Any(x => x.Limit < 0)
                || this.queryModel.ExpandTerms.Any(x => x.Offset < 0))
                throw new ArgumentOutOfRangeException("Limit and Offset parameters for link expansion must not be negative.");
        }
    }
}
