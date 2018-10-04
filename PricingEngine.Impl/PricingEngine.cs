using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

using PricingEngine.Contract;

using PricingSource.Contract;

using Timer = System.Timers.Timer;


namespace PricingEngine.Impl
{
    /// <summary>
    /// Simulated pricing source.
    /// </summary>
    public class PricingEngine : IPricingEngine
    {
        #region private members.
        private readonly IPricingSource myPricingSource;
        private readonly IDictionary<string, double> myCurrentInstrumentsWithPrices = new Dictionary<string, double>();
        private Timer myTimer = new Timer();
        private readonly Dispatcher myDispatcherToOwnerThread;
        #endregion

        #region public methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pricingSource">Pricing source </param>
        /// <param name="updateInterval"> interval between price updates in milli seconds </param>
        public PricingEngine(IPricingSource pricingSource, int updateInterval)
        {
            
            myPricingSource = pricingSource;
            myDispatcherToOwnerThread = Dispatcher.FromThread(Thread.CurrentThread);
            myTimer.Interval = updateInterval;
            myTimer.Elapsed += OnTimerElapsed;
        }

        
        //<see cref = "IPricingSource.Start" />.
        public void Start()
        {
            myTimer.Start();
        }

        //<see cref = "IPricingSource.StartMonitoring" />.
        public void StartMonitoring(IList<string> instrumentSymbols)
        {
            lock (myCurrentInstrumentsWithPrices)
            {
                foreach (var instrumentSymbol in instrumentSymbols)
                {
                    if (!myCurrentInstrumentsWithPrices.ContainsKey(instrumentSymbol))
                    {
                        myCurrentInstrumentsWithPrices.Add(instrumentSymbol, myPricingSource.GetPrice(instrumentSymbol));
                    }
                }
            }
        }


        //<see cref = "IPricingSource.StopMonitoring" />.
        public void StopMonitoring(IList<string> instrumentSymbols)
        {
            lock (myCurrentInstrumentsWithPrices)
            {
                foreach (var instrumentSymbol in instrumentSymbols)
                {
                    if (myCurrentInstrumentsWithPrices.ContainsKey(instrumentSymbol))
                    {
                        myCurrentInstrumentsWithPrices.Remove(instrumentSymbol);
                    }
                }
            }
        }

        //<see cref = "IPricingSource.Stop" />.
        public void Stop()
        {
            myTimer.Stop();
        }

        //<see cref = "IPricingSource.InstrumentPricesChanged" />.
        public event EventHandler<InstrumentPricesEventArgs> InstrumentPricesChanged;

        #endregion

        #region Private Methods
       
        
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //Timer event may get called in a different thread, hence re-dispatch in the thread where this object is 
            //created.
            if (myDispatcherToOwnerThread != null)
            {
                DispatchToCallingThread(UpdatePricesAndNotify);
            }
        }

        private void DispatchToCallingThread(Action theAction)
        {
            if (myDispatcherToOwnerThread == null || myDispatcherToOwnerThread.CheckAccess())
            {
                theAction();
            }
            else
            {
                myDispatcherToOwnerThread.BeginInvoke(theAction);
            }
        }

        private void UpdatePricesAndNotify()
        {
            //Update prices of all instruments randomly
            UpdatedPrices();

            NotifyNewPrices();
        }

        private void NotifyNewPrices()
        {
            lock (myCurrentInstrumentsWithPrices)
            {
                InstrumentPricesChanged?.Invoke(this, new InstrumentPricesEventArgs(myCurrentInstrumentsWithPrices.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)));
            }
        }

        private void UpdatedPrices()
        {
            lock (myCurrentInstrumentsWithPrices)
            {
                foreach (var currentInstrument in myCurrentInstrumentsWithPrices.Keys.ToList())
                {
                    myCurrentInstrumentsWithPrices[currentInstrument] = myPricingSource.GetPrice(currentInstrument);
                }
            }
        }

       

        #endregion
    }
}
