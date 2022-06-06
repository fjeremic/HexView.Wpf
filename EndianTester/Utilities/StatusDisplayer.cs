using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EndianTester.Utilities
{

    public class StatusDisplayer
    {

       // NOTICE: Textbox must have UndoLimit="0" set else performace and memory usage will suffer greatly!

        private TextBox textBox;
        public StatusDisplayer(TextBox tb)
        {
            textBox = tb;
            startStatusUpdateTimer();
        }

        // this constructor allows initial declaration and reference to the displayer 
        // without the textbox (which will be supplied after component initializaton)
        public StatusDisplayer()
        {
        }

        #region Message Update Timer

        private DispatcherTimer statusUpdateTimer = new DispatcherTimer();
        private const long statusUpdateMs = 100;

        private void startStatusUpdateTimer()
        {
            //Start the status message timer running
            statusUpdateTimer.Tick += new EventHandler(statusUpdateTimer_Tick);
            statusUpdateTimer.Interval = new TimeSpan(statusUpdateMs * 10000);
            statusUpdateTimer.Start();
        }

        // timer routine to copy current text into text box
        // handle null textbox that can occur during project initialization
        private void statusUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (textBox == null) return;

            // update text in error text box
            // may need to do this in a background thread using invoke
            if (statusTextUpdate)
            {
                lock (sbStatus)
                {
                    // some question as to how efficient the AppendText is.  May need to limit size
                    textBox.AppendText(sbStatus.ToString());
                    textBox.ScrollToEnd();
                    sbStatus.Clear();
                    statusTextUpdate = false;
                }
            }
        }

        public void ClearStatusText()
        {
            lock (sbStatus)
            {
                sbStatus.Clear();
                if (textBox != null)
                    textBox.Clear();
            }
        }

        #endregion

        #region Status Text Updates

        // function to append text into response box
        // will queue up text and re-draw occasionally
        private bool statusTextEnabled = true;
        private bool statusTextUpdate = false;
        private const int maxResponseLen = 10000000;
        private StringBuilder sbStatus = new StringBuilder(maxResponseLen);

        public void AppendResponse(string txt, params object[] args)
        {
            if (statusTextEnabled)
            {
                sbStatus.AppendLine(string.Format(txt, args));
                if (sbStatus.Length > maxResponseLen)
                    sbStatus.Remove(0, sbStatus.Length - maxResponseLen);
                statusTextUpdate = true;
            }
        }

        //version without formatting to allow HEX dump
        public void AppendResponse(string txt)
        {
            if (statusTextEnabled)
            {
                sbStatus.AppendLine(txt);
                if (sbStatus.Length > maxResponseLen)
                    sbStatus.Remove(0, sbStatus.Length - maxResponseLen);
                statusTextUpdate = true;
            }
        }

        //version that doen not add CRLF
        public void AppendResponseNoLine(string txt)
        {
            if (statusTextEnabled)
            {
                sbStatus.Append(txt);
                if (sbStatus.Length > maxResponseLen)
                    sbStatus.Remove(0, sbStatus.Length - maxResponseLen);
                statusTextUpdate = true;
            }
        }

        //version for displaying a HEX dump  TODO:  Add ASCII text to each line
        public void AppendResponseHex(IList<byte> bytes, int bytesPerLine = 16, bool addLineNumbers = false, long lineNumStartValue = 0, bool ShowAscii = false)
        {
            if (statusTextEnabled == false) { return; }

            //convert IList to byte array for parsing
            byte[] data = bytes.ToArray();

            // determine location of ASCII field
            int ascPad;
            if (addLineNumbers == true)
                ascPad = (bytesPerLine * 3) + 10;
            else
                ascPad = (bytesPerLine * 3) + 4;

            // determine max line number & set field format:
            long topAddr = bytes.Count + lineNumStartValue - 1;
            string addrFmt = "X8";
            if (topAddr <= 0xFFFF)
                addrFmt = "X4";
            else if (topAddr <= 0xFFFFFF)
                addrFmt = "X6";

            // parse one line at a time...
            for (int i = 0; i < data.Length; i += bytesPerLine)
            {
                int sz = ((i + bytesPerLine) < data.Length) ? bytesPerLine : (data.Length - i);
                string hex = BitConverter.ToString(data, i, sz).Replace("-", " ");
                string asc = GetSanitizedString(data, i, sz);
                long addr = i + lineNumStartValue;
                string line;

                // add optional line numbers
                if (addLineNumbers == true)
                    line = string.Format("{0}: {1}", addr.ToString(addrFmt), hex);
                else
                    line = hex;

                // add (optional) ascii field:
                if (ShowAscii == true)
                    sbStatus.AppendFormat("{0}{1}{2}", line.PadRight(ascPad), asc, Environment.NewLine);
                else
                    sbStatus.AppendFormat("{0}{1}", line, Environment.NewLine);
            }
            
            // make sure there is no space and only one NewLine at the end
            if (sbStatus[sbStatus.Length - 1].ToString() == " ") { sbStatus.Length--; }
            if (sbStatus[sbStatus.Length - 1].ToString() != Environment.NewLine) { sbStatus.Append(Environment.NewLine); }

            // set update needed flag
            statusTextUpdate = true;

        }


        private string GetSanitizedString(IList<byte> data, int startIndex, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                byte b = data[i + startIndex];
                if ((b > 0x1F) && (b < 0x7F))
                    sb.Append(System.Text.Encoding.ASCII.GetString(new[] { b }));
                else
                    sb.Append(".");
            }
            return sb.ToString();

        }
        #endregion


    }

}
