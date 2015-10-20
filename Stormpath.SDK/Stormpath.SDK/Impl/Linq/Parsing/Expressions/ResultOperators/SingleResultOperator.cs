using System.Linq.Expressions;

namespace Stormpath.SDK.Impl.Linq.Parsing.Expressions.ResultOperators
{
    internal class SingleResultOperator : ResultOperatorExpression
    {
        public SingleResultOperator(bool defaultIfEmpty)
        {
            this.ResultType = ResultOperator.Single;
            this.DefaultIfEmpty = defaultIfEmpty;
        }

        public bool DefaultIfEmpty { get; private set; }
    }
}
