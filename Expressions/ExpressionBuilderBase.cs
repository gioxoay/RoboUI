using System;
using System.Linq.Expressions;

namespace RoboUI.Expressions
{
    public abstract class ExpressionBuilderBase
    {
        private readonly Type itemType;
        private readonly ExpressionBuilderOptions options;
        private ParameterExpression parameterExpression;

        protected ExpressionBuilderBase(Type itemType)
        {
            this.itemType = itemType;
            options = new ExpressionBuilderOptions();
        }

        public ExpressionBuilderOptions Options => options;

        protected internal Type ItemType => itemType;

        protected internal ParameterExpression ParameterExpression
        {
            get
            {
                if (parameterExpression == null)
                    parameterExpression = Expression.Parameter(ItemType, "item");
                return parameterExpression;
            }
            set => parameterExpression = value;
        }
    }
}
