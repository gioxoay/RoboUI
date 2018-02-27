using System;
using System.Linq.Expressions;

namespace RoboUI.Expressions
{
    public abstract class MemberAccessExpressionBuilderBase : ExpressionBuilderBase
    {
        private readonly string memberName;

        protected MemberAccessExpressionBuilderBase(Type itemType, string memberName)
            : base(itemType)
        {
            this.memberName = memberName;
        }

        public string MemberName => memberName;

        public abstract Expression CreateMemberAccessExpression();

        public LambdaExpression CreateLambdaExpression()
        {
            return Expression.Lambda(CreateMemberAccessExpression(), ParameterExpression);
        }
    }
}
