namespace HexView.Wpf
{
    /// <summary>
    /// Enumerates how the data (bytes read from the buffer) is to be interpreted when displayed.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Display the data as floating point values.
        /// </summary>
        FloatingPoint,

        /// <summary>
        /// Display the data as integral (integer) values.
        /// </summary>
        Integer,
    }
}
