using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators
{
    internal class AnyResultOperator : ResultOperatorExpression
    {
        public AnyResultOperator()
        {
            this.ResultType = ResultOperator.Any;
        }
    }
}
