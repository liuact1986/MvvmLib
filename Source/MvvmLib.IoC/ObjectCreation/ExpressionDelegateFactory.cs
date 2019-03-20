using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.IoC
{

    public sealed class ExpressionDelegateFactory : IDelegateFactory
    {
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

        public Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo ctor)
        {
            try
            {
                var argsExpression = Expression.Parameter(typeof(object[]), "args");

                var parameters = ctor.GetParameters();
                var parameterExpressions = new Expression[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Expression index = Expression.Constant(i);
                    Type paramType = parameters[i].ParameterType;
                    Expression paramAccessorExp = Expression.ArrayIndex(argsExpression, index);
                    Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                    parameterExpressions[i] = paramCastExp;
                }
                Expression newExpression = Expression.New(ctor, parameterExpressions);
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


