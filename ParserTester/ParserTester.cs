using System;
using System.Windows.Forms;
using Parser;

namespace ParserTester
{
    public partial class FrmParserTester : Form
    {
        #region Variables

        /// <summary>
        /// Instance of the ThreadFunctions
        /// </summary>
        private readonly ThreadFunctions _threadFunctions;

        /// <summary>
        /// Global flag for the test result
        /// This flag must be set if any test case failed
        /// </summary>
        private bool _resultFlag;

        #endregion Variables

        #region Properties
        #endregion Properties

        #region Methodes

        public FrmParserTester()
        {
            InitializeComponent();

            _threadFunctions = new ThreadFunctions(this);

            _threadFunctions.OnUpdateGuiEvent -= OnUpdateGui;
            _threadFunctions.OnUpdateGuiEvent += OnUpdateGui;
            _resultFlag = true;
        }
        
        private void FrmParserTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            _threadFunctions.Parser.OnParserUpdate -= OnUpdate;
            _threadFunctions.OnUpdateGuiEvent -= OnUpdateGui;
            _threadFunctions.ThreadStop = true;
        }

        /// <summary>
        /// This function starts the test process
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">EventArgs</param>
        private void OnBtnStartTest_Click(object sender, EventArgs e)
        {
            // Reset variables
            _resultFlag = true;
            richTextBoxResult.Clear();

            btnStartTest.Enabled = false;
            btnStopTest.Enabled = true;

            _threadFunctions.StartTestCaseThread();
        }

        /// <summary>
        /// This function stops the test process
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">EventArgs</param>
        private void OnBtnStopTest_Click(object sender, EventArgs e)
        {
            _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

            // Set cancel flag
            _threadFunctions.CancelFlag = true;

            btnStopTest.Enabled = false;
        }

        #endregion Methodes

        #region Events / Delegates

        /// <summary>
        /// This function updates the test report
        /// </summary>
        /// <param name="sender">Parser</param>
        /// <param name="e">OnSiteLoadFinishedEventArgs</param>
        public void OnUpdate(object sender, OnParserUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUpdate(sender, e)));
            }
            else
            {
                // Write state to the rich edit box
                HelperFunctions.AddTestCaseStateToReport(richTextBoxResult, e);

                Console.WriteLine(@"TestCaseCnt: " + _threadFunctions.TestCaseCounter + @" / LastErrorCode: " + e.ParserInfoState.LastErrorCode + @" / GuiUpdateCnt: " + _threadFunctions.GuiUpdateCounter);
                switch (_threadFunctions.TestCaseCounter)
                {
                    case 1:
                        {
                            const int guiUpdateCounterReached = 2;
                            if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.Started)
                            {
                                _threadFunctions.Parser.StartParsing();
                            }

                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;
                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.BusyFailed)
                                {
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                }
                                else
                                {
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelParserStartNextTestCase(false);
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 2:
                        {
                            const int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                                Console.WriteLine(@" LastErrorCode: " + e.ParserInfoState.LastErrorCode + @" / GUI: " + _threadFunctions.GuiUpdateCounter);
                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.InvalidWebSiteGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.Parser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 3:
                        {
                            const int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.NoRegexListGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.Parser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 4:
                        {
                            const int guiUpdateCounterReached = 1;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.InvalidWebSiteGiven)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.Parser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                {
                                    CancelParserStartNextTestCase();
                                }
                            }

                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 5:
                        {
                            const int guiUpdateCounterReached = 5;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.ParsingFailed)
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.Parser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                    CancelParserStartNextTestCase();
                            }
                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 6:
                        {
                            const int guiUpdateCounterReached = 6;
                            if (_threadFunctions.GuiUpdateCounter == guiUpdateCounterReached)
                            {
                                _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                                if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.Finished
                                    && _threadFunctions.Parser.ParsingResult.Count == 1
                                    && e.ParserInfoState.SearchResult["Gesamt"][0] == "Anlage")
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                                else
                                {
                                    _threadFunctions.Parser.CancelThread = true;
                                    HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                    _resultFlag = false;
                                }
                                _threadFunctions.NextTestCaseFlag = true;
                            }
                            else
                            {
                                if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                    CancelParserStartNextTestCase();
                            }
                            _threadFunctions.GuiUpdateCounter++;
                            break;
                        }
                    case 7:
                    {
                        const int guiUpdateCounterReached = 4;
                        if ((e.ParserInfoState.LastErrorCode < 0 || e.ParserInfoState.LastErrorCode == ParserErrorCodes.Finished) && _threadFunctions.GuiUpdateCounter >= guiUpdateCounterReached )
                        {
                            _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

                            if (e.ParserInfoState.LastErrorCode == ParserErrorCodes.Finished
                                && e.ParserInfoState.DailyValuesList.Count >= 1)
                                HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, true);
                            else
                            {
                                _threadFunctions.Parser.CancelThread = true;
                                HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);
                                _resultFlag = false;
                            }
                            _threadFunctions.NextTestCaseFlag = true;
                        }
                        else
                        {
                            if (e.ParserInfoState.LastErrorCode < 0 && _threadFunctions.GuiUpdateCounter < guiUpdateCounterReached)
                                CancelParserStartNextTestCase();
                        }
                        _threadFunctions.GuiUpdateCounter++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This function updates the test report
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">GuiUpdateEventArgs</param>
        public void OnUpdateGui(object sender, ThreadFunctions.GuiUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUpdateGui(sender, e)));
            }
            else
            {
                switch (e.State)
                {
                    case ThreadFunctions.GuiUpdateState.ProcessStartSuccess:
                    {
                        HelperFunctions.AddTestProcessStartToReport(richTextBoxResult);
                        break;
                    }
                    case ThreadFunctions.GuiUpdateState.ProcessCancel:
                    {
                        btnStartTest.Enabled = true;
                        btnStopTest.Enabled = false;
                        HelperFunctions.AddTestCancelToReport(richTextBoxResult);
                        break;
                    }
                    case ThreadFunctions.GuiUpdateState.TestCaseStart:
                    {
                        HelperFunctions.AddTestCaseStartToReport(richTextBoxResult, e.Messages[0]);
                        break;
                    }
                    case ThreadFunctions.GuiUpdateState.TestCaseResult:
                    {
                        btnStartTest.Enabled = true;
                        btnStopTest.Enabled = false;
                        // This check must always the last one!!!
                        // If a new test case will be added, add it before of this case
                        HelperFunctions.AddTestProcessFinishResultToReport(richTextBoxResult, _resultFlag);
                        break;
                    }
                    case ThreadFunctions.GuiUpdateState.ProcessFinish:
                    {
                        btnStartTest.Enabled = true;
                        btnStopTest.Enabled = false;
                        // This check must always the last one!!!
                        // If a new test case will be added, add it before of this case
                        HelperFunctions.AddTestProcessFinishResultToReport(richTextBoxResult, _resultFlag);
                        break;
                    }
                }
            }
        }

        #endregion Events / Delegates

        #region Parser error occoured

        /// <summary>
        /// This function cancels the Parser and starts the next test case
        /// <param name="bWriteMessage">Flag if a message should be written</param>
        /// </summary>
        private void CancelParserStartNextTestCase( bool bWriteMessage = true)
        {
            _threadFunctions.Parser.OnParserUpdate -= OnUpdate;

            _threadFunctions.Parser.CancelThread = true;

            if (bWriteMessage)
                HelperFunctions.AddTestCaseResultToReport(richTextBoxResult, false);

            _resultFlag = false;
            _threadFunctions.NextTestCaseFlag = true;
        }

        #endregion Parser error occoured
    }
}
