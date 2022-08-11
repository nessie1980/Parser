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

using System.Collections.Generic;

namespace Parser.JsonObjects.Yahoo
{
    public class QuoteResponse
    {
        public List<Result> result;
        public object error;
    }

    public partial class Result
    {
        public string language;
        public string region;
        public string quoteType;
        public string typeDisp;
        public string quoteSourceName;
        public bool triggerable;
        public string customPriceAlertConfidence;
        public string currency;
        public string exchange;
        public string shortName;
        public string longName;
        public string messageBoardId;
        public string exchangeTimezoneName;
        public string exchangeTimezoneShortName;
        public int gmtOffSetMilliseconds;
        public string market;
        public bool esgPopulated;
        public string marketState;
        public int priceHint;
        public double postMarketChangePercent;
        public int postMarketTime;
        public double postMarketPrice;
        public double postMarketChange;
        public double regularMarketChange;
        public double regularMarketChangePercent;
        public int regularMarketTime;
        public double regularMarketPrice;
        public double regularMarketDayHigh;
        public string regularMarketDayRange;
        public double regularMarketDayLow;
        public int regularMarketVolume;
        public double regularMarketPreviousClose;
        public double bid;
        public double ask;
        public int bidSize;
        public int askSize;
        public string fullExchangeName;
        public string financialCurrency;
        public double regularMarketOpen;
        public int averageDailyVolume3Month;
        public int averageDailyVolume10Day;
        public double fiftyTwoWeekLowChange;
        public double fiftyTwoWeekLowChangePercent;
        public string fiftyTwoWeekRange;
        public double fiftyTwoWeekHighChange;
        public bool tradeable;
        public double fiftyTwoWeekHighChangePercent;
        public double fiftyTwoWeekLow;
        public double fiftyTwoWeekHigh;
        public int dividendDate;
        public int earningsTimestamp;
        public int earningsTimestampStart;
        public int earningsTimestampEnd;
        public double trailingAnnualDividendRate;
        public double trailingPE;
        public double trailingAnnualDividendYield;
        public double epsTrailingTwelveMonths;
        public double epsForward;
        public double epsCurrentYear;
        public double priceEpsCurrentYear;
        public long sharesOutstanding;
        public double bookValue;
        public double fiftyDayAverage;
        public double fiftyDayAverageChange;
        public double fiftyDayAverageChangePercent;
        public double twoHundredDayAverage;
        public double twoHundredDayAverageChange;
        public double twoHundredDayAverageChangePercent;
        public long marketCap;
        public double forwardPE;
        public double priceToBook;
        public int sourceInterval;
        public int exchangeDataDelayedBy;
        public double pageViewGrowthWeekly;
        public string averageAnalystRating;
        public long firstTradeDateMilliseconds;
        public string displayName;
        public string symbol;
    }

    public class RealTimeData
    {
        public QuoteResponse quoteResponse;
    }
}
