using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation context class.
    /// </summary>
    public class NavigationContext
    {
        private Type sourceType;
        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        /// <summary>
        /// Creates the <see cref="NavigationContext"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public NavigationContext(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            this.sourceType = sourceType;
            this.parameter = parameter;
        }
    }
}
