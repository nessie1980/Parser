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
