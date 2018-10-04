using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PricingEngine.Contract
{
    public class InstrumentPricesEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instrumentWithCurrentPrices">current instrument prices</param>
        public InstrumentPricesEventArgs(IEnumerable<KeyValuePair<string, double>> instrumentWithCurrentPrices)
        {
            CurrentInstrumentsPrices = instrumentWithCurrentPrices;
        }
        
        /// <summary>
        /// Returns the current instrument with prices.
        /// </summary>
        public IEnumerable<KeyValuePair<string, double>> CurrentInstrumentsPrices { get; }
    }
}
