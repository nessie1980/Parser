//MIT License
//
//Copyright(c) 2018 - 2022 nessie1980(nessie1980@gmx.de)
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

using System.Collections.Generic;

namespace Parser.JsonObjects.Yahoo
{
    public class QuoteResponse
    {
        public List<Result> Result { get; set; }

        /* Unused properties of the class
        public object Error;
        */
    }

    public partial class Result
    {
        /* Unused properties of the class
        public string Language;
        public string Region;
        public string QuoteType;
        public string TypeDisp;
        public string QuoteSourceName;
        public bool Triggerable;
        public string CustomPriceAlertConfidence;
        */
        public string Currency { get; set; }

        /* Unused properties of the class
        public string Exchange;
        public string ShortName;
        public string LongName;
        public string MessageBoardId;
        public string ExchangeTimezoneName;
        public string ExchangeTimezoneShortName;
        public int GmtOffSetMilliseconds;
        public string Market;
        public bool EsgPopulated;
        public string MarketState;
        public int PriceHint;
        public double PostMarketChangePercent;
        public int PostMarketTime;
        public double PostMarketPrice;
        public double PostMarketChange;
        public double RegularMarketChange;
        public double RegularMarketChangePercent;
        */

        public int RegularMarketTime { get; set; }
        public double RegularMarketPrice { get; set; }

        /* Unused properties of the class
        public double RegularMarketDayHigh;
        public string RegularMarketDayRange;
        public double RegularMarketDayLow;
        public int RegularMarketVolume;
        */

        public double RegularMarketPreviousClose { get; set; }

        /* Unused properties of the class
        public double Bid;
        public double Ask;
        public int BidSize;
        public int AskSize;
        public string FullExchangeName;
        public string FinancialCurrency;
        public double RegularMarketOpen;
        public int AverageDailyVolume3Month;
        public int AverageDailyVolume10Day;
        public double FiftyTwoWeekLowChange;
        public double FiftyTwoWeekLowChangePercent;
        public string FiftyTwoWeekRange;
        public double FiftyTwoWeekHighChange;
        public bool Tradeable;
        public double FiftyTwoWeekHighChangePercent;
        public double FiftyTwoWeekLow;
        public double FiftyTwoWeekHigh;
        public int DividendDate;
        public int EarningsTimestamp;
        public int EarningsTimestampStart;
        public int EarningsTimestampEnd;
        public double TrailingAnnualDividendRate;
        public double TrailingPe;
        public double TrailingAnnualDividendYield;
        public double EpsTrailingTwelveMonths;
        public double EpsForward;
        public double EpsCurrentYear;
        public double PriceEpsCurrentYear;
        public long SharesOutstanding;
        public double GookValue;
        public double FiftyDayAverage;
        public double FiftyDayAverageChange;
        public double FiftyDayAverageChangePercent;
        public double TwoHundredDayAverage;
        public double TwoHundredDayAverageChange;
        public double TwoHundredDayAverageChangePercent;
        public long MarketCap;
        public double ForwardPe;
        public double PriceToBook;
        public int SourceInterval;
        public int ExchangeDataDelayedBy;
        public double PageViewGrowthWeekly;
        public string AverageAnalystRating;
        public long FirstTradeDateMilliseconds;
        public string DisplayName;
        public string Symbol;
        */
    }

    public class RealTimeData
    {
        public QuoteResponse QuoteResponse { get; set; }
    }
}
