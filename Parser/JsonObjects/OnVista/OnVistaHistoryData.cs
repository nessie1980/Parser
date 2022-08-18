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
    internal class HistoryData
    {
        /* Unused properties of the class
        public string EntityType { get; set; }
        public string EntityValue { get; set; }
        public Market Market { get; set; }
        public int IdInstrument { get; set; }
        public int IdNotation { get; set; }
        public string IsoCurrency { get; set; }
        public string UnitType { get; set; }
        public DateTime DatetimeStartAvailableHistory { get; set; }
        public DateTime DatetimeEndAvailableHistory { get; set; }
        public int IdTradingSchedule { get; set; }
        */

        public int[] DatetimeLast { get; set; }
        public float[] First { get; set; }
        public float[] Last { get; set; }
        public float[] High { get; set; }
        public float[] Low { get; set; }
        public float[] Volume { get; set; }

        /* Unused properties of the class
        public int[] NumberPrices { get; set; }
        public string DisplayUnit { get; set; }
        */
    }

    /* Unused class
    public class Market
    {
        public string Name { get; set; }
        public string CodeMarket { get; set; }
        public string NameExchange { get; set; }
        public string CodeExchange { get; set; }
        public int IdNotation { get; set; }
        public string IsoCountry { get; set; }
    }
    */
}
