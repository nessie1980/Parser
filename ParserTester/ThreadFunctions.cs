using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Parser;
using Parser = Parser.Parser;

namespace ParserTester
{
    public class ThreadFunctions
    {
        #region Variables

        public readonly FrmParserTester FrmParserTester;

        /// <summary>
        /// States for the GUI update of the test report
        /// </summary>
        public enum GuiUpdateState
        {
            ProcessStartSuccess = 1,
            ProcessStartFailed = 2,
            TestCaseStart = 3,
            TestCaseResult = 4,
            ProcessFinish = 5,
            ProcessCancel = 6
        };

        #endregion Variables

        #region Properties

        /// <summary>
        /// Global flag for stopping the thread
        /// </summary>
        public bool ThreadStop { set; get; } = false;

        /// <summary>
        /// Global flag for starting the thread
        /// </summary>
        public bool ThreadStart { set; get; }

        /// <summary>
        /// Counter for the test case
        /// </summary>
        public int TestCaseCounter { internal set; get; }

        /// <summary>
        /// Counter for the GUI updates
        /// </summary>
        public int GuiUpdateCounter { internal set; get; }

        /// <summary>
        /// Global flag for starting the next test-case
        /// </summary>
        public bool NextTestCaseFlag { internal set; get; } = true;

        /// <summary>
        /// Global flag for stopping the test process
        /// </summary>
        public bool CancelFlag { set; get; }

        /// <summary>
        /// Global flag for signal that the test process has been finished
        /// </summary>
        public bool FinishFlag { internal set; get; }

        /// <summary>
        /// Parser for the thread
        /// </summary>
        public global::Parser.Parser Parser { internal set; get; }

        #endregion Properties

        #region Delegates / Events

        public delegate void UpdateGuiEventHandler(object sender, GuiUpdateEventArgs e);

        public event UpdateGuiEventHandler OnUpdateGuiEvent;

        #endregion Delegates / Events

        public ThreadFunctions(FrmParserTester frmParserTester)
        {
            FrmParserTester = frmParserTester;

            Parser = new global::Parser.Parser();

            var threadTestCase = new Thread(ThreadTestCases)
            {
                Name = @"Testautomation"
            };

            threadTestCase.Start();
        }

        /// <summary>
        /// This function starts the test automation
        /// </summary>
        public void StartTestCaseThread()
        {
            ThreadStart = true;
        }

        /// <summary>
        /// This function does the test automation
        /// </summary>
        private void ThreadTestCases()
        {
            while (true)
            {
                if (ThreadStop)
                    break;

                while (ThreadStart && !CancelFlag && !FinishFlag)
                {
                    if (TestCaseCounter == 0)
                    {
                        OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartSuccess));
                    }

                    if (NextTestCaseFlag)
                    {
                        Thread.Sleep(1000);
                        NextTestCaseFlag = false;

                        while (Parser.State != ParserState.Idle)
                        {
                            Thread.Sleep(100);
                        }

                        // Increase the test case counter to start the next test case
                        TestCaseCounter++;

                        // Reset GUI update counter
                        GuiUpdateCounter = 0;

                        if (GuiUpdateCounter == 0)
                        {
                            Parser.OnParserUpdate -= FrmParserTester.OnUpdate;
                            Parser.OnParserUpdate += FrmParserTester.OnUpdate;
                        }

                        // Add new test cases here
                        switch (TestCaseCounter)
                        {
                            case 1:
                            {
                                CheckParserStillWorking();
                                break;
                            }
                            case 2:
                            {
                                CheckParserUrlGiven();
                                break;
                            }
                            case 3:
                            {
                                CheckParserRegexListGiven();
                                break;
                            }
                            case 4:
                            {
                                CheckParserInvalidUrlGiven();
                                break;
                            }
                            case 5:
                            {
                                CheckParserError();
                                break;
                            }
                            case 6:
                            {
                                CheckParserWebParsingSuccessful();
                                break;
                            }
                            case 7:
                            {
                                CheckParserDailyValuesSuccessful();
                                break;
                            }
                            case 8:
                            {
                                FinishFlag = true;
                                break;
                            }
                        }
                    }
                }

                // Check if the test process should be canceled
                if (CancelFlag)
                {
                    OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessCancel));

                    CancelFlag = false;
                    NextTestCaseFlag = true;
                    ThreadStart = false;
                    TestCaseCounter = 0;
                }

                if (!FinishFlag) continue;

                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessFinish));

                FinishFlag = false;
                NextTestCaseFlag = true;
                ThreadStart = false;
                TestCaseCounter = 0;
            }
        }

        #region Testcase functions

        /// <summary>
        /// This test case checks if the Parser is still working
        /// </summary>
        private void CheckParserStillWorking()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to Parser
            if (Uri.TryCreate(@"http://www.google.com", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Create RegexList
            var regexList = new RegExList(@"FirstRegex", new RegexElement(@"RegexString1", 1, true, new List<RegexOptions>() { RegexOptions.None }));
            regexList.Add(@"SecondRegex", new RegexElement(@"RegexString2", 1, false, new List<RegexOptions>() { RegexOptions.Singleline, RegexOptions.IgnoreCase }));
            
            // Set regex list to Parser
            Parser.RegexList = regexList;
            
            // Start parsing
            Parser.StartParsing();
        }

        /// <summary>
        /// This test case checks if the Parser has a url given
        /// </summary>
        private void CheckParserUrlGiven()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to a empty string;
            if (!Uri.TryCreate(@"", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This test case checks if the Parser has a regex list given
        /// </summary>
        private void CheckParserRegexListGiven()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the Parser
            if (Uri.TryCreate(@"http://www.google.com", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Set regex list to null in the Parser
            Parser.RegexList = null;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This test case checks when a invalid url is given
        /// </summary>
        private void CheckParserInvalidUrlGiven()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the Parser
            if (Uri.TryCreate(@"htt://www.google.com", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Set regex list to null in the Parser
            Parser.RegexList = null;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This test case checks if the search is successful
        /// </summary>
        private void CheckParserError()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the Parser
            if (Uri.TryCreate(@"http://tbarth.eu/sunnyconnectoranalyzer", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Create regex list
            var regexList = new RegExList(@"Gesamt", new RegexElement(@">Gsamt-(.*?)<", 0, false, new List<RegexOptions>() { RegexOptions.None }));
            // Set regex list to Parser
            Parser.RegexList = regexList;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This test case checks if the web parsing search is successful
        /// </summary>
        private void CheckParserWebParsingSuccessful()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set website to the Parser
            if (Uri.TryCreate(@"http://tbarth.eu/sunnyconnectoranalyzer", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set parsing type
            Parser.ParingType = ParsingType.WebParsing;

            // Create regex list
            var regexList = new RegExList(@"Gesamt", new RegexElement(@">Gesamt-(.*?)<", -1, false, new List<RegexOptions>() { RegexOptions.None }));

            // Set regex list to Parser
            Parser.RegexList = regexList;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }

        /// <summary>
        /// This test case checks if the daily values search is successful
        /// </summary>
        private void CheckParserDailyValuesSuccessful()
        {
            // Add start entry to report
            OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.TestCaseStart, new List<string> { System.Reflection.MethodBase.GetCurrentMethod().Name }));

            // Set parsing type
            Parser.ParingType = ParsingType.DailyValuesParing;

            // Set website to the Parser
            if (Uri.TryCreate(
                $@"https://www.onvista.de/onvista/times+sales/popup/historische-kurse/?notationId=39517324&dateStart={DateTime.Now.Date.AddDays(-7).ToShortDateString()}&interval=D7&assetName=Adidas&exchange=Tradegate", UriKind.Absolute, out var uriResult))
                Parser.WebSiteUrl = uriResult;

            // Set regex list to Parser
            Parser.RegexList = null;

            // Daily values list
            var dailyValuesList = new List<DailyValues>();

            // Set daily values list
            Parser.DailyValuesList = dailyValuesList;

            // Check if the parsing process has been started
            if (!Parser.StartParsing())
            {
                OnUpdateGuiEvent?.Invoke(this, new GuiUpdateEventArgs(GuiUpdateState.ProcessStartFailed));
            }
        }
        #endregion Thread functions

        /// <summary>
        /// Class for the GUI update via test case process thread
        /// </summary>
        public class GuiUpdateEventArgs : EventArgs
        {

            #region Variables

            #endregion Variables

            #region Properties

            public GuiUpdateState State { get; }

            public List<string> Messages { get; }

            #endregion Properties

            #region Methodes

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="state">State of the update</param>
            /// <param name="messages">Messages of the update</param>
            public GuiUpdateEventArgs(GuiUpdateState state, List<string> messages = null)
            {
                State = state;
                Messages = messages;
            }

            #endregion Methodes
        }
    }
}

