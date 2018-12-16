using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Parser
{
    [Serializable]
    public class Parser
    {

        #region Variables

        /// <summary>
        /// This object is used for the thread lock
        /// </summary>
        private readonly object _thisLockStarting = new object();
        private readonly object _thisLockThread = new object();

        #region Given parameters

        /// <summary>
        /// String for the website url which should be parsed
        /// </summary>
        private string _webSiteUrl = "invalid";

        /// <summary>
        /// List with the regex string for the parsing
        /// </summary>
        private RegExList _regexList;

        #endregion Given parameters

        /// <summary>
        /// User agent identifier with the default value
        /// </summary>
        private string _userAgentIdentifier = @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0";

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
        private Dictionary<string, List<string>> _parsingResult;

        #endregion Parsing result

        #endregion Variables

        #region Properties

        #region Tread

        /// <summary>
        /// This flag starts the thread
        /// </summary>
        public bool ThreadRunning { get; internal set; }

        /// <summary>
        /// Flag if the thread should be canceled
        /// </summary>
        public bool CancelThread { set; get; }

        #endregion Thread

        #region Given parameters

        /// <summary>
        /// Flag if the parser should download content of a given website
        /// or a given text should be parsed
        /// </summary>
        public bool WebParsing { get; set; }

        /// <summary>
        /// String for the website url which should be parsed
        /// </summary>
        public string WebSiteUrl
        {
            get => _webSiteUrl;
            set
            {
                try
                {
                    // Check if the website url is valid
                    //                String regexPattern = @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
                    //                if (System.Text.RegularExpressions.Regex.IsMatch(value, regexPattern))
                    if (value != "")
                    {
                        var uriWebSite = new Uri(value);
                        var uriHostNameType = uriWebSite.HostNameType;
                        //var strDNSName = uriWebSite.DnsSafeHost;
                        var strWebSiteScheme = uriWebSite.Scheme;
                        var isWllFormedUriStringFlag = Uri.IsWellFormedUriString(value, UriKind.Absolute);
                        if (isWllFormedUriStringFlag &&
                            (strWebSiteScheme == Uri.UriSchemeHttp || strWebSiteScheme == Uri.UriSchemeHttps) &&
                            (uriHostNameType == UriHostNameType.Dns || uriHostNameType == UriHostNameType.IPv4 || uriHostNameType == UriHostNameType.IPv6)
                        )
                        {
                            _webSiteUrl = !Regex.IsMatch(value, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?") ? @"invalid" : value;
                        }
                        else
                            _webSiteUrl = @"invalid";
                    }
                    else
                        _webSiteUrl = @"invalid";

                    ParserInfoState.WebSite = _webSiteUrl;
                }
                catch
                {
                    _webSiteUrl = @"invalid";
                }
            }
        }

        /// <summary>
        /// List with the regex string for the parsing
        /// </summary>
        public RegExList RegexList
        {
            get => _regexList;
            set
            {
                _regexList = value;
                ParserInfoState.RegexList = value;
            }
        }

        /// <summary>
        /// Encoding type for the download content
        /// </summary>
        public string EncodingType { get; set; }

        #endregion Given parameters

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
        /// String with the loaded website content
        /// </summary>
        public string WebSiteContent { get; internal set; }

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
            get => _parsingResult;
            internal set
            {
                _parsingResult = value;
                ParserInfoState.SearchResult = value;
            }
        }

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
            threadParser.Start();

            EncodingType = Encoding.Default.ToString();
            WebSiteUrl = @"";
            State = ParserState.Idle;
            RegexList = null;
        }

        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="webParsing">Flag if a website content should be loaded and parsed or a given text should be parsed</param>
        /// <param name="webSiteUrlOrText">URL of the website which should be loaded and parsed or the text which should be parsed</param>
        /// <param name="regexList">Dictionary with the regex strings and the regex options for it</param>
        /// <param name="encoding">Encoding for the download content</param>
        // ReSharper disable once RedundantBaseConstructorCall
        public Parser(bool webParsing, string webSiteUrlOrText, RegExList regexList, string encoding) : this ()
        {
#if _DEBUG_THREADFUNCTION
            Console.WriteLine(@"Parameter constructor");
#endif

            WebParsing = webParsing;

            // Check if a web content should be parsed or a text
            if (WebParsing)
            {
                // User property for validation
                WebSiteUrl = webSiteUrlOrText;
            }
            else
            {
                TextForParsing = webSiteUrlOrText;
            }

            EncodingType = encoding;
            RegexList = regexList;
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
                                    if (WebParsing)
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
                                            // Browser identifier (e.g. FireFox 36)
                                            client.Headers["User-Agent"] = UserAgentIdentifier;
                                            // Download content as raw data
#if _DEBUG_THREADFUNCTION
                                            Console.WriteLine(@"WebSide: {0}", _webSiteUrl);
#endif
                                            WebSiteDownloadComplete = false;
                                            client.DownloadProgressChanged += OnWebClientDownloadProgressChanged;
                                            client.DownloadDataCompleted += OnWebClientDownloadDataWebSiteCompleted;
                                            client.DownloadDataAsync(new Uri(_webSiteUrl));
                                            while (!WebSiteDownloadComplete)
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

                                        // Set loaded web site content to parsing text
                                        TextForParsing = WebSiteContent;

                                        // Check if the website content load was successful and call event
                                        if (WebSiteContent == @"")
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
                                        // Set state to parsing
                                        State = ParserState.Parsing;
                                        ParserErrorCode = ParserErrorCodes.SearchStarted;
                                        LastException = null;
                                        PercentOfProgress = 15;
                                        SetAndSendState(ParserInfoState);

                                        var statusValueStep = (100 - 15) / RegexList.RegexListDictionary.Count;
                                        var statusValue = 15;
#if _DEBUG_THREADFUNCTION
                                        Console.WriteLine("Parsing-Step: {0}", statusValueStep);
#endif

                                        // Loop through the dictionary and fill the result in the result list
                                        foreach (var regexExpression in RegexList.RegexListDictionary)
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
                                            Console.WriteLine("RegexExpression: {0}", regexExpression.Value.RegexExpression);
#endif

                                            // Search for the value
                                            var added = false;
                                            var matchCollection = Regex.Matches(TextForParsing, regexExpression.Value.RegexExpression, tmpRegexOptions);

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
                                                    Console.WriteLine(@"Value: '{0}' = '{1}'", regexExpression.Key, matchCollection[regexExpression.Value.RegexFoundPosition].Groups[1].Value);
#endif
                                                    for (var i = 1; i < matchCollection[regexExpression.Value.RegexFoundPosition].Groups.Count; i++)
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
                                                        if (matchCollection[regexExpression.Value.RegexFoundPosition]
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
                                                        Console.WriteLine(@"Value: '{0}' = '{1}'", regexExpression.Key, match.Groups[1].Value);
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
                                            if ((matchCollection.Count == 0 || added == false) && !regexElement.ResultEmpty)
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

                WebSiteContent = e.Result.LongLength > 0 ? Encoding.UTF8.GetString(e.Result) : "";

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
                    if (WebSiteUrl == @"invalid" && WebParsing)
                    {
                        ParserErrorCode = ParserErrorCodes.InvalidWebSiteGiven;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    if (RegexList == null || RegexList.RegexListDictionary.Count == 0)
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
#endif
            // Set state to "idle"
            if (parserInfoState.LastErrorCode == ParserErrorCodes.Finished || parserInfoState.LastErrorCode < 0)
            {
                State = ParserState.Idle;
                CancelThread = false;
            }

            // Send state
            if (OnParserUpdate != null)
            {
                if (ThreadRunning)
                    OnParserUpdate(this, new OnParserUpdateEventArgs(parserInfoState));
            }

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
        /// Exception if an exception occurred
        /// </summary>
        public Exception Exception { get; internal set; }

        #endregion Properties
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
