// <copyright file="AbstractExpressionTransformer.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Linq.Parsing.Transformers
{
    internal abstract class AbstractExpressionTransformer : ExpressionVisitor, IExpressionTransformer
    {
        public Expression Transform(Expression expression)
            => this.Visit(expression);

        public override Expression Visit(Expression node)
        {
            var transformed = this.TransformNode(node);
            if (transformed != node)
                return transformed;

            return base.Visit(node);
        }

        protected abstract Expression TransformNode(Expression node);
    }
}
