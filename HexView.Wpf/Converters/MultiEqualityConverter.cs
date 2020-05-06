namespace HexView.Wpf.Converters
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// A converter which tests for equality against an <see cref="ArrayList"/> of parameters.
    /// </summary>
    internal class MultiEqualityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Tests the values against the sequence of parameters for equality.
        /// </summary>
        ///
        /// <param name="values">
        /// The values produced by the binding source.
        /// </param>
        ///
        /// <param name="targetType">
        /// The type of the binding target property which must be <see cref="bool"/>.
        /// </param>
        ///
        /// <param name="parameter">
        /// The converter parameters to use which must be of type <see cref="ArrayList"/>.
        /// </param>
        ///
        /// <param name="culture">
        /// The culture to use in the converter which is unused.
        /// </param>
        ///
        /// <returns>
        /// <c>true</c> if the one-to-one mapping of <paramref name="values"/> to <paramref name="parameter"/> all test
        /// for equality induvidually; <c>false</c> otherwise.
        /// </returns>
        ///
        /// <remarks>
        /// Ideally we would like to use <c>x:Array</c> to supply the parameters, however WPF currently throws an
        /// exception that the <see cref="MultiBinding.ConverterParameter"/> is <c>null</c>. It is unkonwn whether this
        /// is by deisgn or it is an existing bug in WPF itself.
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new ArgumentException("Argument targetType must be of type 'Boolean'", nameof(targetType));
            }

            if (parameter.GetType() != typeof(ArrayList))
            {
                throw new ArgumentException("Argument parameter must be of type 'ArrayList'", nameof(parameter));
            }

            var parameters = (ArrayList)parameter;

            if (parameters.Count != values.Length)
            {
                throw new ArgumentException("Arguments parameter and values must be of equal length");
            }

            for (var i = 0; i < values.Length; ++i)
            {
                if (!values[i].Equals(parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tests the values against the sequence of parameters for equality.
        /// </summary>
        ///
        /// <param name="value">
        /// The values produced by the binding source.
        /// </param>
        ///
        /// <param name="targetTypes">
        /// The type of the binding target property which must be <see cref="bool"/>.
        /// </param>
        ///
        /// <param name="parameter">
        /// The converter parameters to use which must be of type <see cref="ArrayList"/>.
        /// </param>
        ///
        /// <param name="culture">
        /// The culture to use in the converter which is unused.
        /// </param>
        ///
        /// <returns>
        /// <c>true</c> if the one-to-one mapping of <paramref name="value"/> to <paramref name="parameter"/> all test
        /// for equality induvidually; <c>false</c> otherwise.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var parameters = (ArrayList)parameter;

            var result = new object[parameters.Count];
            parameters.CopyTo(result);

            return result;
        }
    }
}
