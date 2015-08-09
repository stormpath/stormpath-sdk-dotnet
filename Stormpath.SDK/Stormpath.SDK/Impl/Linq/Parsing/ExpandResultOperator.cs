// <copyright file="ExpandResultOperator.cs" company="Stormpath, Inc.">
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

using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace Stormpath.SDK.Impl.Linq.Parsing
{
    internal class ExpandResultOperator : SequenceTypePreservingResultOperatorBase
    {
        public ExpandResultOperator(Expression keySelector, ConstantExpression offset, ConstantExpression limit)
        {
            this.KeySelector = keySelector;
            this.Offset = offset;
            this.Limit = limit;
        }

        public Expression KeySelector { get; private set; }

        public ConstantExpression Offset { get; private set; }

        public ConstantExpression Limit { get; private set; }

        public override string ToString()
        {
            return $"Expand({FormattingExpressionTreeVisitor.Format(this.KeySelector)}, offset: {this.Offset}, limit: {this.Limit})";
        }

        public override StreamedSequence ExecuteInMemory<T>(StreamedSequence input)
        {
            return input; // (does not mutate the sequence)
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new ExpandResultOperator(this.KeySelector, this.Offset, this.Limit);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
            this.KeySelector = transformation(this.KeySelector);
        }
    }
}