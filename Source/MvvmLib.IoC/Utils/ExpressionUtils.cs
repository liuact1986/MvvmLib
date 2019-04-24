using System;
using System.Linq.Expressions;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Linq Expressions utils.
    /// </summary>
    internal class ExpressionUtils
    {
        /// <summary>
        /// Gets default value for value type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default value</returns>
        public static object GetDefaultValue(Type type)
        {
            var expression = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
            return expression.Compile()();
        }

        /// <summary>
        /// Ensure the cast for boxing/ unboxing.
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="targetType">The target type</param>
        /// <returns>The expression or converted  expression</returns>
        public static Expression EnsureCastExpression(Expression expression, Type targetType)
        {
            Type expressionType = expression.Type;
            if (expressionType == targetType || (!expressionType.IsValueType && targetType.IsAssignableFrom(expressionType)))
                return expression;

            if (targetType.IsValueType)
            {
                Expression convert = Expression.Unbox(expression, targetType);
                return Expression.Condition(Expression.Equal(expression, Expression.Constant(null, typeof(object))), 
                    Expression.Default(targetType), convert);
            }
            return Expression.Convert(expression, targetType);
        }
    }
}
