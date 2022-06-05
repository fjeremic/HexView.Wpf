namespace HexView.Wpf
{
    /// <summary>
    /// Enumerates the address column formatting options.
    /// </summary>
    public enum AddressDisplayFormat
    {
        /// <summary>16 bit HEX address "0000" 4 spaces. </summary>
        Addr16,

        /// <summary>24 bit HEX address "00:0000" 7 spaces. </summary>
        Addr24,

        /// <summary>32 bit HEX address "0000:0000" 9 spaces. </summary>
        Addr32,

        /// <summary>48 bit HEX address "0000:00000000" 13 spaces. </summary>
        Addr48,

        /// <summary>64 bit HEX address "00000000:00000000" 17 spaces. </summary>
        Addr64,
    }
}
