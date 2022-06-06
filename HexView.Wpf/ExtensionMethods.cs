namespace HexView.Wpf
{
    using System;
    using System.Buffers.Binary;
    using System.IO;

    /// <summary>
    /// Binary Reader Extension methods to handle Big Endian, floats and Doubles.
    /// BinaryPrimitives is used for net.core which handles the endian of the computer architecture.
    /// https://github.com/dotnet/runtime/issues/2365 .
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
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }

        /// <summary>
        /// Reads a 2-byte signed integer in little-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int16 (signed short).</returns>
        public static short ReadInt16LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            return BinaryPrimitives.ReadInt16LittleEndian(data);
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer in big-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt16 (unsigned short).</returns>
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            return BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer in little-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt16 (unsigned short).</returns>
        public static ushort ReadUInt16LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            return BinaryPrimitives.ReadUInt16LittleEndian(data);
        }

        /// <summary>
        /// Reads a 4-byte signed integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int32 (signed int).</returns>
        public static int ReadInt32BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }

        /// <summary>
        /// Reads a 4-byte signed integer in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int32 (signed int).</returns>
        public static int ReadInt32LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BinaryPrimitives.ReadInt32LittleEndian(data);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt32 (unsigned int).</returns>
        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BinaryPrimitives.ReadUInt32BigEndian(data);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt32 (unsigned int).</returns>
        public static uint ReadUInt32LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BinaryPrimitives.ReadUInt32LittleEndian(data);
        }

        /// <summary>
        /// Reads a 8-byte signed integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int64 (signed long).</returns>
        public static long ReadInt64BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BinaryPrimitives.ReadInt64BigEndian(data);
        }

        /// <summary>
        /// Reads a 8-byte signed integer in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int64 (signed long).</returns>
        public static long ReadInt64LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BinaryPrimitives.ReadInt64LittleEndian(data);
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt64 (unsigned long).</returns>
        public static ulong ReadUInt64BE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BinaryPrimitives.ReadUInt64BigEndian(data);
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt64 (unsigned long).</returns>
        public static ulong ReadUInt64LE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BinaryPrimitives.ReadUInt64LittleEndian(data);
        }

        /// <summary>
        /// Reads a 4-byte float in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (single).</returns>
        public static float ReadFloatLE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(data));
        }

        /// <summary>
        /// Reads a 4-byte float in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (single).</returns>
        public static float ReadFloatBE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(data));
        }

        /// <summary>
        /// Reads a 8-byte double in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDoubleLE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(data));
        }

        /// <summary>
        /// Reads a 8-byte double in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDoubleBE(this BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64BigEndian(data));
        }
    }
}
