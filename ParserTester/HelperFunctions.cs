using System;
using System.Drawing;
using System.Windows.Forms;
using Parser;

namespace ParserTester
{
    public static class HelperFunctions
    {
        /// <summary>
        /// This function add the start test process fail entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        /// <returns>Flag that the test process start failed</returns>
        public static bool AddTestProcessFailedToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText("Test case start ");
            richTextBoxResult.SelectionColor = Color.Red;
            richTextBoxResult.AppendText("FAILED");
            richTextBoxResult.AppendText($"{Environment.NewLine}{Environment.NewLine}");

            return false;
        }

        /// <summary>
        /// This function add the start test process entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        public static void AddTestProcessStartToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText($"Test process started{Environment.NewLine}{Environment.NewLine}");
        }

        /// <summary>
        /// This function add the start entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        /// <param name="testCaseName">Name of the test case</param>
        public static void AddTestCaseStartToReport(RichTextBox richTextBoxResult, string testCaseName)
        {
            richTextBoxResult.AppendText(
                $"========================================================================{Environment.NewLine}");
            richTextBoxResult.AppendText(
                $"Start test case: \"{testCaseName}\"{Environment.NewLine}{Environment.NewLine}");
        }

        public static void AddTestCaseStateToReport(RichTextBox richTextBoxResult, OnParserUpdateEventArgs e)
        {
            // Only past the Parser result if the parsing failed
            if (e.ParserInfoState.Exception == null)
            {
                richTextBoxResult.AppendText(e.ParserInfoState.LastErrorCode < 0 ? "Error:\t" : "Status:\t");

                richTextBoxResult.AppendText(
                    string.Format("{0} ({1} %){2}",
                    e.ParserInfoState.LastErrorCode.ToString(), e.ParserInfoState.Percentage.ToString(), Environment.NewLine, Environment.NewLine)
                    );
            }
            else
            {
                richTextBoxResult.AppendText(e.ParserInfoState.LastErrorCode < 0 ? "Error:\t" : "Status:\t");

                richTextBoxResult.AppendText(
                    $"{e.ParserInfoState.LastErrorCode.ToString()} ({e.ParserInfoState.Percentage.ToString()} %){Environment.NewLine}{e.ParserInfoState.Exception.Message}{Environment.NewLine}"
                );
            }
        }

        /// <summary>
        /// This function add the test case result entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        /// <param name="result">test case result</param>
        public static void AddTestCaseResultToReport(RichTextBox richTextBoxResult, bool result)
        {
            richTextBoxResult.AppendText($"{Environment.NewLine}Result: ");

            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
                richTextBoxResult.AppendText("PASSED");
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
                richTextBoxResult.AppendText("FAILED");
            }

            richTextBoxResult.SelectionColor = Color.Black;
            richTextBoxResult.AppendText(
                $"{Environment.NewLine}========================================================================");
            richTextBoxResult.AppendText($"{Environment.NewLine}{Environment.NewLine}");
        }

        /// <summary>
        /// This function add the finish test process result entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        /// <param name="result">Test finish result</param>
        public static void AddTestProcessFinishResultToReport(RichTextBox richTextBoxResult, bool result)
        {
            richTextBoxResult.SelectionColor = result ? Color.Green : Color.Red;
            richTextBoxResult.AppendText(
                $"========================================================================{Environment.NewLine}");


            richTextBoxResult.SelectionColor = Color.Black;
            richTextBoxResult.AppendText($"Test process finished{Environment.NewLine}{Environment.NewLine}END-Result: ");

            if (result)
            {
                richTextBoxResult.SelectionColor = Color.Green;
                richTextBoxResult.AppendText("PASSED");
            }
            else
            {
                richTextBoxResult.SelectionColor = Color.Red;
                richTextBoxResult.AppendText("FAILED");
            }

            richTextBoxResult.SelectionColor = result ? Color.Green : Color.Red;
            richTextBoxResult.AppendText(
                $"{Environment.NewLine}========================================================================{Environment.NewLine}");
        }

        /// <summary>
        /// This function add the cancel text process entry to the report box
        /// </summary>
        /// <param name="richTextBoxResult">Rich edit box for the test result output</param>
        public static void AddTestCancelToReport(RichTextBox richTextBoxResult)
        {
            richTextBoxResult.AppendText($"{Environment.NewLine}Test process: ");
            richTextBoxResult.SelectionColor = Color.Red;
            richTextBoxResult.AppendText("CANELLED");
        }
    }
}
