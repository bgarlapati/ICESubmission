using System;
using System.Collections.Generic;


namespace PricingEngine.Contract
{
    public interface IPricingEngine
    {
        /// <summary>
        /// Starts the engine.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Start monitoring for the symbols. This will start monitoring the given instruments
        /// in addition to what it is already monitoring.
        /// </summary>
        /// <param name="instrumentSymbols">list of instruments to monitor</param>
        void StartMonitoring(IList<string> instrumentSymbols);

        /// <summary>
        /// Stop monitoring for the symbols. This will stop monitoring the given instruments
        /// </summary>
        /// <param name="instrumentSymbols">list of instruments to stop monitoring</param>
        void StopMonitoring(IList<string> instrumentSymbols);


        /// <summary>
        /// Stops the pricing engine. Stops monitoring for all the instruments.
        /// </summary>
        void Stop();

        /// <summary>
        /// Event raised when instrument prices changed
        /// </summary>
        event EventHandler<InstrumentPricesEventArgs> InstrumentPricesChanged;
    }
}
