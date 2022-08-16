using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using Parser;

namespace ParserTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// This test checks that it is not possible to start a parser which is running again.
        /// </summary>
        [Test]
        public void Test_1_CheckParserAlreadyBusy()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to Parser
            if (Uri.TryCreate(@"http://www.google.com", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Create RegexList
            var regexList = new RegExList(@"FirstRegex",
                new RegexElement(@"RegexString1", 1, true, new List<RegexOptions>() {RegexOptions.None}));
            regexList.Add(@"SecondRegex",
                new RegexElement(@"RegexString2", 1, false,
                    new List<RegexOptions>() {RegexOptions.Singleline, RegexOptions.IgnoreCase}));

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), regexList);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            newParser.StartParsing();
            Thread.Sleep(25);

            // Try to start parsing again
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(false, result);

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the BusyFailed event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.BusyFailed), 0);
        }

        /// <summary>
        /// This test checks if no web site is given
        /// </summary>
        [Test]
        public void Test_2_CheckEmptyWebSiteGiven()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to a empty string;
            if (!Uri.TryCreate(@"", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), new RegExList());

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(false, result);

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the InvalidWebSiteGiven event has been signaled
            Assert.GreaterOrEqual(
                receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.InvalidWebSiteGiven), 0);
        }

        /// <summary>
        /// This test checks if an invalid web site is given
        /// </summary>
        [Test]
        public void Test_3_CheckInvalidWebSiteGiven()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(@"htt://www.google.com", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), null);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(false, result);

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the InvalidWebSiteGiven event has been signaled
            Assert.GreaterOrEqual(
                receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.InvalidWebSiteGiven), 0);
        }

        /// <summary>
        /// This test checks a regex list is given
        /// </summary>
        [Test]
        public void Test_4_CheckParserRegexListGiven()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to Parser
            if (Uri.TryCreate(@"http://www.google.com", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), null);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(false, result);

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the NoRegexListGiven event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoRegexListGiven),
                0);
        }

        /// <summary>
        /// This test checks if a parser error occurred
        /// </summary>
        [Test]
        public void Test_5_CheckParserError()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(@"http://tbarth.eu/sunnyconnectoranalyzer", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Create regex list
            var regexList = new RegExList(@"Gesamt",
                new RegexElement(@">Gsamt-(.*?)<", 0, false, new List<RegexOptions>() {RegexOptions.None}));

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), regexList);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.ParsingFailed) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the ParsingFailed event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.ParsingFailed), 0);
        }

        /// <summary>
        /// This test checks if the parse is successful
        /// </summary>
        [Test]
        public void Test_6_CheckParserSuccessful()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(@"http://tbarth.eu/sunnyconnectoranalyzer", UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Create regex list
            var regexList = new RegExList(@"Gesamt",
                new RegexElement(@">Gesamt-(.*?)<", -1, false, new List<RegexOptions>() {RegexOptions.None}));

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, Encoding.UTF8.ToString(), regexList);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate(object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished), 0);
        }

        /// <summary>
        /// This test checks if a OnVista history API request could be done
        /// </summary>
        [Test]
        public void Test_7_CheckOnVistaApiRequestHistory()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://api.onvista.de/api/v1/instruments/FUND/114917893/eod_history?idNotation=39517324&startDate=2021-02-28&range=M1",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, null, Encoding.UTF8.ToString(), DataTypes.ParsingType.OnVistaHistoryData);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished), 0);
        }

        /// <summary>
        /// This test checks if a OnVista history API request could be done
        /// </summary>
        [Test]
        public void Test_8_CheckOnVistaApiRequestHistoryWebContentNotLoaded()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://api.onvista.de/api/v1/instruments/FUND/114917893/eod_history?idNotation=3951732&startDate=2021-02-28&range=M1",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, null, Encoding.UTF8.ToString(), DataTypes.ParsingType.OnVistaHistoryData);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoWebContentLoaded) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoWebContentLoaded), 0);
        }

        /// <summary>
        /// This test checks if a OnVista realtime API request could be done
        /// </summary>
        /*[Test]
        public void Test_9_CheckOnVistaApiRequestRealTime()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://www.onvista.de/api/quote/9385986/RLT",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, null, Encoding.UTF8.ToString(), DataTypes.ParsingType.OnVistaRealTime);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished), 0);
        }*/

        /// <summary>
        /// This test checks if a OnVista realtime API request could not be done
        /// </summary>
        /*[Test]
        public void Test_10_CheckOnVistaApiRequestRealTimeError()
        {
            // Create parser
            var newParser = new Parser.Parser();
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://www.onvista.de/api/quote/9385986/RL",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, null, Encoding.UTF8.ToString(), DataTypes.ParsingType.OnVistaRealTime);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoWebContentLoaded) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(
                receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoWebContentLoaded), 0);
        }*/

        /// <summary>
        /// This test checks if a Yahoo realtime API request could be done
        /// </summary>
        [Test]
        public void Test_11_CheckYahooApiRequestRealTime()
        {
            // Create parser
            var newParser = new Parser.Parser(true);
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://yfapi.net/v6/finance/quote?region=DE&lang=de&symbols=APC.F",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, "T7Dcb5soihfwQpPoQAXtMashju0rQj8VCfztXC90", Encoding.UTF8.ToString(), DataTypes.ParsingType.YahooRealTime);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished), 0);
        }

        /// <summary>
        /// This test checks if a Yahoo realtime API request could be done
        /// </summary>
        [Test]
        public void Test_12_CheckYahooApiRequestHistoryData()
        {
            // Create parser
            var newParser = new Parser.Parser(true);
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://yfapi.net/v8/finance/chart/APC.F?range=1mo&region=DE&interval=1d&lang=de",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, "T7Dcb5soihfwQpPoQAXtMashju0rQj8VCfztXC90", Encoding.UTF8.ToString(), DataTypes.ParsingType.YahooHistoryData);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 500 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished), 0);
        }

        /// <summary>
        /// This test checks if a Yahoo realtime API request could be done
        /// </summary>
        [Test]
        public void Test_13_CheckHttpStatusCode()
        {
            // Create parser
            var newParser = new Parser.Parser(true);
            // Create list for the received events
            var receivedEventsCodes = new List<DataTypes.ParserErrorCodes>();
            var receivedEventsWebContent = new List<string>();
            var receivedEventsHttpStatucCode = new List<HttpStatusCode>();

            // WebSiteUrl for the parser
            Uri webSiteUrl = null;

            // Set website to the Parser
            if (Uri.TryCreate(
                @"https://www.onvista.de/api/quote/9385986/RLT",
                UriKind.Absolute, out var uriResult))
                webSiteUrl = uriResult;

            // Set parsing value to the parser
            newParser.ParsingValues = new ParsingValues(webSiteUrl, @"", Encoding.UTF8.ToString(), DataTypes.ParsingType.YahooHistoryData);

            // Create delegate function which adds the received events to the list
            newParser.OnParserUpdate += delegate (object sender, DataTypes.OnParserUpdateEventArgs e)
            {
                receivedEventsCodes.Add(e.ParserInfoState.LastErrorCode);
                receivedEventsWebContent.Add(e.ParserInfoState.WebSiteContentAsString);
                receivedEventsHttpStatucCode.Add(e.ParserInfoState.HttpStatus);
            };

            // Start parsing
            var result = newParser.StartParsing();

            // Check if the start was not successful
            Assert.AreEqual(true, result);

            // Wait for the parsing result with a 5s timeout
            var counter = 0;
            while (counter < 300 &&
                   receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.Finished) < 0)
            {
                Thread.Sleep(10);
                counter++;
            }

            ShowParsingErrorCodes(receivedEventsCodes, receivedEventsWebContent, receivedEventsHttpStatucCode);

            // Check if the Finished event has been signaled
            Assert.GreaterOrEqual(receivedEventsCodes.FindIndex(x => x == DataTypes.ParserErrorCodes.NoWebContentLoaded), 0);
            Assert.GreaterOrEqual(receivedEventsHttpStatucCode.FindIndex(x => x == HttpStatusCode.NotFound), 0);
        }

        private static void ShowParsingErrorCodes(List<DataTypes.ParserErrorCodes> errorCodes, List<string> webContent, List<HttpStatusCode>httpStatusCodes)
        {
            for (var i = 0; i < errorCodes.Count; i++)
            {
                Console.WriteLine(@"ErrorCode:  {0}", errorCodes[i]);
                Console.WriteLine(@"WebContent: {0}", webContent[i]);
                Console.WriteLine(@"HttpCode:   {0}", httpStatusCodes[i]);

                Console.WriteLine(@"");
            }
        }
    }
}