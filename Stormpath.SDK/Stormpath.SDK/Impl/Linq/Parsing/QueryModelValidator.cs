using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            bool datetimeTermsUseValidComparisonOperators = this.queryModel.WhereTerms
                .Where(x => x.Type == typeof(DateTimeOffset))
                .All(x => x.Comparison == WhereComparison.GreaterThan ||
                        x.Comparison == WhereComparison.GreaterThanOrEqual ||
                        x.Comparison == WhereComparison.LessThan ||
                        x.Comparison == WhereComparison.LessThanOrEqual);

            if (!termsUseValidComparisonOperators ||
                !datetimeTermsUseValidComparisonOperators)
                throw new NotSupportedException($"One or more Where terms use unsupported comparison operators.");
        }
    }
}
