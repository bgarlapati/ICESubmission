using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PricingSource.Contract
{
    public interface IPricingSource
    {
        /// <summary>
        /// Provides list of instrument symbols this pricing source supports.
        /// </summary>
        /// <returns>supported instruments for price check</returns>
        IEnumerable<string> SupportedInstruments { get; }

        /// <summary>
        /// Gets the price of instrument
        /// </summary>
        double GetPrice( string instrument);

    }
}
