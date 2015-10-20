using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions;
using Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class CompilingExpressionVisitor : ExpressionVisitor
    {
        public CollectionResourceQueryModel Model { get; private set; }
            = new CollectionResourceQueryModel();

        internal Expression VisitParsedExpression(ParsedExpression node)
        {
            throw new NotSupportedException($"{node.GetType()} is an unsupported expression.");
        }

        internal Expression VisitTake(TakeExpression node)
        {
            this.Model.Limit = node.Value;

            return node;
        }

        internal Expression VisitSkip(SkipExpression node)
        {
            this.Model.Offset = node.Value;

            return node;
        }

        internal Expression VisitResultOperator(ResultOperatorExpression node)
        {
            //todo

            return node;
        }
    }
}
