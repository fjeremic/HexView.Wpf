namespace HexView.Wpf
{
    using System;
    using System.IO;

    /// <summary>
    /// Binary Reader Extension methods to handle Big Endian, floats and Doubles.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Reads a 2-byte signed integer in big-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int16 (signed short).</returns>
        public static short ReadInt16BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer in big-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt16 (unsigned short).</returns>
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        /// Reads a 4-byte signed integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int32 (signed int).</returns>
        public static int ReadInt32BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt32 (unsigned int).</returns>
        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        /// Reads a 8-byte signed integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int64 (signed long).</returns>
        public static long ReadInt64BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt64 (unsigned long).</returns>
        public static ulong ReadUInt64BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToUInt64(data, 0);
        }

        /// <summary>
        /// Reads a 4-byte float and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (float).</returns>
        public static float ReadFloat(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Reads a 4-byte float in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (float).</returns>
        public static float ReadFloatBE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        /// <summary>
        /// Reads a 8-byte double and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDouble(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BitConverter.ToDouble(data, 0);
        }

        /// <summary>
        /// Reads a 8-byte double in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDoubleBE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }
    }
}
