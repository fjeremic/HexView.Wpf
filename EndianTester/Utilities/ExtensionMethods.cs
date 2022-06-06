﻿namespace EndianTester.Utilities
{
    using System;
    using System.IO;

    /// <summary>
    /// Binary Reader Extension methods to handle Big & Little Endian, floats and Doubles on all computer endian architectures
    /// If system endian does not match the desired endian, the bytes are reversed.
    /// BinaryPrimitives ARE NOT USED, they are very slow (2.5x) compared to shift-or operation.
    /// Note single & double conversion may return NaN for some values, this is expected operation.
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
            Int16 value = reader.ReadInt16();
            if (BitConverter.IsLittleEndian == true)
            {
                return (Int16)((value & 0x00FFU) << 8 |
                              (value & 0xFF00U) >> 8);
            }
            return value;
        }

        /// <summary>
        /// Reads a 2-byte signed integer in little-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int16 (signed short).</returns>
        public static short ReadInt16LE(this BinaryReader reader)
        {
            Int16 value = reader.ReadInt16();
            if (BitConverter.IsLittleEndian == false)
            {
                return (Int16)((value & 0x00FFU) << 8 |
                              (value & 0xFF00U) >> 8);
            }
            return value;
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer in big-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt16 (unsigned short).</returns>
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            UInt16 value = reader.ReadUInt16();
            if (BitConverter.IsLittleEndian == true)
            {
                return (UInt16)((value & 0x00FFU) << 8 |
                              (value & 0xFF00U) >> 8);
            }
            return value;
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer in little-endian order and advances the current position of the stream by 2 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt16 (unsigned short).</returns>
        public static ushort ReadUInt16LE(this BinaryReader reader)
        {
            UInt16 value = reader.ReadUInt16();
            if (BitConverter.IsLittleEndian == false)
            {
                return (UInt16)((value & 0x00FFU) << 8 |
                              (value & 0xFF00U) >> 8);
            }
            return value;
        }

        /// <summary>
        /// Reads a 4-byte signed integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int32 (signed int).</returns>
        public static int ReadInt32BE(this BinaryReader reader)
        {
            UInt32 value = reader.ReadUInt32();
            if (BitConverter.IsLittleEndian == true)
            {
                return (Int32)((value & 0x000000FFU) << 24 |
                               (value & 0xFF000000U) >> 24 |
                               (value & 0x0000FF00U) << 8 |
                               (value & 0x00FF0000U) >> 8);
            }
            return (Int32)value;
        }

        /// <summary>
        /// Reads a 4-byte signed integer in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int32 (signed int).</returns>
        public static int ReadInt32LE(this BinaryReader reader)
        {
            UInt32 value = reader.ReadUInt32();
            if (BitConverter.IsLittleEndian == false)
            {
                return (Int32)((value & 0x000000FFU) << 24 |
                               (value & 0xFF000000U) >> 24 |
                               (value & 0x0000FF00U) << 8 |
                               (value & 0x00FF0000U) >> 8);
            }
            return (Int32)value;
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt32 (unsigned int).</returns>
        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            UInt32 value = reader.ReadUInt32();
            if (BitConverter.IsLittleEndian == true)
            {
                return (value & 0x000000FFU) << 24 |
                        (value & 0xFF000000U) >> 24 |
                        (value & 0x0000FF00U) << 8 |
                        (value & 0x00FF0000U) >> 8;
            }
            return value;
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt32 (unsigned int).</returns>
        public static uint ReadUInt32LE(this BinaryReader reader)
        {
            UInt32 value = reader.ReadUInt32();
            if (BitConverter.IsLittleEndian == false)
            {
                return (value & 0x000000FFU) << 24 |
                        (value & 0xFF000000U) >> 24 |
                        (value & 0x0000FF00U) << 8 |
                        (value & 0x00FF0000U) >> 8;
            }
            return value;
        }

        /// <summary>
        /// Reads a 8-byte signed integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int64 (signed long).</returns>
        public static long ReadInt64BE(this BinaryReader reader)
        {
            UInt64 value = reader.ReadUInt64();
            if (BitConverter.IsLittleEndian == true)
            {
                return (Int64)((value & 0x00000000000000FFUL) << 56 |
                               (value & 0xFF00000000000000UL) >> 56 |
                               (value & 0x000000000000FF00UL) << 40 |
                               (value & 0x00FF000000000000UL) >> 40 |
                               (value & 0x0000000000FF0000UL) << 24 |
                               (value & 0x0000FF0000000000UL) >> 24 |
                               (value & 0x00000000FF000000UL) << 8 |
                               (value & 0x000000FF00000000UL) >> 8);
            }
            return (Int64)value;
        }

        /// <summary>
        /// Reads a 8-byte signed integer in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>Int64 (signed long).</returns>
        public static long ReadInt64LE(this BinaryReader reader)
        {
            UInt64 value = reader.ReadUInt64();
            if (BitConverter.IsLittleEndian == false)
            {
                return (Int64)((value & 0x00000000000000FFUL) << 56 |
                               (value & 0xFF00000000000000UL) >> 56 |
                               (value & 0x000000000000FF00UL) << 40 |
                               (value & 0x00FF000000000000UL) >> 40 |
                               (value & 0x0000000000FF0000UL) << 24 |
                               (value & 0x0000FF0000000000UL) >> 24 |
                               (value & 0x00000000FF000000UL) << 8 |
                               (value & 0x000000FF00000000UL) >> 8);
            }
            return (Int64)value;
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt64 (unsigned long).</returns>
        public static ulong ReadUInt64BE(this BinaryReader reader)
        {
            UInt64 value = reader.ReadUInt64();
            if (BitConverter.IsLittleEndian == true)
            {
                return (value & 0x00000000000000FFUL) << 56 |
                        (value & 0xFF00000000000000UL) >> 56 |
                        (value & 0x000000000000FF00UL) << 40 |
                        (value & 0x00FF000000000000UL) >> 40 |
                        (value & 0x0000000000FF0000UL) << 24 |
                        (value & 0x0000FF0000000000UL) >> 24 |
                        (value & 0x00000000FF000000UL) << 8 |
                        (value & 0x000000FF00000000UL) >> 8;
            }
            return value;
        }

        /// <summary>
        /// Reads a 8-byte unsigned integer in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>UInt64 (unsigned long).</returns>
        public static ulong ReadUInt64LE(this BinaryReader reader)
        {
            UInt64 value = reader.ReadUInt64();
            if (BitConverter.IsLittleEndian == false)
            {
                return (value & 0x00000000000000FFUL) << 56 |
                        (value & 0xFF00000000000000UL) >> 56 |
                        (value & 0x000000000000FF00UL) << 40 |
                        (value & 0x00FF000000000000UL) >> 40 |
                        (value & 0x0000000000FF0000UL) << 24 |
                        (value & 0x0000FF0000000000UL) >> 24 |
                        (value & 0x00000000FF000000UL) << 8 |
                        (value & 0x000000FF00000000UL) >> 8;
            }
            return value;
        }

        /// <summary>
        /// Reads a 4-byte float in little-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (single).</returns>
        public static float ReadFloatLE(this BinaryReader reader)
        {
            Int32 data = ReadInt32LE(reader);
            return BitConverter.Int32BitsToSingle(data);
        }

        /// <summary>
        /// Reads a 4-byte float in big-endian order and advances the current position of the stream by 4 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float32 (single).</returns>
        public static float ReadFloatBE(this BinaryReader reader)
        {
            Int32 data = ReadInt32BE(reader);
            return BitConverter.Int32BitsToSingle(data);
        }

        /// <summary>
        /// Reads a 8-byte double in little-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDoubleLE(this BinaryReader reader)
        {
            Int64 data = ReadInt64LE(reader);
            return BitConverter.Int64BitsToDouble(data);
        }

        /// <summary>
        /// Reads a 8-byte double in big-endian order and advances the current position of the stream by 8 bytes.
        /// </summary>
        /// <param name="reader">in-use BinaryReader.</param>
        /// <returns>float64 (double).</returns>
        public static double ReadDoubleBE(this BinaryReader reader)
        {
            Int64 data = ReadInt64BE(reader);
            return BitConverter.Int64BitsToDouble(data);
        }
    }
}
