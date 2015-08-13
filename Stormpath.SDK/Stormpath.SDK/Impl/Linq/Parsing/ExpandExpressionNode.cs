// <copyright file="ExpandExpressionNode.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class ExpandExpressionNode : ResultOperatorExpressionNodeBase
    {
        private readonly LambdaExpression keySelectorLambda;
        private readonly ConstantExpression offset = null;
        private readonly ConstantExpression limit = null;

        public ExpandExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression keySelector, ConstantExpression offset = null, ConstantExpression limit = null)
            : base(parseInfo, null, null)
        {
            this.keySelectorLambda = keySelector;
            this.offset = offset;
            this.limit = limit;
        }

        public static MethodInfo[] SupportedMethods
        {
            get
            {
                return typeof(CollectionResourceQueryableExpandExtensions).GetMethods().ToArray();
            }
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            var resolvedParameter = Source
                .Resolve(this.keySelectorLambda.Parameters[0], this.keySelectorLambda.Body, clauseGenerationContext);
            return new ExpandResultOperator(resolvedParameter, this.offset, this.limit);
        }
    }
}
