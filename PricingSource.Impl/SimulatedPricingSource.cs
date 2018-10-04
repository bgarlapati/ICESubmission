using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PricingSource.Contract;


namespace PricingSource.Impl
{
    /// <summary>
    /// Simulated pricing source.
    /// </summary>
    public class SimulatedPricingSource : IPricingSource
    {
        #region private members.
        private readonly IDictionary<string, double> myInstrumentsWithPrices = new Dictionary<string, double>();
        private readonly Random myRandomNumberGenerator;
        #endregion

        #region public methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="supportedInstrumentList">Supported instruments this source supports</param>
        public SimulatedPricingSource(IList<string> supportedInstrumentList)
        {
            myRandomNumberGenerator = new Random();
            InitializePrices(supportedInstrumentList);
        }

        //<see cref = "IPricingSource.SupportedInsruments" />.
        public IEnumerable<string> SupportedInstruments
        {
            get
            {
                lock (myInstrumentsWithPrices)
                {
                    return myInstrumentsWithPrices.Keys.ToArray();
                }
            }
        } 

        //<see cref = "IPricingSource.GetPrice" />.
        public double GetPrice(string instrument)
        {
           
                if (myInstrumentsWithPrices.ContainsKey(instrument))
                {
                    UpdatedPrice(instrument);
                    return myInstrumentsWithPrices[instrument];
                }
                else
                {
                    throw new InvalidDataException("Not a supported instrument for pricing");
                }
            
        }
        
       
        #endregion

        #region Private Methods

        //<see cref = "IPricingSource.StartMonitoring" />.
        private void InitializePrices(IList<string> instrumentSymbols)
        {
            
                foreach (var instrumentSymbol in instrumentSymbols)
                {
                    if (!myInstrumentsWithPrices.ContainsKey(instrumentSymbol))
                    {
                        myInstrumentsWithPrices.Add(instrumentSymbol, GetRandomPrice());
                    }
                }
            
        }

        //Generating inital price 
        private double GetRandomPrice()
        {
            return myRandomNumberGenerator.Next(1, 100) + myRandomNumberGenerator.NextDouble();
        }

        private bool GetRandomBoolean()
        {
           return myRandomNumberGenerator.Next(0, 2) == 1;
        }

        //Updating the price of the instrument by 1% either increment/decrement 
       private void UpdatedPrice(string instrument)
        {
           
                    if (GetRandomBoolean())
                    {
                        myInstrumentsWithPrices[instrument] +=
                            myInstrumentsWithPrices[instrument] * 0.01;
                    }
                    else
                    {
                        myInstrumentsWithPrices[instrument] -=
                            myInstrumentsWithPrices[instrument] * 0.01;
                    }
        }
        
        #endregion

    }
}
