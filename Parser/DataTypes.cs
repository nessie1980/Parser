using System;
using System.Collections.Generic;

namespace Parser
{
    public static class DataTypes
    {
        /// <summary>
        /// Enum for the Parsing type
        /// </summary>
        public enum ParsingType
        {
            WebParsing,
            TextParsing,
            DailyValuesParsing
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
            WebExceptionOccured = -8,
            ExceptionOccured = -9
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
