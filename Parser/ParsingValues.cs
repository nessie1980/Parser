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
        /// Type for the parsing
        /// WebParsing: A given url will be loaded and the website content will be parsed by the given regex list
        /// TextParsing: A given text will be parsed by the given regex list
        /// DailyValuesParing: A given url will be loaded and the website content will be parsed for the daily values
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
        /// Parsing values with a text which should be parsed
        /// </summary>
        /// <param name="parsingText">Text which should be parsed</param>
        /// <param name="encoding">Encoding of the text</param>
        /// <param name="regexList">Regex list for the parsing</param>
        public ParsingValues(string parsingText, string encoding, RegExList regexList)
        {
            ParsingType = DataTypes.ParsingType.TextParsing;
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
            ParsingType = DataTypes.ParsingType.WebParsing;
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
            ParsingType = DataTypes.ParsingType.DailyValuesParsing;
            WebSiteUrl = webSiteUrl;
            ParsingText = null;
            EncodingType = encoding;
            RegexList = null;
        }
    }
}
