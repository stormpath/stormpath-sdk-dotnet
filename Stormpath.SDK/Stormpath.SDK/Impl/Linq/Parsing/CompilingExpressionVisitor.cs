using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators;
using Stormpath.SDK.Impl.Linq.Parsing.Translators;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class CompilingExpressionVisitor : ExpressionVisitor
    {
        private static readonly int DefaultApiPageLimit = 100;

        private FieldNameTranslator fieldNameTranslator
            = new FieldNameTranslator();

        private DateTimeFieldNameTranslator datetimeFieldNameTranslator
            = new DateTimeFieldNameTranslator();

        public CollectionResourceQueryModel Model { get; private set; }
            = new CollectionResourceQueryModel();

        internal Expression VisitParsedExpression(ParsedExpression node)
        {
            throw new NotSupportedException($"{node.GetType()} is an unsupported expression.");
        }

        internal Expression VisitTake(TakeExpression node)
        {
            if (this.Model.Limit.HasValue)
                return node; // LIFO behavior

            var value = node.Value;
            this.Model.ExecutionPlan.MaxItems = value;
            this.Model.Limit = value;

            if (value > DefaultApiPageLimit)
                this.Model.Limit = DefaultApiPageLimit;

            return node;
        }

        internal Expression VisitSkip(SkipExpression node)
        {
            if (this.Model.Offset.HasValue)
                return node; // LIFO behavior

            this.Model.Offset = node.Value;

            return node;
        }

        internal Expression VisitOrderBy(OrderByExpression node)
        {
            string translatedFieldName = null;
            if (!this.fieldNameTranslator.TryGetValue(node.FieldName, out translatedFieldName))
                throw new NotSupportedException($"{node.FieldName} is not a supported field.");

            this.Model.OrderByTerms.Add(new OrderByTerm()
            {
                FieldName = translatedFieldName,
                Direction = node.Direction
            });

            return node;
        }

        internal Expression VisitFilter(FilterExpression node)
        {
            if (!string.IsNullOrEmpty(this.Model.FilterTerm))
                throw new NotSupportedException("Multiple Filter terms are not supported");

            this.Model.FilterTerm = node.Value;

            return node;
        }

        internal Expression VisitWhereMember(WhereMemberExpression node)
        {
            string translatedFieldName = null;
            if (!this.fieldNameTranslator.TryGetValue(node.FieldName, out translatedFieldName) &&
                !this.datetimeFieldNameTranslator.TryGetValue(node.FieldName, out translatedFieldName))
                throw new NotSupportedException($"{node.FieldName} is not a supported field.");

            this.Model.WhereTerms.Add(new WhereTerm()
            {
                FieldName = translatedFieldName,
                Comparison = node.Comparison,
                Value = node.Value,
                Type = node.Value.GetType()
            });

            return node;
        }

        internal Expression VisitResultOperator(ResultOperatorExpression node)
        {
            this.Model.ResultOperator = node.ResultType;

            if (node.ResultType == ResultOperator.First)
            {
                this.Model.ResultDefaultIfEmpty = (node as FirstResultOperator).DefaultIfEmpty;
            }
            else if (node.ResultType == ResultOperator.Single)
            {
                this.Model.ResultDefaultIfEmpty = (node as SingleResultOperator).DefaultIfEmpty;
            }

            return node;
        }
    }
}
