using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Parser
{
    [Serializable]
    // ReSharper disable once UnusedMember.Global
    public class Parser
    {

        #region Variables

        /// <summary>
        /// This object is used for the thread lock
        /// </summary>
        private readonly object _thisLockStarting = new object();
        private readonly object _thisLockThread = new object();

        /// <summary>
        /// User agent identifier with the default value
        /// </summary>
        private string _userAgentIdentifier = @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0";

        /// <summary>
        /// Parsing values for the parser
        /// </summary>
        private ParsingValues _parsingValues;

        #region Parser states and values

        /// <summary>
        /// Status of the Parser
        /// </summary>
        private ParserState _parserState = ParserState.Idle;

        /// <summary>
        /// Parser error code
        /// </summary>
        private ParserErrorCodes _parserErrorCode = ParserErrorCodes.NoError;

        /// <summary>
        /// State of the Parser as percentage progress
        /// </summary>
        private int _percentageOfProgress;

        /// <summary>
        /// Value of the last parsed regex key
        /// </summary>
        private string _lastParsedRegexListKey = "";

        /// <summary>
        /// Stores the last throw exception
        /// </summary>
        private Exception _lastException;

        #endregion Parser states and values

        #region Parsing result

        /// <summary>
        /// Dictionary with the search result
        /// Key is the regex string
        /// Value is the search result of the regex
        /// </summary>
        private Dictionary<string, List<string>> _marketValuesResult;

        #endregion Parsing result

        #endregion Variables

        #region Properties

        #region Thread

        /// <summary>
        /// This flag starts the thread
        /// </summary>
        public bool ThreadRunning { get; internal set; }

        /// <summary>
        /// Flag if the thread should be canceled
        /// </summary>
        public bool CancelThread { set; get; }

        #endregion Thread

        /// <summary>
        /// User agent identifier with the default value
        /// </summary>
        public string UserAgentIdentifier
        {
            get => _userAgentIdentifier;
            set
            {
                _userAgentIdentifier = value;
                ParserInfoState.UserAgentIdentifier = value;
            }
        }

        #region Given parameters

        /// <summary>
        /// The parsing values variables
        /// </summary>
        public ParsingValues ParsingValues
        {
            get => _parsingValues;
            set => _parsingValues = value;
        }

        #endregion Given parameters

        #region Parser states and values

        /// <summary>
        /// Text for the parsing
        /// </summary>
        public string TextForParsing { get; internal set; }

        /// <summary>
        /// Status of the Parser
        /// </summary>
        public ParserState State
        {
            get => _parserState;
            internal set
            {
                _parserState = value;
                ParserInfoState.State = value;
            }
        }

        /// <summary>
        /// Parser error code
        /// </summary>
        public ParserErrorCodes ParserErrorCode
        {
            get => _parserErrorCode;
            internal set
            {
                _parserErrorCode = value;
                ParserInfoState.LastErrorCode = value;
            }
        }

        /// <summary>
        /// State of the Parser as percentage progress
        /// </summary>
        public int PercentOfProgress
        {
            get => _percentageOfProgress;
            internal set
            {
                _percentageOfProgress = value;
                ParserInfoState.Percentage = value;
            }
        }

        /// <summary>
        /// Current state of the Parser
        /// </summary>
        public ParserInfoState ParserInfoState { get; } = new ParserInfoState();

        /// <summary>
        /// Value of the last parsed regex key
        /// </summary>
        public string LastParsedRegexListKey
        {
            get => _lastParsedRegexListKey;
            internal set
            {
                _lastParsedRegexListKey = value;
                ParserInfoState.LastRegexListKey = value;
            }
        }

        /// <summary>
        /// Stores the last throw exception
        /// </summary>
        public Exception LastException
        {
            get => _lastException;
            internal set
            {
                _lastException = value;
                ParserInfoState.Exception = value;
            }
        }

        #endregion Parser states and values

        #region WebSite download

        /// <summary>
        /// Byte array with the loaded website content
        /// </summary>
        public byte[] WebSiteContentAsByteArray { get; internal set; }

        /// <summary>
        /// String with the loaded website content
        /// </summary>
        public string WebSiteContentAsString { get; internal set; } = "";

        /// <summary>
        /// Byte array with the downloaded data content
        /// </summary>
        public byte[] WebSiteBytesDataContent { get; internal set; }

        /// <summary>
        /// Flag if the download of the website content is complete
        /// </summary>
        public bool WebSiteDownloadComplete { get; internal set; }

        #endregion WebSite download

        #region Parsing result
        
        /// <summary>
        /// Dictionary with the search result
        /// Key is the regex string
        /// Value is the search result of the regex
        /// </summary>
        public Dictionary<string, List<string>> ParsingResult
        {
            get => _marketValuesResult;
            internal set
            {
                _marketValuesResult = value;
                ParserInfoState.SearchResult = value;
            }
        }

        //public List<DailyValues> DailyValuesResult
        //{
        //    get => _dailyValuesResult;
        //    internal set
        //    {
        //        _dailyValuesResult = value;
        //        ParserInfoState.DailyValuesList = value;
        //    }
        //}

        #endregion Parsing result

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Parser()
        {
            var threadParser = new Thread(ThreadFunction)
            {
                IsBackground = true,
                Name = @"Parser"
            };

            TextForParsing = @"";

            threadParser.Start();

            State = ParserState.Idle;
        }

        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="parsingValues">Parsing values for the parsing</param>
        public Parser( ParsingValues parsingValues)
        {
#if _DEBUG_THREADFUNCTION
            Console.WriteLine(@"Parameter constructor");
#endif
            ParsingValues = parsingValues;
        }

        /// <summary>
        /// Function for the parsing thread
        /// First it checks if a parse process could be started
        /// Then it starts the process
        /// - Loading the page source code
        /// - Parsing the page source code
        /// - Signal of process finish
        /// </summary>
        private void ThreadFunction ()
        {
            while (true)
            {
                if (ThreadRunning)
                {
#if _DEBUG_THREADFUNCTION
                    Console.WriteLine(@"ThreadRunning: {0}", ThreadRunning);
#endif
                    try
                    {
                        lock (_thisLockThread)
                        {
                            // Set state
                            State = ParserState.Started;
                            ParserErrorCode = ParserErrorCodes.Started;
                            LastException = null;
                            LastParsedRegexListKey = null;
                            PercentOfProgress = 0;
                            SetAndSendState(ParserInfoState);

                            // Check if thread should be canceled
                            if (CancelThread)
                            {
                                ParserErrorCode = ParserErrorCodes.CancelThread;
                                LastException = null;
                                PercentOfProgress = 0;
                                SetAndSendState(ParserInfoState);
                            }

                            if (ThreadRunning)
                            {
                                // Reset parsing result
                                if (ParsingResult != null && ParsingResult.Count > 0)
                                    ParsingResult.Clear();

                                // Check if thread should be canceled
                                if (CancelThread)
                                {

                                    ParserErrorCode = ParserErrorCodes.CancelThread;
                                    LastException = null;
                                    PercentOfProgress = 0;
                                    SetAndSendState(ParserInfoState);
                                }

                                if (ThreadRunning)
                                {
                                    // Check if a website content should be loaded
                                    if (ParsingValues.ParsingType == ParsingType.WebParsing || ParsingValues.ParsingType == ParsingType.DailyValuesParing)
                                    {
                                        // Set state to loading
                                        State = ParserState.Loading;
                                        ParserErrorCode = ParserErrorCodes.ContentLoadStarted;
                                        LastException = null;
                                        PercentOfProgress = 5;
                                        SetAndSendState(ParserInfoState);

                                        // Create web client with the given or default user agent identifier.
                                        using (var client = new WebClient())
                                        {
                                            try
                                            {

                                                // Browser identifier (e.g. FireFox 36)
                                                client.Headers["User-Agent"] = UserAgentIdentifier;
                                                // Download content as raw data
#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine(@"WebSide: {0}", ParsingValues.WebSiteUrl);
#endif
                                                WebSiteDownloadComplete = false;
                                                client.DownloadProgressChanged += OnWebClientDownloadProgressChanged;
                                                client.DownloadDataCompleted += OnWebClientDownloadDataWebSiteCompleted;
                                                client.DownloadDataAsync(ParsingValues.WebSiteUrl);
                                                while (!WebSiteDownloadComplete || client.IsBusy)
                                                {
                                                    // Check if thread should be canceled
                                                    if (CancelThread)
                                                    {

                                                        ParserErrorCode = ParserErrorCodes.CancelThread;
                                                        LastException = null;
                                                        PercentOfProgress = 0;
                                                        SetAndSendState(ParserInfoState);
                                                        WebSiteDownloadComplete = false;
                                                        client.CancelAsync();
                                                        break;
                                                    }

                                                    Thread.Sleep(10);
                                                }

                                                client.DownloadProgressChanged -= OnWebClientDownloadProgressChanged;
                                                client.DownloadDataCompleted -= OnWebClientDownloadDataWebSiteCompleted;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                throw;
                                            }
                                        }

                                        // Set loaded web site content to parsing text
                                        TextForParsing = WebSiteContentAsString;

                                        // Check if the website content load was successful and call event
                                        if (WebSiteContentAsString == @"")
                                        {
                                            ParserErrorCode = ParserErrorCodes.NoWebContentLoaded;
                                            LastException = null;
                                            PercentOfProgress = 0;
                                            SetAndSendState(ParserInfoState);
                                        }
                                        else
                                        {
                                            ParserErrorCode = ParserErrorCodes.ContentLoadFinished;
                                            LastException = null;
                                            PercentOfProgress = 10;
                                            SetAndSendState(ParserInfoState);
                                        }

                                        // Check if thread should be canceled
                                        if (CancelThread)
                                        {
                                            ParserErrorCode = ParserErrorCodes.CancelThread;
                                            LastException = null;
                                            PercentOfProgress = 0;
                                            SetAndSendState(ParserInfoState);
                                        }
                                    }

                                    if (ThreadRunning)
                                    {
                                        if (ParsingValues.ParsingType == ParsingType.WebParsing)
                                        {
                                            // Set state to parsing
                                            State = ParserState.Parsing;
                                            ParserErrorCode = ParserErrorCodes.SearchStarted;
                                            LastException = null;
                                            PercentOfProgress = 15;
                                            SetAndSendState(ParserInfoState);

                                            var statusValueStep = (100 - 15) / ParsingValues.RegexList.RegexListDictionary.Count;
                                            var statusValue = 15;
#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine("Parsing-Step: {0}", statusValueStep);
#endif

                                            // Loop through the dictionary and fill the result in the result list
                                            foreach (var regexExpression in ParsingValues.RegexList.RegexListDictionary)
                                            {
                                                // Check if thread should be canceled
                                                if (CancelThread)
                                                {
                                                    ParserErrorCode = ParserErrorCodes.CancelThread;
                                                    LastException = null;
                                                    PercentOfProgress = 0;
                                                    SetAndSendState(ParserInfoState);
                                                    break;
                                                }

                                                // Set last regex key
                                                LastParsedRegexListKey = regexExpression.Key;

#if _DEBUG_THREADFUNCTION
                                                Console.WriteLine("Key: {0}", regexExpression.Key);
#endif
                                                var regexElement = regexExpression.Value;

#if _DEBUG_THREADFUNCTION
                                                Console.WriteLine("RegexString: {0}", regexElement.RegexExpression);
#endif
                                                // Build the regex options
                                                var tmpRegexOptionsList = regexElement.RegexOptions;
                                                var tmpRegexOptions = RegexOptions.None;

                                                if (tmpRegexOptionsList != null && tmpRegexOptionsList.Count > 0)
                                                {
                                                    foreach (var regexOption in tmpRegexOptionsList)
                                                    {
                                                        tmpRegexOptions |= regexOption;
                                                    }
                                                }

#if _DEBUG_THREADFUNCTION
                                                Console.WriteLine("RegexOptionSet: {0}", tmpRegexOptions);
                                                Console.WriteLine("RegexExpression: {0}",
                                                    regexExpression.Value.RegexExpression);
#endif

                                                // Search for the value
                                                var added = false;
                                                var matchCollection = Regex.Matches(TextForParsing,
                                                    regexExpression.Value.RegexExpression, tmpRegexOptions);

                                                // Add the parsing result if a result has been found
                                                if (regexExpression.Value.RegexFoundPosition < matchCollection.Count)
                                                {
                                                    if (ParsingResult == null)
                                                    {
                                                        ParsingResult = new Dictionary<string, List<string>>();
                                                    }

                                                    var listResults = new List<string>();

                                                    // If a specific search result should be taken or all results (RegexFoundPosition == -1)
                                                    if (regexExpression.Value.RegexFoundPosition >= 0)
                                                    {
#if _DEBUG_THREADFUNCTION
                                                        Console.WriteLine(@"Value: '{0}' = '{1}'", regexExpression.Key,
                                                            matchCollection[regexExpression.Value.RegexFoundPosition]
                                                                .Groups[1].Value);
#endif
                                                        for (var i = 1;
                                                            i < matchCollection[
                                                                regexExpression.Value.RegexFoundPosition].Groups.Count;
                                                            i++)
                                                        {
                                                            // Check if thread should be canceled
                                                            if (CancelThread)
                                                            {
                                                                ParserErrorCode = ParserErrorCodes.CancelThread;
                                                                LastException = null;
                                                                PercentOfProgress = 0;
                                                                SetAndSendState(ParserInfoState);
                                                            }

                                                            // Check if the result is empty
                                                            if (matchCollection[
                                                                        regexExpression.Value.RegexFoundPosition]
                                                                    .Groups[i].Value == "") continue;

                                                            listResults.Add(
                                                                matchCollection[
                                                                        regexExpression.Value.RegexFoundPosition]
                                                                    .Groups[i]
                                                                    .Value);
                                                            i = matchCollection[
                                                                regexExpression.Value.RegexFoundPosition].Groups.Count;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        foreach (Match match in matchCollection)
                                                        {
                                                            // Check if thread should be canceled
                                                            if (CancelThread)
                                                            {
                                                                ParserErrorCode = ParserErrorCodes.CancelThread;
                                                                LastException = null;
                                                                PercentOfProgress = 0;
                                                                SetAndSendState(ParserInfoState);
                                                            }
#if _DEBUG_THREADFUNCTION
                                                            Console.WriteLine(@"Value: '{0}' = '{1}'",
                                                                regexExpression.Key, match.Groups[1].Value);
#endif
                                                            for (var i = 1; i < match.Groups.Count; i++)
                                                            {
                                                                // Check if thread should be canceled
                                                                if (CancelThread)
                                                                {
                                                                    ParserErrorCode = ParserErrorCodes.CancelThread;
                                                                    LastException = null;
                                                                    PercentOfProgress = 0;
                                                                    SetAndSendState(ParserInfoState);
                                                                }

                                                                if (match.Groups[i].Value != "")
                                                                {
                                                                    listResults.Add(match.Groups[i].Value);
//                                                                    i = match.Groups.Count;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    ParsingResult.Add(regexExpression.Key, listResults);
                                                    added = true;
                                                }

                                                // Check if no result has been found or is not added and the result can not be empty
                                                if ((matchCollection.Count == 0 || added == false) &&
                                                    !regexElement.ResultEmpty)
                                                {
#if _DEBUG_THREADFUNCTION
                                                    Console.WriteLine(@"No MATCH found!");
#endif
                                                    ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                    LastException = null;
                                                    PercentOfProgress = 0;
                                                    SetAndSendState(ParserInfoState);
                                                    break;
                                                }

                                                statusValue += statusValueStep;

                                                if (statusValue < 100)
                                                {
                                                    ParserErrorCode = ParserErrorCodes.SearchRunning;
                                                    LastException = null;
                                                    PercentOfProgress = statusValue;
                                                    SetAndSendState(ParserInfoState);
                                                }
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                ParserErrorCode = ParserErrorCodes.SearchFinished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {
                                                ParserErrorCode = ParserErrorCodes.CancelThread;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                // Signal that the thread has finished
                                                ParserErrorCode = ParserErrorCodes.Finished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }
                                        }

                                        if (ParsingValues.ParsingType == ParsingType.DailyValuesParing)
                                        {
                                            if (TextForParsing == @"")
                                            {
                                                ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                                break;
                                            }

                                            // Split loaded CSV into an array which contains the lines
                                            var lines = TextForParsing.Split().ToList();

                                            // Remove CSV header content
                                            if (lines.Count > 0)
                                                lines.RemoveAt(0);

                                            // Remove empty lines
                                            for (var i = 0; i < lines.Count; i++)
                                            {
                                                if (lines[i] != @"") continue;

                                                lines.RemoveAt(i);
                                                i--;
                                            }

                                            // Check if the content contains lines
                                            if (lines.Count == 0)
                                            {
                                                ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                                break;
                                            }

                                            // Calculate the progress bar step value
                                            var stateCountValueStep = (100.0f - (double)PercentOfProgress) / lines.Count;
                                            var stateValue = (double)PercentOfProgress;

                                            // Split lines into values
                                            foreach (var line in lines)
                                            {
                                                var values = line.Split(';');

                                                if(values.Length != 6)
                                                {
                                                    ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                    LastException = null;
                                                    PercentOfProgress = 0;
                                                    SetAndSendState(ParserInfoState);
                                                    break;
                                                }

                                                var iCounter = 0;
                                                var dailyValues = new DailyValues();

                                                // Set values to the daily values object
                                                foreach (var value in values)
                                                {
                                                    switch (iCounter)
                                                    {
                                                        case 0:
                                                            dailyValues.Date = DateTime.Parse(value);
                                                            break;
                                                        case 1:
                                                            dailyValues.OpeningPrice = decimal.Parse(value);
                                                            break;
                                                        case 2:
                                                            dailyValues.Top = decimal.Parse(value);
                                                            break;
                                                        case 3:
                                                            dailyValues.Bottom = decimal.Parse(value);
                                                            break;
                                                        case 4:
                                                            dailyValues.ClosingPrice = decimal.Parse(value);
                                                            break;
                                                        case 5:
                                                            dailyValues.Volume = decimal.Parse(value);
                                                            break;
                                                    }

                                                    iCounter++;
                                                }

                                                // Check if the DailyValuesResult list is already created
                                                if (ParserInfoState.DailyValuesList == null)
                                                    ParserInfoState.DailyValuesList = new List<DailyValues>();

                                                // Add new daily values to the list
                                                ParserInfoState.DailyValuesList.Add(dailyValues);

                                                // Increase state
                                                stateValue += stateCountValueStep;

                                                if (stateValue < 100)
                                                {
                                                    ParserErrorCode = ParserErrorCodes.SearchRunning;
                                                    LastException = null;
                                                    PercentOfProgress = (int)stateValue;
                                                    SetAndSendState(ParserInfoState);
                                                }
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                ParserErrorCode = ParserErrorCodes.SearchFinished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {
                                                ParserErrorCode = ParserErrorCodes.CancelThread;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                // Signal that the thread has finished
                                                ParserErrorCode = ParserErrorCodes.Finished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (WebException webEx)
                    {
                        // Set state
                        State = ParserState.Idle;
                        ParserErrorCode = ParserErrorCodes.WebExceptionOccured;
                        LastException = webEx;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                    }
                    catch (Exception ex)
                    {
                        // Set state
                        State = ParserState.Idle;
                        ParserErrorCode = ParserErrorCodes.ExceptionOccured;
                        LastException = ex;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                    }
                }

                Thread.Sleep(10);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// This function sets the new percentage value to the class variable
        /// </summary>
        /// <param name="sender">Web client which download the website content asynchronous</param>
        /// <param name="e">DownloadProgressChangedEventArgs with the result</param>
        public void OnWebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ParserInfoState.PercentageDownload = e.ProgressPercentage;
        }

        /// <summary>
        /// This function sets the downloaded website content to the class variable
        /// </summary>
        /// <param name="sender">Web client which download the website content asynchronous</param>
        /// <param name="e">DownloadDataCompletedEventArgs with the result</param>
        public void OnWebClientDownloadDataWebSiteCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
#if _DEBUG_THREADFUNCTION
                Console.WriteLine(@"OnWebClientDownloadDataWebSiteCompleted");
#endif

                if (e.Error != null || e.Cancelled) return;

#if _DEBUG_THREADFUNCTION
                Console.WriteLine(@"e.Result.LongLength: {0}", e.Result.LongLength);
#endif
                if (e.Result.LongLength > 0)
                {
                    WebSiteContentAsByteArray = new byte[e.Result.LongLength];
                    ParserInfoState.WebSiteContentAsByteArray = new byte[e.Result.Length];

                    WebSiteContentAsString = Encoding.UTF8.GetString(e.Result);
                    // Copy loaded content string to the parser info state
                    ParserInfoState.WebSiteContentAsString = WebSiteContentAsString;

                    Buffer.BlockCopy(e.Result, 0, WebSiteContentAsByteArray, 0, e.Result.Length);
                    // Copy loaded content byte array to the parser info state
                    Buffer.BlockCopy(WebSiteContentAsByteArray, 0, ParserInfoState.WebSiteContentAsByteArray, 0, WebSiteContentAsByteArray.Length);
                }
                else
                {
                    WebSiteContentAsString = "";
                    // Copy loaded content string to the parser info state
                    ParserInfoState.WebSiteContentAsString = WebSiteContentAsString;

                    WebSiteContentAsByteArray = null;
                    ParserInfoState.WebSiteContentAsByteArray = null;
                }

                ParserInfoState.PercentageDownload = 100;
                WebSiteDownloadComplete = true;
            }
            catch (WebException webEx)
            {
                // Set state
                State = ParserState.Idle;
                ParserErrorCode = ParserErrorCodes.WebExceptionOccured;
                LastException = webEx;
                PercentOfProgress = 0;
                SetAndSendState(ParserInfoState);
            }
        }

        /// <summary>
        /// This function starts the parsing process.
        /// </summary>
        /// <returns>Start process return code</returns>
        // ReSharper disable once UnusedMember.Global
        public bool StartParsing()
        {
            try
            {
                lock (_thisLockStarting)
                {
                    // Reset parsing result
                    if (ParsingResult != null)
                    {
                        ParsingResult?.Clear();
                        ParsingResult = null;
                    }

                    // Reset daily values list result
                    if (ParserInfoState?.DailyValuesList != null)
                    {
                        ParserInfoState.DailyValuesList.Clear();
                        ParserInfoState.DailyValuesList = null;
                    }

                    // Send start event to the GUI
                    ParserErrorCode = ParserErrorCodes.Starting;
                    LastException = null;
                    PercentOfProgress = 0;
                    SetAndSendState(ParserInfoState);

                    // Check if a new parsing could be started
                    if (State != ParserState.Idle)
                    {
                        ParserErrorCode = ParserErrorCodes.BusyFailed;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    // Check if the given web address is not invalid and a web parsing should be done
                    if (ParsingValues.WebSiteUrl == null && (ParsingValues.ParsingType == ParsingType.WebParsing || ParsingValues.ParsingType == ParsingType.DailyValuesParing))
                    {
                        ParserErrorCode = ParserErrorCodes.InvalidWebSiteGiven;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    if ((ParsingValues.RegexList == null || ParsingValues.RegexList.RegexListDictionary.Count == 0) && ParsingValues.ParsingType == ParsingType.WebParsing)
                    {
                        ParserErrorCode = ParserErrorCodes.NoRegexListGiven;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }
                }

                ThreadRunning = true;

                return true;
            }
            catch (Exception ex)
            {
                // Set state
                State = ParserState.Idle;
                ParserErrorCode = ParserErrorCodes.ExceptionOccured;
                LastException = ex;
                PercentOfProgress = 0;
                SetAndSendState(ParserInfoState);
                return false;
            }
        }

        /// <summary>
        /// This function sets the current info state and sends
        /// the state to the GUI
        /// </summary>
        /// <param name="parserInfoState">ParserInfoState</param>
        private void SetAndSendState(ParserInfoState parserInfoState)
        {
#if DEBUG
            Console.WriteLine(@"State: {0} / ThreadRunning: {1} / ErrorCode: {2} / PercentOfProgress: {3}", State, ThreadRunning, parserInfoState.LastErrorCode, parserInfoState.Percentage);

            if (parserInfoState.Exception != null)
                Console.WriteLine(@"Exception: {0}", parserInfoState.Exception.Message);
#endif
            // Set state to "idle"
            if (parserInfoState.LastErrorCode == ParserErrorCodes.Finished || parserInfoState.LastErrorCode < 0)
            {
                State = ParserState.Idle;
                CancelThread = false;
            }

            // Send state
            //if (ThreadRunning)
            OnParserUpdate?.Invoke(this, new OnParserUpdateEventArgs(parserInfoState));

            // Stop thread
            if (parserInfoState.LastErrorCode == ParserErrorCodes.Finished || parserInfoState.LastErrorCode < 0)
            {
                ThreadRunning = false;
            }       
        }

        #endregion Methodes

        #region Events / Delegates

        public delegate void ParserUpdateEventHandler(object sender, OnParserUpdateEventArgs e);

        public event ParserUpdateEventHandler OnParserUpdate;

        #endregion Events / Delegates
    }

    public class ParsingValues
    {
        #region Given parameters

        /// <summary>
        /// String for the website url which should be parsed
        /// </summary>
        private Uri _webSiteUrl;

        #endregion Given parameters

        #region Properties

        /// <summary>
        /// Type for the parsing
        /// WebParsing: A given url will be loaded and the website content will be parsed by the given regex list
        /// TextParsing: A given text will be parsed by the given regex list
        /// DailyValuesParing: A given url will be loaded and the website content will be parsed for the daily values
        /// </summary>
        public ParsingType ParsingType { get; set; }

        /// <summary>
        /// Encoding type for the download content
        /// </summary>
        public string EncodingType { get; internal set; }

        /// <summary>
        /// Uri for the website url which should be parsed
        /// </summary>
        public Uri WebSiteUrl
        {
            get => _webSiteUrl;
            internal set
            {
                try
                {
                    // Check if the website url is valid
                    //                String regexPattern = @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
                    //                if (System.Text.RegularExpressions.Regex.IsMatch(value, regexPattern))
                    if (value.ToString() != @"")
                    {
                        var uriWebSite = value;
                        var uriHostNameType = uriWebSite.HostNameType;
                        //var strDNSName = uriWebSite.DnsSafeHost;
                        var strWebSiteScheme = uriWebSite.Scheme;
                        var isWllFormedUriStringFlag = Uri.IsWellFormedUriString(value.ToString(), UriKind.Absolute);
                        if (isWllFormedUriStringFlag &&
                            (strWebSiteScheme == Uri.UriSchemeHttp || strWebSiteScheme == Uri.UriSchemeHttps) &&
                            (uriHostNameType == UriHostNameType.Dns || uriHostNameType == UriHostNameType.IPv4 || uriHostNameType == UriHostNameType.IPv6)
                        )
                        {
                            _webSiteUrl = new Uri(!Regex.IsMatch(value.ToString(), @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?") ? @"invalid" : value.ToString());
                        }
                        else
                            _webSiteUrl = null;
                    }
                    else
                        _webSiteUrl = null;
                }
                catch
                {
                    _webSiteUrl = null;
                }
            }
        }

        /// <summary>
        /// String for the parsing
        /// </summary>
        public string ParsingText { get; set; }

        /// <summary>
        /// List with the regex string for the parsing
        /// </summary>
        public RegExList RegexList { get; internal set; }

        #endregion Properties

        /// <summary>
        /// Parsing values with a text which should be parsed
        /// </summary>
        /// <param name="parsingText">Text which should be parsed</param>
        /// <param name="encoding">Encoding of the text</param>
        /// <param name="regexList">Regex list for the parsing</param>
        public ParsingValues(string parsingText, string encoding, RegExList regexList)
        {
            ParsingType = ParsingType.TextParing;
            WebSiteUrl = null;
            ParsingText = parsingText;
            EncodingType = encoding;
            RegexList = regexList;
        }

        /// <summary>
        /// Parsing values with a URL for where the text which should be parsed should be loaded
        /// </summary>
        /// <param name="webSiteUrl">URL form where the text which should be parsed should be loaded</param>
        /// <param name="encoding">Encoding of the URL website</param>
        /// <param name="regexList">Regex list for the parsing</param>
        public ParsingValues(Uri webSiteUrl, string encoding, RegExList regexList)
        {
            ParsingType = ParsingType.WebParsing;
            WebSiteUrl = webSiteUrl;
            ParsingText = null;
            EncodingType = encoding;
            RegexList = regexList;
        }

        /// <summary>
        /// Parsing values with a URL for where the text which should be parsed for daily values
        /// </summary>
        /// <param name="webSiteUrl">URL from where the text which should be parsed should be loaded</param>
        /// <param name="encoding">Encoding of the URL website</param>
        public ParsingValues(Uri webSiteUrl, string encoding)
        {
            ParsingType = ParsingType.DailyValuesParing;
            WebSiteUrl = webSiteUrl;
            ParsingText = null;
            EncodingType = encoding;
            RegexList = null;
        }
    }

    /// <summary>
    /// Enum for the Parsing type
    /// </summary>
    public enum ParsingType
    {
        WebParsing,
        TextParing,
        DailyValuesParing
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
        CancelThread = - 7,
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
        public List<DailyValues>DailyValuesList { get; internal set; }

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
        public OnParserUpdateEventArgs (ParserInfoState parserInfoState)
	    {
            ParserInfoState = parserInfoState;
        }

        #endregion Methodes
    }
}
