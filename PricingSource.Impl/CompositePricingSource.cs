using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PricingSource.Contract;


namespace PricingSource.Impl
{
    /// <summary>
    /// composite pricing source.
    /// </summary>
    public class CompositePricingSource : IPricingSource
    {
        #region private members.
        private readonly IList<IPricingSource> myPricingSources = new List<IPricingSource>();

        #endregion

        #region public methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pricingSources">Pricing sources </param>
        public CompositePricingSource(IList<IPricingSource> pricingSources)
        {
            myPricingSources = pricingSources;
        }


        public IEnumerable<string> SupportedInstruments {
            get
            {
                List<string> supportedInstruments = new List<string>();
                foreach (var pricingSource in myPricingSources)
                {
                    supportedInstruments.AddRange(pricingSource.SupportedInstruments.ToList());
                }

                return supportedInstruments;
            }

        }
        public double GetPrice(string instrument)
        {
            var pricingSource = GetPricingSource(instrument);
            return pricingSource.GetPrice(instrument);
        }

        #endregion


        #region private methods

        private IPricingSource GetPricingSource(string instrumentSymbol)
        {
            foreach (var pricingSource in myPricingSources)
            {
                if (pricingSource.SupportedInstruments.Contains(instrumentSymbol))
                {
                    return pricingSource;
                }
            }
            throw new InvalidDataException("Instrument is not supported. See ReadMe ");
        }

        #endregion
    }
}
