// <copyright file="FilterExpressionNode.cs" company="Stormpath, Inc.">
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
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal sealed class FilterExpressionNode : MethodCallExpressionNodeBase
    {
        private readonly ConstantExpression filterTerm;

        public FilterExpressionNode(MethodCallExpressionParseInfo parseInfo, ConstantExpression filterTerm)
            : base(parseInfo)
        {
            this.filterTerm = filterTerm;
        }

        public static MethodInfo[] SupportedMethods
        {
            get
            {
                return typeof(CollectionResourceQueryableFilterExtensions).GetMethods().ToArray();
            }
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return this.Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }

        protected override QueryModel ApplyNodeSpecificSemantics(QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
        {
            queryModel.BodyClauses.Add(new FilterClause(this.filterTerm));
            return queryModel;
        }
    }
}
