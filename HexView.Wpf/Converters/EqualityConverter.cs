namespace HexView.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// A converter which tests for equality against a parameter.
    /// </summary>
    internal class EqualityConverter : IValueConverter
    {
        /// <summary>
        /// Tests the value against the parameter for equality.
        /// </summary>
        ///
        /// <param name="value">
        /// The value produced by the binding source.
        /// </param>
        ///
        /// <param name="targetType">
        /// The type of the binding target property which must be <see cref="bool"/>.
        /// </param>
        ///
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        ///
        /// <param name="culture">
        /// The culture to use in the converter which is unused.
        /// </param>
        ///
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is equal to <paramref name="parameter"/>; <c>false</c> otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new ArgumentException("Argument targetType must be of type 'Boolean'", nameof(targetType));
            }

            return value.Equals(parameter);
        }

        /// <summary>
        /// Tests the value against the parameter for equality.
        /// </summary>
        ///
        /// <param name="value">
        /// The value produced by the binding source.
        /// </param>
        ///
        /// <param name="targetType">
        /// The type of the binding target property which must be <see cref="bool"/>.
        /// </param>
        ///
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        ///
        /// <param name="culture">
        /// The culture to use in the converter which is unused.
        /// </param>
        ///
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is equal to <paramref name="parameter"/>; <c>false</c> otherwise.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
