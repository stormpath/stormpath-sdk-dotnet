// <copyright file="QueryModelCompiler.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Linq.Expressions;
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

            // Transform VB.NET generated calls
            var visualBasicCallTransformer = new VbCallTransformingVisitor();
            var transformedExpression = visualBasicCallTransformer.Visit(evaluatedExpression);

            // Discover
            var discoveringVisitor = new DiscoveringExpressionVisitor();
            discoveringVisitor.Visit(transformedExpression);

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
