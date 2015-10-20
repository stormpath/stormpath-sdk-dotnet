using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions
{
    internal abstract class ParsedExpression : Expression
    {
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            var parsedVisitor = visitor as CompilingExpressionVisitor;
            if (parsedVisitor != null)
                return this.Accept(parsedVisitor);

            return base.Accept(visitor);
        }

        protected internal virtual Expression Accept(CompilingExpressionVisitor visitor)
        {
            return visitor.VisitParsedExpression(this);
        }
    }
}
