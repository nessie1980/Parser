using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using mshtml;

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

        private readonly WebBrowser _webBrowser = new WebBrowser();

        private HtmlDocument _htmlDocument;

        #region Given parameters

        /// <summary>
        /// List with the parsing values variables
        /// </summary>
        private List<ParsingValues> _parsingValuesList;

        /// <summary>
        /// String for the website url which should be parsed
        /// </summary>
        private Uri _webSiteUrl;

        /// <summary>
        /// List with the regex string for the parsing
        /// </summary>
        private RegExList _regexList;

        /// <summary>
        /// List with the daily values list
        /// </summary>
        private List<DailyValues> _dailyValuesList;

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

        #region Given parameters

        /// <summary>
        /// Type for the parsing
        /// WebParsing: A given url will be loaded and the website content will be parsed by the given regex list
        /// TextParsing: A given text will be parsed by the given regex list
        /// DailyValuesParing: A given url will be loaded and the website content will be parsed for the daily values
        /// </summary>
        public ParsingType ParingType { get; set; }

        /// <summary>
        /// List for the already loaded parsing values and the new parsed daily values
        /// </summary>
        public List <ParsingValues> ParsingValuesList
        {
            get => _parsingValuesList;
            set => _parsingValuesList = value;
        }

        /// <summary>
        /// List with the daily values list
        /// </summary>
        public List<DailyValues> DailyValuesList
        {
            get => _dailyValuesList;
            set => _dailyValuesList = value;
        }

        /// <summary>
        /// Uri for the website url which should be parsed
        /// </summary>
        public Uri WebSiteUrl
        {
            get => _webSiteUrl;
            set
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

                    ParserInfoState.WebSite = _webSiteUrl != null ? _webSiteUrl.ToString() : @"";
                }
                catch
                {
                    _webSiteUrl = null;
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

            threadParser.SetApartmentState(ApartmentState.STA);

            DailyValuesList = null;
            TextForParsing = @"";

            threadParser.Start();

            State = ParserState.Idle;
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="parsingValues">Parsing values for the parsing</param>
        // ReSharper disable once RedundantBaseConstructorCall
        public Parser( ParsingValues parsingValues) : this ()
        {
#if _DEBUG_THREADFUNCTION
            Console.WriteLine(@"Parameter constructor");
#endif
            ParingType = parsingValues.ParsingType;
            WebSiteUrl = new Uri(parsingValues.WebSiteUrl);
            EncodingType = parsingValues.Encoding;
            RegexList = parsingValues.RegexList;
            DailyValuesList = parsingValues.DailyValuesList;
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor with text and RegExList
        /// </summary>
        /// <param name="parsingText">Text which should be parsed</param>
        /// <param name="regexList">Dictionary with the regex strings and the regex options for it</param>
        /// <param name="encoding">Encoding for the download content</param>
        // ReSharper disable once RedundantBaseConstructorCall
        public Parser(string parsingText, RegExList regexList, string encoding) : this()
        {
#if _DEBUG_THREADFUNCTION
            Console.WriteLine(@"Parameter constructor");
#endif

            ParingType = ParsingType.TextParing;

            TextForParsing = parsingText;

            EncodingType = encoding;
            RegexList = regexList;
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor with URL and RegExList
        /// </summary>
        /// <param name="webSiteUrl">URL of the website which should be loaded and parsed or the text which should be parsed</param>
        /// <param name="dailyValuesList">List with the daily values</param>
        /// <param name="encoding">Encoding for the download content</param>
        // ReSharper disable once RedundantBaseConstructorCall
        public Parser(Uri webSiteUrl, out List<DailyValues> dailyValuesList, string encoding) : this()
        {
#if _DEBUG_THREADFUNCTION
            Console.WriteLine(@"Parameter constructor");
#endif
            dailyValuesList = new List<DailyValues>();

            ParingType = ParsingType.DailyValuesParing;

            WebSiteUrl = webSiteUrl;

            EncodingType = encoding;
            DailyValuesList = dailyValuesList;
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
                                    if (ParingType == ParsingType.WebParsing || ParingType == ParsingType.DailyValuesParing)
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
                                            client.DownloadDataAsync(_webSiteUrl);
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
                                        if (ParingType == ParsingType.WebParsing)
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

                                        if (ParingType == ParsingType.DailyValuesParing)
                                        {
                                            var htmlDocument = GetHtmlDocument(TextForParsing, _webBrowser);

                                            if (htmlDocument == null)
                                            {
                                                ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                LastException = null;
                                                PercentOfProgress = 0;
                                                SetAndSendState(ParserInfoState);
                                                break;
                                            }

                                            // Check if the "tbody" tag has been found
                                            if (htmlDocument.GetElementsByTagName("tbody").Count > 0)
                                            {
                                                // Set state to parsing
                                                State = ParserState.Parsing;
                                                ParserErrorCode = ParserErrorCodes.SearchStarted;
                                                LastException = null;
                                                PercentOfProgress = 15;
                                                SetAndSendState(ParserInfoState);

                                                // Loop through the found tables
                                                foreach (HtmlElement table in htmlDocument.GetElementsByTagName("tbody"))
                                                {
                                                    var valuesCounter = 0;

                                                    // Check if the current table has childs
                                                    if (table.All.Count > 0)
                                                    {
                                                        // Count the rows
                                                        for (var index = 0; index < table.All.Count; index++)
                                                        {
                                                            var row = table.All[index];
                                                            if (row.TagName == "TR")
                                                            {
                                                                valuesCounter++;
                                                            }
                                                        }

                                                        var statusValueStep = (double)((100.0f - 15.0f) / valuesCounter);
                                                        var statusValue = 15.0;

                                                        // Loop through the childs
                                                        foreach (HtmlElement row in table.All)
                                                        {
                                                            var iCellCounter = 0;
                                                            var dailyValues = new DailyValues();

                                                            // Check if the child is a table row
                                                            if (row.TagName != "TR") continue;

                                                            // Check if the row has eight elements
                                                            if (row.Children.Count == 8)
                                                            {
                                                                foreach (HtmlElement cell in row.Children)
                                                                {
                                                                    switch (iCellCounter)
                                                                    {
                                                                        case 0:
                                                                            dailyValues.Date = DateTime.Parse(cell.InnerText);
                                                                            break;
                                                                        case 3:
                                                                            dailyValues.OpeningPrice = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 4:
                                                                            dailyValues.Top = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 5:
                                                                            dailyValues.Bottom = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 6:
                                                                            dailyValues.ClosingPrice = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 7:
                                                                            dailyValues.Volume = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                    }

                                                                    iCellCounter++;
                                                                }

                                                                // Only add if the date not exists already
                                                                if (!DailyValuesList.Exists(x => x.Date.ToString(CultureInfo.CurrentUICulture) == dailyValues.Date.ToString(CultureInfo.CurrentUICulture)))
                                                                {
                                                                    // Add new daily values to the list
                                                                    DailyValuesList.Add(dailyValues);
                                                                    DailyValuesList.Sort();
                                                                }

                                                                statusValue += statusValueStep;

                                                                if (statusValue < 100)
                                                                {
                                                                    ParserErrorCode = ParserErrorCodes.SearchRunning;
                                                                    LastException = null;
                                                                    PercentOfProgress = (int)statusValue;
                                                                    SetAndSendState(ParserInfoState);
                                                                }
                                                            }
                                                            // Check if the row has six elements
                                                            else if (row.Children.Count == 6)
                                                            {
                                                                foreach (HtmlElement cell in row.Children)
                                                                {
                                                                    switch (iCellCounter)
                                                                    {
                                                                        case 0:
                                                                            dailyValues.Date = DateTime.Parse(cell.InnerText);
                                                                            break;
                                                                        case 1:
                                                                            dailyValues.OpeningPrice = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 2:
                                                                            dailyValues.Top = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 3:
                                                                            dailyValues.Bottom = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 4:
                                                                            dailyValues.ClosingPrice = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                        case 5:
                                                                            dailyValues.Volume = decimal.Parse(cell.InnerText);
                                                                            break;
                                                                    }

                                                                    iCellCounter++;
                                                                }

                                                                // Only add if the date not exists already
                                                                if (!DailyValuesList.Exists(x => x.Date.ToString(CultureInfo.CurrentUICulture) == dailyValues.Date.ToString(CultureInfo.CurrentUICulture)))
                                                                {
                                                                    // Add new daily values to the list
                                                                    DailyValuesList.Add(dailyValues);
                                                                    DailyValuesList.Sort();
                                                                }

                                                                statusValue += statusValueStep;

                                                                if (statusValue < 100)
                                                                {
                                                                    ParserErrorCode = ParserErrorCodes.SearchRunning;
                                                                    LastException = null;
                                                                    PercentOfProgress = (int)statusValue;
                                                                    SetAndSendState(ParserInfoState);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                                LastException = null;
                                                                PercentOfProgress = 0;
                                                                SetAndSendState(ParserInfoState);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                        LastException = null;
                                                        PercentOfProgress = 0;
                                                        SetAndSendState(ParserInfoState);
                                                        break;
                                                    }
                                                }

                                                // Signal that the thread has finished
                                                ParserErrorCode = ParserErrorCodes.Finished;
                                                LastException = null;
                                                PercentOfProgress = 100;
                                                SetAndSendState(ParserInfoState);
                                            }
                                            else
                                            {
                                                ParserErrorCode = ParserErrorCodes.ParsingFailed;
                                                LastException = null;
                                                PercentOfProgress = 0;
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
                    ParsingResult?.Clear();
                    ParsingResult = null;

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
                    if (WebSiteUrl == null && (ParingType == ParsingType.WebParsing || ParingType == ParsingType.DailyValuesParing))
                    {
                        ParserErrorCode = ParserErrorCodes.InvalidWebSiteGiven;
                        LastException = null;
                        PercentOfProgress = 0;
                        SetAndSendState(ParserInfoState);
                        return false;
                    }

                    if ((RegexList == null || RegexList.RegexListDictionary.Count == 0) && ParingType == ParsingType.WebParsing)
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
            if (OnParserUpdate != null)
            {
                //if (ThreadRunning)
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

        private static HtmlDocument GetHtmlDocument(string html, WebBrowser webBrowser)
        {
            webBrowser.DocumentText = html;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.Document.OpenNew(true);

            var htmlDocument = webBrowser.Document;
            htmlDocument.Write(html);

            return htmlDocument;
        }
    }

    public class ParsingValues
    { 
        internal ParsingType ParsingType { get; set; }
        internal string WebSiteUrl { get; set; }
        internal string ParsingText { get; set; }
        internal string Encoding { get; set; }
        internal RegExList RegexList { get; set; }
        internal List<DailyValues> DailyValuesList { get; set; }

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
            Encoding = encoding;
            RegexList = regexList;
            DailyValuesList = null;
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
            WebSiteUrl = webSiteUrl.ToString();
            ParsingText = null;
            Encoding = encoding;
            RegexList = regexList;
            DailyValuesList = null;
        }

        /// <summary>
        /// Parsing values with a URL for where the text which should be parsed for daily values
        /// </summary>
        /// <param name="webSiteUrl">URL from where the text which should be parsed should be loaded</param>
        /// <param name="encoding">Encoding of the URL website</param>
        /// <param name="dailyValuesList">Daily values which already had been loaded</param>
        public ParsingValues(string webSiteUrl, string encoding, List<DailyValues> dailyValuesList)
        {
            ParsingType = ParsingType.DailyValuesParing;
            WebSiteUrl = webSiteUrl;
            ParsingText = null;
            Encoding = encoding;
            RegexList = null;
            DailyValuesList = dailyValuesList;
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
