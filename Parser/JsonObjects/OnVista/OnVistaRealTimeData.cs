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

namespace Parser.JsonObjects.OnVista
{
    internal class RealTimeData
    {

        //public float first { get; set; }

        //public DateTimeFirst datetimeFirst { get; set; }

        public float Price { get; set; }

        public DateTimePrice DatetimePrice { get; set; }

        //public int volume { get; set; }

        //public object addendum { get; set; }

        //public int money { get; set; }

        //public float high { get; set; }

        //public DateTimeHigh datetimeHigh { get; set; }

        //public float low { get; set; }

        //public DateTimeLow datetimeLow { get; set; }

        //public int totalVolume { get; set; }

        //public float totalMoney { get; set; }

        //public int numberPrices { get; set; }

        public float PreviousLast { get; set; }

        //public DateTimePreviousLast datetimePreviousLast { get; set; }

        //public int idTypePriceTotals { get; set; }

        //public float performance { get; set; }

        //public float performancePct { get; set; }

        //public float ask { get; set; }

        //public DateTimeAsk datetimeAsk { get; set; }

        //public int volumeAsk { get; set; }

        //public object moneyAsk { get; set; }

        //public object numberOrdersAsk { get; set; }

        //public float bid { get; set; }

        //public DateTimeBid datetimeBid { get; set; }

        //public int volumeBid { get; set; }

        //public object moneyBid { get; set; }

        //public object numberOrdersBid { get; set; }

        //public int volume4Weeks { get; set; }

        //public float highPrice1Year { get; set; }

        //public DateTimeHighPrice1Year datetimeHighPrice1Year { get; set; }

        //public float lowPrice1Year { get; set; }

        //public DateTimeLowPrice1Year datetimeLowPrice1Year { get; set; }

        //public float performance1Year { get; set; }

        //public float performance1YearPct { get; set; }

        public int IdNotation { get; set; }

        //public int idTimezone { get; set; }

        //public int idExchange { get; set; }

        //public string codeExchange { get; set; }

        //public int idContributor { get; set; }

        //public string codeContributor { get; set; }

        public int IdCurrency { get; set; }

        public string IsoCurrency { get; set; }

        //public int idTradingSchedule { get; set; }

        //public int idQualityPrice { get; set; }

        //public string codeQualityPrice { get; set; }

        //public int idSalesProduct { get; set; }

        //public int idQualityPriceBidAsk { get; set; }

        //public string codeQualityPriceBidAsk { get; set; }

        //public int idSalesProductBidAsk { get; set; }

        //public int idInstrument { get; set; }

        //public int idTypeInstrument { get; set; }

        //public string codeTool { get; set; }

        //public int idUnitPrice { get; set; }

        //public int amount { get; set; }

        //public object sourcePrice { get; set; }

        //public object sourceAsk { get; set; }

        //public object sourceBid { get; set; }

        //public int propertyFlagsPrice { get; set; }

        //public int propertyFlagsAsk { get; set; }

        //public int propertyFlagsBid { get; set; }
    }

    //public class DateTimeFirst
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    public class DateTimePrice
    {

        public string LocalTime { get; set; }

        public string LocalTimeZone { get; set; }

        public int UtcTimeStamp { get; set; }
    }

    //public class DateTimeHigh
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    //public class DateTimeLow
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    //public class DateTimePreviousLast
    //{

    //    public string LocalTime { get; set; }

    //    public string LocalTimeZone { get; set; }

    //    public int UtcTimeStamp { get; set; }
    //}

    //public class DateTimeAsk
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    //public class DateTimeBid
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    //public class DateTimeHighPrice1Year
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}

    //public class DateTimeLowPrice1Year
    //{

    //    public string localTime { get; set; }

    //    public string localTimeZone { get; set; }

    //    public int utcTimeStamp { get; set; }
    //}
}
