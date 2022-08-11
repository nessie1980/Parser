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
using System.Text.RegularExpressions;

namespace Parser
{
    public class ParsingValues
    {
        #region Given parameters

        /// <summary>
        /// Uri for the website url which should be parsed
        /// </summary>
        private Uri _webSiteUrl;

        #endregion Given parameters

        #region Properties

        /// <summary>
        /// Type of the content load
        /// Web: Load content of the given URL for the parsing
        /// Text: A given text will be used for the parsing
        /// </summary>
        public DataTypes.LoadType LoadingType { get; set; }

        /// <summary>
        /// Type for the parsing
        /// Regex: Parse the content by the given regex list
        /// OnVistaRealTime: Serialize the content to a JSON object and then copy it to the regex result
        /// OnVistaHistoryData: Serialize the content to a JSON object and then copy it to the daily values result
        /// </summary>
        public DataTypes.ParsingType ParsingType { get; set; }

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
                // Save given website url
                GivenWebSiteUrl = value != null ? value.ToString() : @"Nothing given";

                try
                {
                    // Check if the website url is valid
                    //                String regexPattern = @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$";
                    //                if (System.Text.RegularExpressions.Regex.IsMatch(value, regexPattern))
                    if (value != null && value.ToString() != @"")
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
        /// String for the API key
        /// </summary>
        public string ApiKey { get; internal set; }

        /// <summary>
        /// Uri for the website which is original given to the parser
        /// Used for later usage ( e.g. logging )
        /// </summary>
        public string GivenWebSiteUrl { get; private set; }

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
        /// Parsing values for parsing a given text with the given regex list values
        /// </summary>
        /// <param name="parsingText">Text which should be parsed</param>
        /// <param name="encoding">Encoding of the text</param>
        /// <param name="regexList">Regex list for the parsing</param>
        public ParsingValues(string parsingText, string encoding, RegExList regexList)
        {
            LoadingType = DataTypes.LoadType.Text;
            ParsingType = DataTypes.ParsingType.Regex;
            WebSiteUrl = null;
            ApiKey = null;
            ParsingText = parsingText;
            EncodingType = encoding;
            RegexList = regexList;
        }

        /// <summary>
        /// Parsing values for parsing the loaded URL content with the given regex list
        /// </summary>
        /// <param name="webSiteUrl">URL from which the content should be loaded which should be parsed</param>
        /// <param name="encoding">Encoding of the URL website</param>
        /// <param name="regexList">Regex list for the parsing</param>
        public ParsingValues(Uri webSiteUrl, string encoding, RegExList regexList)
        {
            LoadingType = DataTypes.LoadType.Web;
            ParsingType = DataTypes.ParsingType.Regex;
            WebSiteUrl = webSiteUrl;
            ApiKey = null;
            ParsingText = null;
            EncodingType = encoding;
            RegexList = regexList;
        }

        /// <summary>
        /// Parsing values for parsing the loaded URL content as JSON and map the JSON values to the parsing result
        /// </summary>
        /// <param name="webSiteUrl">URL from which the content should be loaded as JSON</param>
        /// <param name="apiKey">Key for the API</param>
        /// <param name="encoding">Encoding of the URL website</param>
        /// <param name="parsingType">Parsing type</param>
        public ParsingValues(Uri webSiteUrl, string apiKey, string encoding, DataTypes.ParsingType parsingType)
        {

            ParsingType = parsingType;
            WebSiteUrl = webSiteUrl;
            ApiKey = apiKey;
            ParsingText = null;
            EncodingType = encoding;
            RegexList = null;
        }
    }
}
