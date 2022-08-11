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
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Adjclose
    {
        public List<double> AdjcloseData { get; set; }
    }

    public class Chart
    {
        public List<Result> Result { get; set; }
        public object Error { get; set; }
    }

    public class CurrentTradingPeriod
    {
        public Pre Pre { get; set; }
        public Regular Regular { get; set; }
        public Post Post { get; set; }
    }

    public class Indicators
    {
        public List<Quote> Quote { get; set; }
        public List<Adjclose> Adjclose { get; set; }
    }

    public class Meta
    {
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public string ExchangeName { get; set; }
        public string InstrumentType { get; set; }
        public int FirstTradeDate { get; set; }
        public int RegularMarketTime { get; set; }
        public int Gmtoffset { get; set; }
        public string Timezone { get; set; }
        public string ExchangeTimezoneName { get; set; }
        public double RegularMarketPrice { get; set; }
        public double ChartPreviousClose { get; set; }
        public int PriceHint { get; set; }
        public CurrentTradingPeriod CurrentTradingPeriod { get; set; }
        public string DataGranularity { get; set; }
        public string Range { get; set; }
        public List<string> ValidRanges { get; set; }
    }

    public class Post
    {
        public string Timezone { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Gmtoffset { get; set; }
    }

    public class Pre
    {
        public string Timezone { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Gmtoffset { get; set; }
    }

    public class Quote
    {
        public List<double> Low { get; set; }
        public List<int> Volume { get; set; }
        public List<double> High { get; set; }
        public List<double> Close { get; set; }
        public List<double> Open { get; set; }
    }

    public class Regular
    {
        public string Timezone { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Gmtoffset { get; set; }
    }

    public partial class Result
    {
        public Meta Meta { get; set; }
        public List<int> Timestamp { get; set; }
        public Indicators Indicators { get; set; }
    }

    public class HistoryData
    {
        public Chart Chart { get; set; }
    }




}
