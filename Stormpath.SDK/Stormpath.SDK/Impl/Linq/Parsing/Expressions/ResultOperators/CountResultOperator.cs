using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators
{
    internal class CountResultOperator : ResultOperatorExpression
    {
        public CountResultOperator()
        {
            this.ResultType = ResultOperator.Count;
        }
    }
}
