using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to create predicate with <see cref="Expression"/>.
    /// </summary>
    public class PredicateBuilder
    {
        /// <summary>
        /// Creates a <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <returns>The parameter expression created</returns>
        public ParameterExpression CreateParameter(Type type)
        {
            // p =>
            var parameterExpression = Expression.Parameter(type);
            return parameterExpression;
        }

        /// <summary>
        /// Creates a <see cref="ParameterExpression"/>.
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <returns>The parameter expression created</returns>
        public ParameterExpression CreateParameter<T>()
        {
            return CreateParameter(typeof(T));
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="parameter">The parameter expression</param>
        /// <param name="propertyPath">The property path</param>
        /// <returns>The member expression created</returns>
        public MemberExpression CreateMember(Expression parameter, string propertyPath)
        {
            // p.FirstName
            var propertyNames = propertyPath.Split('.');
            if (propertyNames.Length == 1)
            {
                var memberExpression = Expression.Property(parameter, propertyPath);
                return memberExpression;
            }
            else
            {
                // p.SubItem.SubSubItem.MyProperty
                MemberExpression property = null;
                foreach (string propertyName in propertyNames)
                {
                    var tmp = property == null ? parameter : property;
                    property = Expression.Property(tmp, propertyName);
                }
                return property;
            }
        }


        /// <summary>
        /// Creates a <see cref="ConstantExpression"/>.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The constant expression created</returns>
        public ConstantExpression CreateConstant(object value)
        {
            // "Marie"
            var constantExpression = Expression.Constant(value);
            return constantExpression;
        }

        /// <summary>
        /// Creates the body <see cref="Expression"/>.
        /// </summary>
        /// <param name="operator">The operator</param>
        /// <param name="left">The left expression</param>
        /// <param name="right">The right expression</param>
        /// <param name="isCaseSensitive">Checks if is case sensitive</param>
        /// <returns>The expression created</returns>
        public Expression CreateBody(PredicateOperator @operator, Expression left, Expression right, bool isCaseSensitive)
        {
            // p.FirstName == "Marie"
            Expression expression = null;
            switch (@operator)
            {
                case PredicateOperator.IsEqual:
                    expression = Expression.Equal(left, right);
                    break;
                case PredicateOperator.IsLessThan:
                    expression = Expression.LessThan(left, right);
                    break;
                case PredicateOperator.IsGreaterThan:
                    expression = Expression.GreaterThan(left, right);
                    break;
                case PredicateOperator.IsGreaterThanOrEqualTo:
                    expression = Expression.GreaterThanOrEqual(left, right);
                    break;
                case PredicateOperator.IsLessThanOrEqualTo:
                    expression = Expression.LessThanOrEqual(left, right);
                    break;
                case PredicateOperator.IsNotEqual:
                    expression = Expression.NotEqual(left, right);
                    break;
                case PredicateOperator.StartsWith:
                    if (isCaseSensitive)
                    {
                        expression = Expression.Call(left, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), right);
                    }
                    else
                    {
                        expression = Expression.Call(left, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string), typeof(StringComparison) }),
                            right, Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                    }
                    break;
                case PredicateOperator.EndsWith:
                    if (isCaseSensitive)
                    {
                        expression = Expression.Call(left, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), right);
                    }
                    else
                    {
                        expression = Expression.Call(left, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string), typeof(StringComparison) }),
                            right, Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                    }
                    break;
                case PredicateOperator.Contains:
                    if (isCaseSensitive)
                    {
                        expression = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                    }
                    else
                    {
                        expression = Expression.GreaterThanOrEqual(Expression.Call(left, typeof(string).GetMethod("IndexOf", new Type[] { typeof(string), typeof(StringComparison) }),
                            right, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)), Expression.Constant(0));
                    }
                    break;
                case PredicateOperator.DoesNotContain:
                    if (isCaseSensitive)
                    {
                        expression = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right));
                    }
                    else
                    {
                        expression = Expression.Equal(Expression.Call(left, typeof(string).GetMethod("IndexOf", new Type[] { typeof(string), typeof(StringComparison) }),
                            right, Expression.Constant(StringComparison.InvariantCultureIgnoreCase)), Expression.Constant(-1));
                    }
                    break;
            }
            return expression;
        }

        /// <summary>
        /// Tries to convert the value to the property type.
        /// </summary>
        /// <param name="memberExpression">The member expression</param>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <returns>The value or the converted value</returns>
        public object TryConvert(MemberExpression memberExpression, object value, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }

            var propertyType = ((PropertyInfo)memberExpression.Member).PropertyType;
            var typeOfValue = value.GetType();
            if (!Equals(propertyType, typeOfValue))
            {
                try
                {
                    if (culture != null)
                    {
                        var convertedValue = Convert.ChangeType(value, propertyType, culture);
                        return convertedValue;
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(value, propertyType);
                        return convertedValue;
                    }
                }
                catch
                {
                    throw new ArgumentException($"Failed to convert value '{value}' from type '{typeOfValue.Name}' to type '{propertyType.Name}'");
                }

                // alternative
                //var converter = TypeDescriptor.GetConverter(propertyType);
                //if (!converter.CanConvertFrom(typeOfValue))
                //    throw new NotSupportedException($"Unable to convert value '{value}' from type '{typeOfValue.Name}' to type '{propertyType.Name}'");

                //if (culture != null)
                //{
                //    var convertedValue = converter.ConvertFrom(null, culture, value);
                //    return convertedValue;
                //}
                //else
                //{
                //    var convertedValue = converter.ConvertFrom(value);
                //    return convertedValue;
                //}
            }

            return value;
        }

        /// <summary>
        /// Creates the predicate expression.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="operator">The operator</param>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <param name="isCaseSensitive">Checks if is case sensitive</param>
        /// <returns>The predicate expresion</returns>
        public Expression<Predicate<T>> CreatePredicateExpression<T>(string propertyName, PredicateOperator @operator, object value, CultureInfo culture, bool isCaseSensitive)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            // p => p.FirstName == "Marie"
            var parameterExpression = CreateParameter(typeof(T)); // Type
            var memberExpression = CreateMember(parameterExpression, propertyName); // property name

            value = TryConvert(memberExpression, value, culture); // convert value ?

            var constantExpression = CreateConstant(value); // value

            var bodyExpression = CreateBody(@operator, memberExpression, constantExpression, isCaseSensitive); // operator
            var lambdaExpression = Expression.Lambda<Predicate<T>>(bodyExpression, parameterExpression);
            return lambdaExpression;
        }

        /// <summary>
        /// Creates the predicate expression.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="operator">The operator</param>
        /// <param name="value">The value</param>
        /// <returns>The predicate expresion</returns>
        public Expression<Predicate<T>> CreatePredicateExpression<T>(string propertyName, PredicateOperator @operator, object value)
        {
            return CreatePredicateExpression<T>(propertyName, @operator, value, CultureInfo.InvariantCulture, false);
        }

        /// <summary>
        /// Combines the expressions with the <see cref="LogicalOperator"/>.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="expression1">The first expression</param>
        /// <param name="expression2">The second expression</param>
        /// <param name="operator">The logical operator</param>
        /// <returns>The expression created</returns>
        public Expression<Predicate<T>> Combine<T>(Expression<Predicate<T>> expression1, Expression<Predicate<T>> expression2, LogicalOperator @operator)
        {
            if (expression1 == null)
                throw new ArgumentNullException(nameof(expression1));
            if (expression2 == null)
                throw new ArgumentNullException(nameof(expression2));

            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            switch (@operator)
            {
                case LogicalOperator.And:
                    return Expression.Lambda<Predicate<T>>(Expression.AndAlso(expression1.Body, invokedExpression), expression1.Parameters);
                case LogicalOperator.Or:
                    return Expression.Lambda<Predicate<T>>(Expression.OrElse(expression1.Body, invokedExpression), expression1.Parameters);
                default:
                    throw new ArgumentException("Unexpected Logical operator");
            }
        }
    }

    /// <summary>
    /// The predicate operator.
    /// </summary>
    public enum PredicateOperator
    {
        /// <summary>
        /// Is equal.
        /// </summary>
        IsEqual, // ==
        /// <summary>
        /// Is not equal.
        /// </summary>
        IsNotEqual, // !=  
        /// <summary>
        /// Is less than.
        /// </summary>
        IsLessThan, // <
        /// <summary>
        /// Is less or equal to.
        /// </summary>
        IsLessThanOrEqualTo, // <=
        /// <summary>
        /// Is greater than.
        /// </summary>
        IsGreaterThan, // >
        /// <summary>
        /// Is greater than or equal to.
        /// </summary>
        IsGreaterThanOrEqualTo, // >=
        /// <summary>
        /// Starts with.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Ends with.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Contains.
        /// </summary>
        Contains,
        /// <summary>
        /// Does not contain.
        /// </summary>
        DoesNotContain
    }

    /// <summary>
    /// The logical operator.
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// Logical AND.
        /// </summary>
        And,
        /// <summary>
        /// Logical OR.
        /// </summary>
        Or
    }
}
