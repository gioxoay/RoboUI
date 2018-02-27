using System.Linq.Expressions;

namespace RoboUI.Filters
{
    internal class ExpressionConstants
    {
        internal static Expression TrueLiteral => Expression.Constant(true);

        internal static Expression FalseLiteral => Expression.Constant(false);

        internal static Expression NullLiteral => Expression.Constant(null);
    }
}
