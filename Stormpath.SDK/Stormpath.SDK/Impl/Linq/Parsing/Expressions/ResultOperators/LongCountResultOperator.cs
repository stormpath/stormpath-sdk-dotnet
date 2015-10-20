using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators
{
    internal class LongCountResultOperator : ResultOperatorExpression
    {
        public LongCountResultOperator()
        {
            this.ResultType = ResultOperator.LongCount;
        }
    }
}
