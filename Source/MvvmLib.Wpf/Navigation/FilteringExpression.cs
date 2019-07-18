using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The base class for filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FilteringExpressionBase<T>
    {
        /// <summary>
        /// The predicate expression.
        /// </summary>
        protected Expression<Predicate<T>> expression;
        /// <summary>
        /// The predicate expression.
        /// </summary>
        public Expression<Predicate<T>> Expression
        {
            get { return expression; }
        }

        /// <summary>
        /// The filter used by CollectionView.
        /// </summary>
        protected Predicate<object> filter;
        /// <summary>
        /// The filter used by CollectionView.
        /// </summary>
        public Predicate<object> Filter
        {
            get { return filter; }
        }

        /// <summary>
        /// The generic filter.
        /// </summary>
        protected Predicate<T> protectedFilter;
        /// <summary>
        /// The generic filter.
        /// </summary>
        public Predicate<T> GenericFilter
        {
            get { return protectedFilter; }
        }


        /// <summary>
        /// Creates the <see cref="FilteringExpressionBase{T}"/>.
        /// </summary>
        public FilteringExpressionBase()
        {
            this.filter = new Predicate<object>(PassesFilter);
        }

        /// <summary>
        /// Refresh the filter.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Checks the filter.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if passes filter</returns>
        public virtual bool PassesFilter(object item)
        {
            if (protectedFilter == null || protectedFilter((T)item))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks the filter.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>True if passes filter</returns>
        public virtual bool PassesFilter(T item)
        {
            if (protectedFilter == null || protectedFilter(item))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// A simple filter with property name, operator and value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilteringExpression<T> : FilteringExpressionBase<T>
    {
        private readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

        private CultureInfo culture;
        /// <summary>
        /// The culture.
        /// </summary>
        public CultureInfo Culture
        {
            get { return culture ?? DefaultCulture; }
            set { culture = value; }
        }

        private bool isCaseSensitive;
        /// <summary>
        /// Allows to change case case sensitive.
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set { isCaseSensitive = value; }
        }

        private readonly string propertyName;
        /// <summary>
        /// The property name.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }

        private PredicateOperator @operator;
        /// <summary>
        /// The operator.
        /// </summary>
        public PredicateOperator Operator
        {
            get { return @operator; }
            set
            {
                if (!Equals(@operator, value))
                {
                    @operator = value;
                    Refresh();
                }
            }
        }

        private object value;
        /// <summary>
        /// The value.
        /// </summary>
        public object Value
        {
            get { return value; }
            set
            {
                if (!Equals(this.value, value))
                {
                    this.value = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="FilteringExpression{T}"/>.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="operator">The operator</param>
        /// <param name="value">The value</param>
        public FilteringExpression(string propertyName, PredicateOperator @operator, object value)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            this.propertyName = propertyName;
            this.@operator = @operator;
            this.value = value;
            this.Refresh();
        }

        /// <summary>
        /// Refresh the filter.
        /// </summary>
        public override void Refresh()
        {
            var builder = new PredicateBuilder();
            var expression = builder.CreatePredicateExpression<T>(propertyName, @operator, value, Culture, isCaseSensitive);
            var compiled = expression.Compile();

            this.expression = expression;
            this.protectedFilter = compiled;
        }
    }

    /// <summary>
    /// A composite filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositeFilteringExpression<T> : FilteringExpressionBase<T>
    {
        private readonly List<FilteringExpressionBase<T>> filters;
        /// <summary>
        /// The filters list.
        /// </summary>
        public List<FilteringExpressionBase<T>> Filters
        {
            get { return filters; }
        }

        private LogicalOperator logicalOperator;
        /// <summary>
        /// The logical operator.
        /// </summary>
        public LogicalOperator LogicalOperator
        {
            get { return logicalOperator; }
            set
            {
                if (!Equals(logicalOperator, value))
                {
                    logicalOperator = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// creates the <see cref="CompositeFilteringExpression{T}"/>.
        /// </summary>
        /// <param name="logicalOperator">The logical operator</param>
        public CompositeFilteringExpression(LogicalOperator logicalOperator)
        {
            this.filters = new List<FilteringExpressionBase<T>>();
            this.logicalOperator = logicalOperator;
        }

        /// <summary>
        /// creates the <see cref="CompositeFilteringExpression{T}"/> with AND operator.
        /// </summary>
        public CompositeFilteringExpression()
            : this(LogicalOperator.And)
        { }

        /// <summary>
        /// Adds a filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <param name="refresh">Allows to refresh the composite filter</param>
        public void AddFilter(FilteringExpressionBase<T> filter, bool refresh = false)
        {
            this.filters.Add(filter);
            if (refresh)
                Refresh();
        }

        /// <summary>
        /// Removes the filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <returns>True if removed</returns>
        public bool RemoveFilter(FilteringExpressionBase<T> filter)
        {
            bool removed = false;
            if (this.filters.Contains(filter))
            {
                removed = this.filters.Remove(filter);
                Refresh();
            }
            return removed;
        }

        /// <summary>
        /// Clears the filters.
        /// </summary>
        public void ClearFilters()
        {
            this.filters.Clear();
            this.Refresh();
        }

        /// <summary>
        /// Refresh the filter.
        /// </summary>
        public override void Refresh()
        {
            var builder = new PredicateBuilder();
            Expression<Predicate<T>> result = null;
            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                filter.Refresh();

                if (i > 0)
                    result = builder.Combine<T>(result, filter.Expression, logicalOperator);
                else
                    result = filter.Expression;
            }

            var compiled = result != null ? result.Compile() : null;
            this.expression = result;
            this.protectedFilter = compiled;
        }
    }

}
