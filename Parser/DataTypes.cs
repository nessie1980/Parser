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
using System.Collections.Generic;

namespace Parser
{
    public static class DataTypes
    {
        /// <summary>
        /// Enum for the load type of the parsing content
        /// </summary>
        public enum LoadType
        {
            Web,    // Load content from web
            Text    // Given text by this class
        };

        /// <summary>
        /// Enum for the Parsing type
        /// </summary>
        public enum ParsingType
        {
            Regex,                  // Regex parsing of the content via the given regex list
            OnVistaRealTime,        // JSON parsing of the real time value of the OnVista finance API
            OnVistaHistoryData,     // JSON parsing of the daily values of the OnVista finance API
            YahooRealTime,          // JSON parsing of the real time value of the Yahoo finance API
            YahooHistoryData        // JSON parsing of the daily values of the Yahoo finance API
        };

        /// <summary>
        /// Enum for the Parser state
        /// </summary>
        public enum ParserState
        {
            Idle,
            Started,
            Loading,
            Parsing
        };

        /// <summary>
        /// ErrorCodes
        /// </summary>
        public enum ParserErrorCodes
        {
            Finished = 8,
            SearchFinished = 7,
            SearchRunning = 6,
            SearchStarted = 5,
            ContentLoadFinished = 4,
            ContentLoadStarted = 3,
            Started = 2,
            Starting = 1,
            NoError = 0,
            StartFailed = -1,
            BusyFailed = -2,
            InvalidWebSiteGiven = -3,
            NoRegexListGiven = -4,
            NoWebContentLoaded = -5,
            ParsingFailed = -6,
            CancelThread = -7,
            WebExceptionOccurred = -8,
            FileExceptionOccurred = -9,
            JsonExceptionOccurred = -10,
            ExceptionOccurred = -11
        }

        /// <summary>
        /// Class of the current info state of the Parser
        /// </summary>
        [Serializable]
        public class ParserInfoState
        {
            #region Properties

            /// <summary>
            /// Current url of the Parser
            /// </summary>
            public string WebSite { get; internal set; }

            /// <summary>
            /// Current user agent identifier of the Parser
            /// </summary>
            public string UserAgentIdentifier { get; internal set; }

            /// <summary>
            /// Current state of the Parser
            /// </summary>
            public ParserState State { get; internal set; }

            /// <summary>
            /// Percentage of the update process
            /// </summary>
            public int Percentage { get; internal set; }

            /// <summary>
            /// Percentage of the download process of the website or data content
            /// </summary>
            public int PercentageDownload { get; internal set; }

            /// <summary>
            /// Loaded website content as byte array
            /// </summary>
            public byte[] WebSiteContentAsByteArray { get; internal set; }

            /// <summary>
            /// Loaded website content as string
            /// </summary>
            public string WebSiteContentAsString { get; internal set; } = "";

            /// <summary>
            /// Last error code of the Parser
            /// </summary>
            public ParserErrorCodes LastErrorCode { get; internal set; }

            /// <summary>
            /// Current regular expression list of the Parser
            /// </summary>
            public RegExList RegexList { get; internal set; }

            /// <summary>
            /// Last _regexList key
            /// </summary>
            public string LastRegexListKey { get; internal set; }

            /// <summary>
            /// Dictionary with the current search result of the Parser
            /// Key is the regex string
            /// Value is the search result of the regex
            /// </summary>
            public Dictionary<string, List<string>> SearchResult { get; internal set; }

            /// <summary>
            /// List with current search result of the Parser
            /// </summary>
            public List<DailyValues> DailyValuesList { get; internal set; }

            /// <summary>
            /// Exception if an exception occurred
            /// </summary>
            public Exception Exception { get; internal set; }

            #endregion Properties

            /// <summary>
            /// Constructor
            /// </summary>
            public ParserInfoState()
            {
                DailyValuesList = new List<DailyValues>();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Class of the EventArgs of the event OnWebSiteLoadFinished
        /// </summary>
        [Serializable]
        public class OnParserUpdateEventArgs : EventArgs
        {
            #region Properties

            public ParserInfoState ParserInfoState { get; }

            #endregion Properties

            #region Methodes

            /// <inheritdoc />
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parserInfoState">Last error code of the Parser</param>
            /// Error code see the class "Parser"
            public OnParserUpdateEventArgs(ParserInfoState parserInfoState)
            {
                ParserInfoState = parserInfoState;
            }

            #endregion Methodes
        }
    }
}
