using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions
{
    internal class TakeExpression : ParsedExpression
    {
        public TakeExpression(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }

        protected internal override Expression Accept(CompilingExpressionVisitor visitor)
        {
            return visitor.VisitTake(this);
        }
    }
}
