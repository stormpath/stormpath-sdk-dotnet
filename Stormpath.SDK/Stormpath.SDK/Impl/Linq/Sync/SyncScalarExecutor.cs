using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Linq.Sync
{
    internal sealed class SyncScalarExecutor<TItem>
    {
        private readonly CollectionResourceExecutor<TItem> executor;

        public SyncScalarExecutor(CollectionResourceExecutor<TItem> executor, Expression expression)
        {
            this.executor = new CollectionResourceExecutor<TItem>(executor, expression);
        }

        public object Execute()
        {
            this.executor.MoveNext();
            var resultOperator = this.executor.CompiledModel.ResultOperator;

            if (resultOperator == Parsing.ResultOperator.Any)
                return this.executor.CurrentPage.Any();

            if (resultOperator == Parsing.ResultOperator.Count)
                return this.executor.Size;

            if (resultOperator == Parsing.ResultOperator.LongCount)
                return this.executor.Size;

            if (resultOperator == Parsing.ResultOperator.First)
            {
                return this.executor.CompiledModel.ResultDefaultIfEmpty
                    ? this.executor.CurrentPage.FirstOrDefault()
                    : this.executor.CurrentPage.First();
            }

            if (resultOperator == Parsing.ResultOperator.Single)
            {
                return this.executor.CompiledModel.ResultDefaultIfEmpty
                    ? this.executor.CurrentPage.SingleOrDefault()
                    : this.executor.CurrentPage.Single();
            }

            throw new NotSupportedException("This result operator is not supported.");
        }
    }
}
