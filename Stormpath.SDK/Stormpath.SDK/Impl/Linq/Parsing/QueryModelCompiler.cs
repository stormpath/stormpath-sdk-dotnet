using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Linq.QueryModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class QueryModelCompiler
    {
        public static CollectionResourceQueryModel Compile(Expression expression)
        {
            var compiler = new QueryModelCompiler();
            return compiler.GenerateQueryModel(expression);
        }

        private CollectionResourceQueryModel GenerateQueryModel(Expression expression)
        {
            // Partial evaluation
            var evaluatedExpression = Evaluator.PartialEval(expression);
            //var evaluatedExpression = expression;

            // Discover
            var discoveringVisitor = new DiscoveringExpressionVisitor();
            discoveringVisitor.Visit(evaluatedExpression);

            // Compile
            var compilingVisitor = new CompilingExpressionVisitor();
            compilingVisitor.Visit(discoveringVisitor.Expressions);
            var compiledModel = compilingVisitor.Model;

            // Validate
            var validator = new QueryModelValidator(compiledModel);
            validator.Validate();

            return compiledModel;
        }
    }
}
