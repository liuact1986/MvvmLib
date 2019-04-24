using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to gets instance factories with Linq Expressions (more efficient performance).
    /// </summary>
    public sealed class ExpressionDelegateFactory : IDelegateFactory
    {
        /// <summary>
        /// Creates a factory for an empty constructor.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        public Func<T> CreateConstructor<T>(Type type, ConstructorInfo constructor)
        {
            try
            {
                Expression newExpression = Expression.New(type);
                newExpression = ExpressionUtils.EnsureCastExpression(newExpression, typeof(T));
                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<T>), newExpression);
                return (Func<T>)lambdaExpression.Compile();
            }
            catch
            {
                return () => (T)Activator.CreateInstance(type);
            }
        }

        /// <summary>
        /// Creates a factory for a constructor with parameters.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        public Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo constructor)
        {
            try
            {
                var argsExpression = Expression.Parameter(typeof(object[]), "args");

                var parameters = constructor.GetParameters();
                var parameterExpressions = new Expression[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Expression index = Expression.Constant(i);
                    Type paramType = parameters[i].ParameterType;
                    Expression paramAccessorExp = Expression.ArrayIndex(argsExpression, index);
                    Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                    parameterExpressions[i] = paramCastExp;
                }
                Expression newExpression = Expression.New(constructor, parameterExpressions);
                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<object[], T>), newExpression, argsExpression);
                return (Func<object[], T>)lambdaExpression.Compile();
            }
            catch
            {
                return (p) => (T)Activator.CreateInstance(type, p);
            }
        }
    }

}


