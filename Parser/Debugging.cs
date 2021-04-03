//MIT License
//
//Copyright(c) 2021 nessie1980(nessie1980 @gmx.de)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.IO;

namespace Parser
{
    internal class Debugging
    {
        #region Properties

        public int ParserInstance { get; set; }

        /// <summary>
        /// Flag for enable or disable of the text file debugging
        /// </summary>
        public bool DebuggingEnabled { set; get; }

        /// <summary>
        /// Fixed file name for the text file debugging
        /// </summary>
        public string DebuggingFileName = @"Debugging_Parser.txt";

        #endregion Properties

        #region Methods

        public Debugging(int instance)
        {
            ParserInstance = instance;
        }

        /// <summary>
        /// This function checks if text file debugging is enabled.
        /// If it is enabled it writes the given message to the text file.
        /// </summary>
        /// <param name="msg">Message which should be written</param>
        /// <param name="append">Flag if the message should be appended or the text file should be cleared</param>
        public void WriteDebuggingMsg(string msg, bool append = true)
        {
            if (!DebuggingEnabled) return;

            DebuggingFileName = $"Debugging_Parser_{ParserInstance}.txt";

            try
            {
                using (var debuggingFile =
                    new StreamWriter(DebuggingFileName, append))

                {
                    const string dateTimeFullFormat = "{0:dd/MM/yyyy HH:mm:ss}";
                    debuggingFile.WriteLine(string.Format(dateTimeFullFormat, DateTime.Now) + @" " + msg);

#if DEBUG
                    Console.WriteLine(string.Format(dateTimeFullFormat, DateTime.Now) + @" Nr: " + ParserInstance +
                                      " Msg: " + msg);
#endif
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion Methods
    }
}
