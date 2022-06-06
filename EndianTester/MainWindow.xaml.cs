using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using EndianTester.Utilities;
using System.Buffers.Binary;

namespace EndianTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /*    Results on XPS laptop i7-10875H  VS 2021 netcore 3.1
         *    note binary primitives are slow, but easy to use....

            >>>Testing Big Endian INT16 extraction times, 5242880 loops
            Array.Reverse 16 ET = 129.715 mS, nS/loop = 24.741
            BinaryPrimitives 16 ET = 134.741 mS, nS/loop = 25.700
            Shift-OR 16 ET = 48.894 mS, nS/loop = 9.326

            >>>Testing Big Endian INT32 extraction times, 2621440 loops
            Array.Reverse 32 ET = 69.100 mS, nS/loop = 26.360
            BinaryPrimitives 32 ET = 70.928 mS, nS/loop = 27.057
            Shift-OR 32 ET = 25.128 mS, nS/loop = 9.586

            >>>Testing Big Endian INT64 extraction times, 1310720 loops
            Array.Reverse 64 ET = 38.235 mS, nS/loop = 29.171
            BinaryPrimitives 32 ET = 36.075 mS, nS/loop = 27.523
            Shift-OR 64 ET = 13.290 mS, nS/loop = 10.140

            >>>Testing Big Endian FLOAT-32 extraction times, 2621440 loops
            Array.Reverse FLOAT-32 ET = 77.382 mS, nS/loop = 29.519
            Shift-OR FLOAT-32 ET = 42.955 mS, nS/loop = 16.386

            >>>Testing Big Endian FLOAT-64 extraction times, 1310720 loops
            Array.Reverse FLOAT-64 ET = 41.860 mS, nS/loop = 31.937
            Shift-OR FLOAT-64 ET = 21.677 mS, nS/loop = 16.538
        */


        private BinaryReader reader;
        private StatusDisplayer StatReport;

        public MainWindow()
        {
            InitializeComponent();

            this.StatReport = new StatusDisplayer(txtStatusMsgs);
            StatReport.ClearStatusText();

            // 16 MB of random data
            var rand = new Random(1234);
            var bytes = new byte[10 * 1024 * 1024];
            rand.NextBytes(bytes);

            reader = new BinaryReader(new MemoryStream(bytes));

        }

        private void BtnRunTest_Click(object sender, RoutedEventArgs e)
        {
            double ET, TPL;
            Stopwatch sw = new Stopwatch();
            StatReport.ClearStatusText();

            // *******************************************************
            //   I N T 1 6   T E S T S 
            // *******************************************************

            
            int loopcnt = (int)reader.BaseStream.Length / 2;
            StatReport.AppendResponse("\n>>>Testing Big Endian INT16 extraction times, {0} loops", loopcnt);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int16 val = ReadInt16BEReverse(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Array.Reverse 16 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int16 val = ReadInt16BEPrim(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("BinaryPrimitives 16 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int16 val = ReadInt16BEFast(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Shift-OR 16 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);



            // *******************************************************
            //   I N T 3 2   T E S T S 
            // *******************************************************

            loopcnt = (int)reader.BaseStream.Length / 4;
            StatReport.AppendResponse("\n>>>Testing Big Endian INT32 extraction times, {0} loops", loopcnt);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int32 val = ReadInt32BEReverse(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Array.Reverse 32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int32 val = ReadInt32BEPrim(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("BinaryPrimitives 32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int32 val = ReadInt32BEFast(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Shift-OR 32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            // *******************************************************
            //   I N T 64   T E S T S 
            // *******************************************************
            loopcnt = (int)reader.BaseStream.Length / 8;
            StatReport.AppendResponse("\n>>>Testing Big Endian INT64 extraction times, {0} loops", loopcnt);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int64 val = ReadInt64BEReverse(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Array.Reverse 64 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int64 val = ReadInt64BEPrim(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("BinaryPrimitives 32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                Int64 val = ReadInt64BEFast(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Shift-OR 64 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            // *******************************************************
            //   F L O A T   3 2   T E S T S 
            // *******************************************************
            loopcnt = (int)reader.BaseStream.Length / 4;
            StatReport.AppendResponse("\n>>>Testing Big Endian FLOAT-32 extraction times, {0} loops", loopcnt);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                float val = ReadSingleBEReverse(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Array.Reverse FLOAT-32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                float val = ReadSingleBEFast(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Shift-OR FLOAT-32 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            // *******************************************************
            //   F L O A T   6 4   T E S T S 
            // *******************************************************
            loopcnt = (int)reader.BaseStream.Length / 8;
            StatReport.AppendResponse("\n>>>Testing Big Endian FLOAT-64 extraction times, {0} loops", loopcnt);

            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                double val = ReadDoubleBEReverse(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Array.Reverse FLOAT-64 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);


            reader.BaseStream.Position = 0;
            sw.Restart();
            for (int i = 0; i < loopcnt; i++)
            {
                double val = ReadDoubleBEFast(reader);
            }
            sw.Stop();
            ET = (double)sw.ElapsedTicks;
            TPL = ET / (double)loopcnt;
            StatReport.AppendResponse("Shift-OR FLOAT-64 ET = {0:0.000} mS, nS/loop = {1:0.000}", ET / 10000, TPL * 100);
        }

        // INT16 TESTS 
        private Int16 ReadInt16BEReverse(BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian == true) { Array.Reverse(data); }
            return BitConverter.ToInt16(data, 0);
        }

        private Int16 ReadInt16BEFast(BinaryReader reader)
        {
            Int16 value = reader.ReadInt16();
            if (BitConverter.IsLittleEndian == true)
            {
                return (Int16)((value & 0x00FFU) << 8 |
                              (value & 0xFF00U) >> 8);
            }
            return value;
        }

        private Int16 ReadInt16BEPrim(BinaryReader reader)
        {
            var data = reader.ReadBytes(2);
            return BinaryPrimitives.ReadInt16BigEndian(data);
        }

        // INT32 TESTS 
        private Int32 ReadInt32BEReverse(BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian == true) { Array.Reverse(data); }
            return BitConverter.ToInt32(data, 0);
        }

        private Int32 ReadInt32BEFast(BinaryReader reader)
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

        private Int32 ReadInt32BEPrim(BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            return BinaryPrimitives.ReadInt32BigEndian(data);
        }

        // INT64 TESTS 
        private Int64 ReadInt64BEReverse(BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian == true) { Array.Reverse(data); }
            return BitConverter.ToInt64(data, 0);
        }

        private Int64 ReadInt64BEFast(BinaryReader reader)
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

        public Int64 ReadInt64BEPrim(BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            return BinaryPrimitives.ReadInt64BigEndian(data);
        }


        // float32 (float/single)TESTS
        public float ReadSingleBEReverse(BinaryReader reader)
        {
            var data = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian == true) { Array.Reverse(data); }
            return BitConverter.ToSingle(data, 0);
        }

        public float ReadSingleBEFast(BinaryReader reader)
        {
            Int32 data = ReadInt32BEFast(reader);
            return BitConverter.Int32BitsToSingle(data);
        }





        // float64 (double) TESTS
        public double ReadDoubleBEReverse(BinaryReader reader)
        {
            var data = reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian == true) { Array.Reverse(data); }
            return BitConverter.ToDouble(data, 0);
        }

        public double ReadDoubleBEFast(BinaryReader reader)
        {
            Int64 data = ReadInt64BEFast(reader);
            return BitConverter.Int64BitsToDouble(data);
        }




    }
}
