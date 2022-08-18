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

namespace Parser.JsonObjects.OnVista
{
    internal class RealTimeData
    {
        /* Unused properties of the class
        public float First { get; set; }
        public DateTimeFirst DatetimeFirst { get; set; }
        */

        public float Price { get; set; }
        public DateTimePrice DatetimePrice { get; set; }

        /* Unused properties of the class
        public int Volume { get; set; }
        public object Addendum { get; set; }
        public int Money { get; set; }
        public float High { get; set; }
        public DateTimeHigh DatetimeHigh { get; set; }
        public float Low { get; set; }
        public DateTimeLow DatetimeLow { get; set; }
        public int TotalVolume { get; set; }
        public float TotalMoney { get; set; }
        public int NumberPrices { get; set; }
        */

        public float PreviousLast { get; set; }

        /* Unused properties of the class
        public DateTimePreviousLast DatetimePreviousLast { get; set; }
        public int IdTypePriceTotals { get; set; }
        public float Performance { get; set; }
        public float PerformancePct { get; set; }
        public float Ask { get; set; }
        public DateTimeAsk DatetimeAsk { get; set; }
        public int VolumeAsk { get; set; }
        public object MoneyAsk { get; set; }
        public object NumberOrdersAsk { get; set; }
        public float Bid { get; set; }
        public DateTimeBid DatetimeBid { get; set; }
        public int VolumeBid { get; set; }
        public object MoneyBid { get; set; }
        public object NumberOrdersBid { get; set; }
        public int Volume4Weeks { get; set; }
        public float HighPrice1Year { get; set; }
        public DateTimeHighPrice1Year DatetimeHighPrice1Year { get; set; }
        public float LowPrice1Year { get; set; }
        public DateTimeLowPrice1Year DatetimeLowPrice1Year { get; set; }
        public float Performance1Year { get; set; }
        public float Performance1YearPct { get; set; }
        public int IdNotation { get; set; }
        public int IdTimezone { get; set; }
        public int IdExchange { get; set; }
        public string CodeExchange { get; set; }
        public int IdContributor { get; set; }
        public string CodeContributor { get; set; }
        public int IdCurrency { get; set; }
        */

        public string IsoCurrency { get; set; }

        /* Unused properties of the class
        public int IdTradingSchedule { get; set; }
        public int IdQualityPrice { get; set; }
        public string CodeQualityPrice { get; set; }
        public int IdSalesProduct { get; set; }
        public int IdQualityPriceBidAsk { get; set; }
        public string CodeQualityPriceBidAsk { get; set; }
        public int IdSalesProductBidAsk { get; set; }
        public int IdInstrument { get; set; }
        public int IdTypeInstrument { get; set; }
        public string CodeTool { get; set; }
        public int IdUnitPrice { get; set; }
        public int Amount { get; set; }
        public object SourcePrice { get; set; }
        public object SourceAsk { get; set; }
        public object SourceBid { get; set; }
        public int PropertyFlagsPrice { get; set; }
        public int PropertyFlagsAsk { get; set; }
        public int PropertyFlagsBid { get; set; }
        */
    }

    /* Unused class
    public class DateTimeFirst
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    public class DateTimePrice
    {

        public string LocalTime { get; set; }

        /* Unused properties of the class
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
        */
    }

    /* Unused class
    public class DateTimeHigh
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimeLow
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimePreviousLast
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimeAsk
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimeBid
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimeHighPrice1Year
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */

    /* Unused class
    public class DateTimeLowPrice1Year
    {
        public string LocalTime { get; set; }
        public string LocalTimeZone { get; set; }
        public int UtcTimeStamp { get; set; }
    }
    */
}
