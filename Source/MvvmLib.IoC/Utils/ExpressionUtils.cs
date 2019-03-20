using System;
using System.Linq.Expressions;

namespace MvvmLib.IoC
{
    public class ExpressionUtils
    {
        public static Expression EnsureCastExpression(Expression expression, Type targetType)
        {
            Type expressionType = expression.Type;
            if (expressionType == targetType || (!expressionType.IsValueType && targetType.IsAssignableFrom(expressionType)))
            {
                return expression;
            }

            if (targetType.IsValueType)
            {
                Expression convert = Expression.Unbox(expression, targetType);
                return Expression.Condition(
                    Expression.Equal(expression, Expression.Constant(null, typeof(object))),
                    Expression.Default(targetType), convert);
            }

            return Expression.Convert(expression, targetType);
        }
    }
}
