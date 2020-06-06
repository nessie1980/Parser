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
        /// Counter of the created share objects
        /// </summary>
        private static int _iObjectCounter;

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
        private DataTypes.ParserState _parserState = DataTypes.ParserState.Idle;

        /// <summary>
        /// Parser error code
        /// </summary>
        private DataTypes.ParserErrorCodes _parserErrorCode = DataTypes.ParserErrorCodes.NoError;

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
        /// Debugger class
        /// </summary>
        private Debugging _debugger;

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
            set
            {
                _userAgentIdentifier = value;
                ParserInfoState.UserAgentIdentifier = value;
            }
            get => _userAgentIdentifier;
        }

        #region Given parameters

        /// <summary>
        /// The parsing values variables
        /// </summary>
        public ParsingValues ParsingValues
        {
            set
            {
                _parsingValues = value;

                _debugger.WriteDebuggingMsg(@"ParsingValues.WebSiteUrl: " + _parsingValues.WebSiteUrl);
                _debugger.WriteDebuggingMsg(@"ParsingValues.EncodingType: " + _parsingValues.EncodingType);
                _debugger.WriteDebuggingMsg(@"ParsingValues.ParsingType: " + _parsingValues.ParsingType);
                _debugger.WriteDebuggingMsg(@"ParsingValues.ParsingText: " + _parsingValues.ParsingText);

                if (_parsingValues.RegexList != null)
                {
                    foreach (var element in _parsingValues.RegexList.RegexListDictionary)
                        _debugger.WriteDebuggingMsg(@"Regex- Key: " + element.Key + " / Value: " + element.Value);
                }
            }
            get => _parsingValues;
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
        public DataTypes.ParserState State
        {
            internal set
            {
                _parserState = value;
                ParserInfoState.State = value;
            }
            get => _parserState;
        }

        /// <summary>
        /// Parser error code
        /// </summary>
        public DataTypes.ParserErrorCodes ParserErrorCode
        {
            internal set
            {
                _parserErrorCode = value;
                ParserInfoState.LastErrorCode = value;
            }
            get => _parserErrorCode;
        }

        /// <summary>
        /// State of the Parser as percentage progress
        /// </summary>
        public int PercentOfProgress
        {
            internal set
            {
                _percentageOfProgress = value;
                ParserInfoState.Percentage = value;
            }
            get => _percentageOfProgress;
        }

        /// <summary>
        /// Current state of the Parser
        /// </summary>
        public DataTypes.ParserInfoState ParserInfoState { get; } = new DataTypes.ParserInfoState();

        /// <summary>
        /// Value of the last parsed regex key
        /// </summary>
        public string LastParsedRegexListKey
        {
            internal set
            {
                _lastParsedRegexListKey = value;
                ParserInfoState.LastRegexListKey = value;
            }
            get => _lastParsedRegexListKey;
        }

        /// <summary>
        /// Stores the last throw exception
        /// </summary>
        public Exception LastException
        {
            internal set
            {
                _lastException = value;
                ParserInfoState.Exception = value;
            }
            get => _lastException;
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
            internal set
            {
                _marketValuesResult = value;
                ParserInfoState.SearchResult = value;
            }
            get => _marketValuesResult;
        }

        #endregion Parsing result

        #region Parser debugging

        /// <summary>
        /// Flag if the text file logging of the parser is enabled
        /// </summary>
        public bool DebuggingEnabled
        {
            internal set => _debugger.DebuggingEnabled = value;
            get => _debugger.DebuggingEnabled;
        }

        /// <summary>
        /// File name for the text file debugging
        /// </summary>
        public string DebuggingFileName => _debugger.DebuggingFileName;

        #endregion Parser debugging

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="debuggingEnable">Flag if the text file debugging should be enabled</param>
        public Parser(bool debuggingEnable = false)
        {
            _iObjectCounter++;

            // Create debugger instance
            if (_debugger == null)
                _debugger = new Debugging(_iObjectCounter);

            // Text file debugging
            DebuggingEnabled = debuggingEnable;

            var threadParser = new Thread(ThreadFunction)
            {
                IsBackground = true,
                Name = @"Parser"
            };

            TextForParsing = @"";

            threadParser.Start();

            State = DataTypes.ParserState.Idle;

            _debugger.WriteDebuggingMsg(@"Constructor", false);
            _debugger.WriteDebuggingMsg(@"Constructor 2");
        }

        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="parsingValues">Parsing values for the parsing</param>
        /// <param name="debuggingEnable">Flag if the debugging should be enabled</param>
        public Parser( ParsingValues parsingValues, bool debuggingEnable = false)
        {
            // Set debugging values
            DebuggingEnabled = debuggingEnable;

            _iObjectCounter++;

            // Create debugger instance
            if (_debugger == null)
                _debugger = new Debugging(_iObjectCounter);

            ParsingValues = parsingValues;

            _debugger.WriteDebuggingMsg(@"Parameter constructor", false);
            _debugger.WriteDebuggingMsg(@"Parameter constructor 2");
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
                    _debugger.WriteDebuggingMsg($@"ThreadRunning: {ThreadRunning}");

                    try
                    {
                        lock (_thisLockThread)
                        {
                            // Set state
                            State = DataTypes.ParserState.Started;
                            ParserErrorCode = DataTypes.ParserErrorCodes.Started;
                            LastException = null;
                            LastParsedRegexListKey = null;
                            PercentOfProgress = 0;
                            SetAndSendState(ParserInfoState);

                            // Check if thread should be canceled
                            if (CancelThread)
                            {
                                ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
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

                                    ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                    LastException = null;
                                    PercentOfProgress = 0;
                                    SetAndSendState(ParserInfoState);
                                }

                                if (ThreadRunning)
                                {
                                    // Check if a website content should be loaded
                                    if (ParsingValues.ParsingType == DataTypes.ParsingType.WebParsing || ParsingValues.ParsingType == DataTypes.ParsingType.DailyValuesParsing)
                                    {
                                        // Set state to loading
                                        State = DataTypes.ParserState.Loading;
                                        ParserErrorCode = DataTypes.ParserErrorCodes.ContentLoadStarted;
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
                                                _debugger.WriteDebuggingMsg($@"WebSiteUrl: {ParsingValues.WebSiteUrl}");
                                                WebSiteDownloadComplete = false;
                                                client.DownloadProgressChanged += OnWebClientDownloadProgressChanged;
                                                client.DownloadDataCompleted += OnWebClientDownloadDataWebSiteCompleted;
                                                client.DownloadDataAsync(ParsingValues.WebSiteUrl);
                                                while (!WebSiteDownloadComplete || client.IsBusy)
                                                {
                                                    // Check if thread should be canceled
                                                    if (CancelThread)
                                                    {

                                                        ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
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
                                                _debugger.WriteDebuggingMsg($@"Exception: {e.Message}");
                                                throw;
                                            }
                                        }

                                        // Set loaded web site content to parsing text
                                        TextForParsing = WebSiteContentAsString;

                                        // Check if the website content load was successful and call event
                                        if (WebSiteContentAsString == @"")
                                        {
                                            ParserErrorCode = DataTypes.ParserErrorCodes.NoWebContentLoaded;
                                            LastException = null;
                                            PercentOfProgress = 0;
                                            SetAndSendState(ParserInfoState);
                                        }
                                        else
                                        {
                                            ParserErrorCode = DataTypes.ParserErrorCodes.ContentLoadFinished;
                                            LastException = null;
                                            PercentOfProgress = 10;
                                            SetAndSendState(ParserInfoState);
                                        }

                                        // Check if thread should be canceled
                                        if (CancelThread)
                                        {
                                            ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                            LastException = null;
                                            PercentOfProgress = 0;
                                            SetAndSendState(ParserInfoState);
                                        }
                                    }

                                    // Set given parsing text to the parser variable
                                    if (ParsingValues.ParsingType == DataTypes.ParsingType.TextParsing)
                                    {
                                        TextForParsing = ParsingValues.ParsingText;
                                    }

                                    _debugger.WriteDebuggingMsg(TextForParsing);

                                    if (ThreadRunning)
                                    {
                                        // Do website or text parsing
                                        if (ParsingValues.ParsingType == DataTypes.ParsingType.WebParsing || ParsingValues.ParsingType == DataTypes.ParsingType.TextParsing)
                                        {
                                            // Set state to parsing
                                            State = DataTypes.ParserState.Parsing;
                                            ParserErrorCode = DataTypes.ParserErrorCodes.SearchStarted;
                                            LastException = null;
                                            PercentOfProgress = 15;
                                            SetAndSendState(ParserInfoState);

                                            var statusValueStep = (100 - 15) / ParsingValues.RegexList.RegexListDictionary.Count;
                                            var statusValue = 15;
                                            _debugger.WriteDebuggingMsg($@"Parsing-Step: {statusValueStep}");

                                            // Loop through the dictionary and fill the result in the result list
                                            foreach (var regexExpression in ParsingValues.RegexList.RegexListDictionary)
                                            {
                                                // Check if thread should be canceled
                                                if (CancelThread)
                                                {
                                                    ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                                    LastException = null;
                                                    PercentOfProgress = 0;
                                                    SetAndSendState(ParserInfoState);
                                                    break;
                                                }

                                                // Set last regex key
                                                LastParsedRegexListKey = regexExpression.Key;

                                                var regexElement = regexExpression.Value;

                                                _debugger.WriteDebuggingMsg(@"");
                                                _debugger.WriteDebuggingMsg($@"regexExpression.Key: {regexExpression.Key}");
                                                _debugger.WriteDebuggingMsg($@"regexElement.RegexExpression: {regexElement.RegexExpression}");

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

                                                _debugger.WriteDebuggingMsg($@"tmpRegexOptions: {tmpRegexOptions}");
                                                _debugger.WriteDebuggingMsg($@"regexExpression.Value.RegexExpression: {regexExpression.Value.RegexExpression}");

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
                                                        _debugger.WriteDebuggingMsg(
                                                            $@"regexExpression.Key: {regexExpression.Key} = Value:{matchCollection[regexExpression.Value.RegexFoundPosition]
                                                                .Groups[1].Value}");

                                                        for (var i = 1;
                                                            i < matchCollection[
                                                                regexExpression.Value.RegexFoundPosition].Groups.Count;
                                                            i++)
                                                        {
                                                            // Check if thread should be canceled
                                                            if (CancelThread)
                                                            {
                                                                ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
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
                                                                ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                                                LastException = null;
                                                                PercentOfProgress = 0;
                                                                SetAndSendState(ParserInfoState);
                                                            }

                                                            _debugger.WriteDebuggingMsg(
                                                                $@"regexExpression.Key: {regexExpression.Key} = Value: {match.Groups[1].Value}");
                                                            
                                                            for (var i = 1; i < match.Groups.Count; i++)
                                                            {
                                                                // Check if thread should be canceled
                                                                if (CancelThread)
                                                                {
                                                                    ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
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
                                                    _debugger.WriteDebuggingMsg(@"No MATCH found!");

                                                    ParserErrorCode = DataTypes.ParserErrorCodes.ParsingFailed;
                                                    LastException = null;
                                                    PercentOfProgress = 0;
                                                    SetAndSendState(ParserInfoState);
                                                    break;
                                                }

                                                statusValue += statusValueStep;

                                                if (statusValue < 100)
                                                {
                                                    ParserErrorCode = DataTypes.ParserErrorCodes.SearchRunning;
                                                    LastException = null;
                                                    PercentOfProgress = statusValue;
                                                    SetAndSendState(ParserInfoState);
                                                }

                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                ParserErrorCode = DataTypes.ParserErrorCodes.SearchFinished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {
                                                ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                // Signal that the thread has finished
                                                ParserErrorCode = DataTypes.ParserErrorCodes.Finished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }
                                        }

                                        if (ParsingValues.ParsingType == DataTypes.ParsingType.DailyValuesParsing)
                                        {
                                            if (TextForParsing == @"")
                                            {
                                                ParserErrorCode = DataTypes.ParserErrorCodes.ParsingFailed;
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
                                                ParserErrorCode = DataTypes.ParserErrorCodes.ParsingFailed;
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
                                                    ParserErrorCode = DataTypes.ParserErrorCodes.ParsingFailed;
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
                                                    ParserErrorCode = DataTypes.ParserErrorCodes.SearchRunning;
                                                    LastException = null;
                                                    PercentOfProgress = (int)stateValue;
                                                    SetAndSendState(ParserInfoState);
                                                }
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                ParserErrorCode = DataTypes.ParserErrorCodes.SearchFinished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Check if thread should be canceled
                                            if (CancelThread)
                                            {
                                                ParserErrorCode = DataTypes.ParserErrorCodes.CancelThread;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                            }

                                            // Get time to press "Cancel"
                                            Thread.Sleep(100);

                                            if (ThreadRunning)
                                            {
                                                // Signal that the thread has finished
                                                ParserErrorCode = DataTypes.ParserErrorCodes.Finished;
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
                        State = DataTypes.ParserState.Idle;
                        ParserErrorCode = DataTypes.ParserErrorCodes.WebExceptionOccured;
                        LastException = webEx;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                    }
                    catch (Exception ex)
                    {
                        // Set state
                        State = DataTypes.ParserState.Idle;
                        ParserErrorCode = DataTypes.ParserErrorCodes.ExceptionOccured;
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
                _debugger.WriteDebuggingMsg(@"Event: OnWebClientDownloadDataWebSiteCompleted");

                if (e.Error != null || e.Cancelled) return;

                _debugger.WriteDebuggingMsg($@"e.Result.LongLength: OnWebClientDownloadDataWebSiteCompleted: {e.Result.LongLength}");

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
                State = DataTypes.ParserState.Idle;
                ParserErrorCode = DataTypes.ParserErrorCodes.WebExceptionOccured;
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
                    ParserErrorCode = DataTypes.ParserErrorCodes.Starting;
                    LastException = null;
                    PercentOfProgress = 0;
                    SetAndSendState(ParserInfoState);

                    // Check if a new parsing could be started
                    if (State != DataTypes.ParserState.Idle)
                    {
                        ParserErrorCode = DataTypes.ParserErrorCodes.BusyFailed;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    // Check if the given web address is not invalid and a web parsing should be done
                    if (ParsingValues.WebSiteUrl == null && (ParsingValues.ParsingType == DataTypes.ParsingType.WebParsing || ParsingValues.ParsingType == DataTypes.ParsingType.DailyValuesParsing))
                    {
                        ParserErrorCode = DataTypes.ParserErrorCodes.InvalidWebSiteGiven;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    if ((ParsingValues.RegexList == null || ParsingValues.RegexList.RegexListDictionary.Count == 0) && ParsingValues.ParsingType == DataTypes.ParsingType.WebParsing)
                    {
                        ParserErrorCode = DataTypes.ParserErrorCodes.NoRegexListGiven;
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
                State = DataTypes.ParserState.Idle;
                ParserErrorCode = DataTypes.ParserErrorCodes.ExceptionOccured;
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
        private void SetAndSendState(DataTypes.ParserInfoState parserInfoState)
        {
            _debugger.WriteDebuggingMsg($@"");
            _debugger.WriteDebuggingMsg($@"SetAndSendState:");
            _debugger.WriteDebuggingMsg(
                $@"State: {State} / ThreadRunning: {ThreadRunning} / ErrorCode: {parserInfoState.LastErrorCode} / PercentageOfProgress: {parserInfoState.Percentage}");

            if (parserInfoState.Exception != null)
                _debugger.WriteDebuggingMsg($@"Exception: {parserInfoState.Exception.Message}");

            // Set state to "idle"
            if (parserInfoState.LastErrorCode == DataTypes.ParserErrorCodes.Finished || parserInfoState.LastErrorCode < 0)
            {
                State = DataTypes.ParserState.Idle;
                CancelThread = false;
            }

            // Send state
            //if (ThreadRunning)
            OnParserUpdate?.Invoke(this, new DataTypes.OnParserUpdateEventArgs(parserInfoState));

            // Stop thread
            if (parserInfoState.LastErrorCode == DataTypes.ParserErrorCodes.Finished || parserInfoState.LastErrorCode < 0)
            {
                ThreadRunning = false;
            }       
        }

        #endregion Methodes

        #region Events / Delegates

        public delegate void ParserUpdateEventHandler(object sender, DataTypes.OnParserUpdateEventArgs e);

        public event ParserUpdateEventHandler OnParserUpdate;

        #endregion Events / Delegates
    }
}
