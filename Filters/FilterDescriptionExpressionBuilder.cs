using System.Linq.Expressions;
using System.Reflection;

namespace RoboUI.Filters
{
    internal class FilterDescriptionExpressionBuilder : FilterExpressionBuilder
    {
        private readonly FilterDescription filterDescription;

        public FilterDescriptionExpressionBuilder(ParameterExpression parameterExpression,
            FilterDescription filterDescription)
            : base(parameterExpression)
        {
            this.filterDescription = filterDescription;
        }

        public FilterDescription FilterDescription => filterDescription;

        private Expression FilterDescriptionExpression => Expression.Constant(filterDescription);

        private MethodInfo SatisfiesFilterMethodInfo => filterDescription.GetType().GetMethod("SatisfiesFilter", new[]
        {
            typeof (object)
        });

        public override Expression CreateBodyExpression()
        {
            if (filterDescription.IsActive)
                return CreateActiveFilterExpression();
            return ExpressionConstants.TrueLiteral;
        }

        protected virtual Expression CreateActiveFilterExpression()
        {
            return CreateSatisfiesFilterExpression();
        }

        private MethodCallExpression CreateSatisfiesFilterExpression()
        {
            Expression expression = ParameterExpression;
            if (expression.Type.GetTypeInfo().IsValueType)
                expression = Expression.Convert(expression, typeof(object));
            return Expression.Call(FilterDescriptionExpression, SatisfiesFilterMethodInfo, new[]
            {
                expression
            });
        }
    }
}
