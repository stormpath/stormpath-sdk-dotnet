// <copyright file="CollectionResourceQueryModelVisitor.cs" company="Stormpath, Inc.">
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
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Stormpath.SDK.Impl.Linq.Parsing;
using Stormpath.SDK.Impl.Linq.RequestModel;
using Stormpath.SDK.Impl.Linq.StaticNameTranslators;

namespace Stormpath.SDK.Impl.Linq
{
    internal sealed class CollectionResourceQueryModelVisitor : QueryModelVisitorBase
    {
        private static readonly int DefaultApiPageLimit = 100;

        public CollectionResourceRequestModel ParsedModel { get; private set; } = new CollectionResourceRequestModel();

        public static CollectionResourceRequestModel GenerateRequestModel(QueryModel queryModel)
        {
            var visitor = new CollectionResourceQueryModelVisitor();
            visitor.VisitQueryModel(queryModel);
            return visitor.ParsedModel;
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            // Used to store the original type the query is executing against
            // (see ExecuteScalar in CollectionResourceQueryExecutor for why we need this)
            this.ParsedModel.CollectionType = fromClause.ItemType;

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (this.IsUnsupportedResultOperator(resultOperator))
                throw new NotSupportedException("One or more LINQ operators are not supported.");

            bool isScalar =
                resultOperator is AnyResultOperator ||
                resultOperator is FirstResultOperator ||
                resultOperator is SingleResultOperator;
            if (isScalar)
            {
                this.ParsedModel.Limit = 1;
                this.ParsedModel.ExecutionPlan.MaxItems = 1;
                return;
            }

            // TODO Count/LongCount
            // Todo DefaultIfEmpty
            // Todo ElementAt[OrDefault]
            if (this.HandleTakeResultOperator(resultOperator))
                return; // done

            if (this.HandleSkipResultOperator(resultOperator))
                return; // done

            if (this.HandleExpandExtensionResultOperator(resultOperator))
                return; // done

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        private bool IsUnsupportedResultOperator(ResultOperatorBase resultOperator)
        {
            // TODO make this a dictionary lookup
            return resultOperator is AllResultOperator ||
                resultOperator is AggregateResultOperator ||
                resultOperator is AggregateFromSeedResultOperator ||
                resultOperator is AverageResultOperator ||
                resultOperator is CastResultOperator ||
                resultOperator is ContainsResultOperator ||
                resultOperator is DefaultIfEmptyResultOperator ||
                resultOperator is DistinctResultOperator ||
                resultOperator is ExceptResultOperator ||
                resultOperator is GroupResultOperator ||
                resultOperator is IntersectResultOperator ||
                resultOperator is LastResultOperator ||
                resultOperator is MaxResultOperator ||
                resultOperator is MinResultOperator ||
                resultOperator is OfTypeResultOperator ||
                resultOperator is ReverseResultOperator ||
                resultOperator is SumResultOperator ||
                resultOperator is UnionResultOperator;
        }

        private bool HandleTakeResultOperator(ResultOperatorBase resultOperator)
        {
            var takeResultOperator = resultOperator as TakeResultOperator;
            if (takeResultOperator == null)
                return false;

            var expression = takeResultOperator.Count;
            if (expression.NodeType == ExpressionType.Constant)
            {
                var limit = (int)((ConstantExpression)expression).Value;
                this.ParsedModel.ExecutionPlan.MaxItems = limit;

                this.ParsedModel.Limit = limit;
                if (limit > DefaultApiPageLimit)
                    this.ParsedModel.Limit = DefaultApiPageLimit;
            }
            else
            {
                throw new NotSupportedException("Unsupported expression in Take clause.");
            }

            return true;
        }

        private bool HandleSkipResultOperator(ResultOperatorBase resultOperator)
        {
            var skipResultOperator = resultOperator as SkipResultOperator;
            if (skipResultOperator == null)
                return false;

            var expression = skipResultOperator.Count as ConstantExpression;
            if (expression == null)
                throw new NotSupportedException("Unsupported expression in Skip clause.");

            this.ParsedModel.Offset = (int)expression.Value;

            return true;
        }

        private bool HandleExpandExtensionResultOperator(ResultOperatorBase resultOperator)
        {
            var expandResultOperator = resultOperator as ExpandResultOperator;
            if (expandResultOperator == null)
                return false;

            var methodCallExpression = expandResultOperator.KeySelector as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException("Expand must be used on a link property method.");

            var expandField = string.Empty;
            if (LinkMethodNameTranslator.TryGetValue(methodCallExpression.Method.Name, out expandField))
            {
                bool paginationParametersPresent =
                    expandResultOperator?.Offset?.Value != null ||
                    expandResultOperator?.Limit?.Value != null;
                if (paginationParametersPresent)
                    throw new NotSupportedException("Pagination options cannot be used on link-only properties.");

                this.ParsedModel.Expansions.Add(new ExpansionTerm(expandField));
                return true; // done
            }

            if (CollectionLinkMethodNameTranslator.TryGetValue(methodCallExpression.Method.Name, out expandField))
            {
                this.ParsedModel.Expansions.Add(
                    new ExpansionTerm(
                        expandField,
                        (int?)expandResultOperator.Offset.Value,
                        (int?)expandResultOperator.Limit.Value));
                return true; // done
            }

            throw new NotSupportedException("The selected method does not support expansions.");
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            // Handle simple cases
            if (this.HandleWhereFilterExtensionMethod(whereClause))
                return; // done
            if (this.HandleWhereWithinDateExtensionMethod(whereClause))
                return; // done

            this.ParsedModel.AddAttributeTerms(
                CollectionResourceWhereExpressionVisitor.GenerateModels(whereClause.Predicate));

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        private bool HandleWhereFilterExtensionMethod(WhereClause whereClause)
        {
            var filterClause = whereClause as FilterClause;
            if (filterClause == null)
                return false;

            this.ParsedModel.FilterTerm = filterClause.Term;
            return true;
        }

        private bool HandleWhereWithinDateExtensionMethod(WhereClause whereClause)
        {
            var methodCall = whereClause.Predicate as MethodCallExpression;
            bool isWithinMethodCall = methodCall?.Method.DeclaringType == typeof(CollectionResourceQueryableWithinExtensions);
            if (!isWithinMethodCall)
                return false;

            var fieldName = string.Empty;
            var methodCallMember = methodCall.Arguments[0] as MemberExpression;
            bool validField = methodCallMember != null && DatetimeFieldNameTranslator.TryGetValue(methodCallMember.Member.Name, out fieldName);
            if (!validField)
                throw new NotSupportedException("Within must be used on a supported datetime field.");

            var numberOfConstantArgs = methodCall.Arguments.Count - 1;
            var yearArg = (int)(methodCall.Arguments[1] as ConstantExpression).Value;
            int? monthArg = null,
                dayArg = null,
                hourArg = null,
                minuteArg = null,
                secondArg = null;

            if (methodCall.Arguments.Count >= 3)
                monthArg = (methodCall.Arguments?[2] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 4)
                dayArg = (methodCall.Arguments?[3] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 5)
                hourArg = (methodCall.Arguments?[4] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count >= 6)
                minuteArg = (methodCall.Arguments?[5] as ConstantExpression)?.Value as int?;
            if (methodCall.Arguments.Count == 7)
                secondArg = (methodCall.Arguments?[6] as ConstantExpression)?.Value as int?;

            this.ParsedModel.AddAttributeTerm(new DatetimeShorthandAttributeTermModel(fieldName, yearArg, monthArg, dayArg, hourArg, minuteArg, secondArg));

            return true;
        }

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            var memberAccessor = ordering.Expression as MemberExpression;
            if (memberAccessor == null)
                throw new NotSupportedException("Only resource fields are supported in OrderBy.");

            var fieldName = string.Empty;
            if (!FieldNameTranslator.TryGetValue(memberAccessor.Member.Name, out fieldName))
                throw new NotSupportedException($"The field {memberAccessor.Member.Name} is not supported in OrderBy.");

            this.ParsedModel.AddOrderBy(fieldName, descending: ordering.OrderingDirection == OrderingDirection.Desc);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            // We have to determine if this Select is being called internally by a LINQ filter like Take
            // which essentially appends .Select(x => x) at the end of the query. If so, it's fine.
            // TODO This also applies to LINQ query sytax: from x select x is fine
            bool isInternalSelect = queryModel.MainFromClause ==
                (selectClause.Selector as QuerySourceReferenceExpression)?.ReferencedQuerySource as MainFromClause;

            if (!isInternalSelect)
                throw new NotSupportedException("Select is not supported.");

            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("GroupJoin is not supported.");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Join is not supported.");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            throw new NotSupportedException("Join is not supported.");
        }
    }
}
