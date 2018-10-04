using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using PricingEngine.Contract;


namespace InstrumentMonitor
{
    /// <summary>
    /// View model for the instrument monitor.
    /// </summary>
    public class InstrumentMonitorViewModel: INotifyPropertyChanged
    {
        #region private data members

        private bool myIsMonitorStarted;
        private RelayCommand myAddItemCommand;
        private RelayCommand myRemoveItemCommand;
        private RelayCommand myStartMonitorCommand;
        private RelayCommand myStopMonitorCommand;
        private IPricingEngine myPricingEngine;
        private string myErrorText;

        #endregion

        #region public members

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pricingEngine">Takes pricing engine as input</param>
        public InstrumentMonitorViewModel(IPricingEngine pricingEngine)
        {
            CurrentInstrumentItems = new BindingList<InstrumentItem>();
            myIsMonitorStarted = false;
            myPricingEngine = pricingEngine;
        }

        /// <summary>
        /// List of instrument items
        /// </summary>
        public BindingList<InstrumentItem> CurrentInstrumentItems {get; private set; }

        /// <summary>
        /// Item to remove. Represents currently selected view item.
        /// </summary>
        public InstrumentItem ItemToRemove { get; set; }

        /// <summary>
        /// Represents the item to add
        /// </summary>
        public string ItemToAdd { get; set; }


        /// <summary>
        /// Error text or user guidance information.
        /// </summary>
        public string ErrorText
        {
            get { return myErrorText; }
            set
            {
                if (myErrorText != value)
                {
                    myErrorText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ErrorText"));
                }
            }
        }


        public ICommand AddItemCommand
        {
            get
            {
                if (myAddItemCommand == null)
                {
                    myAddItemCommand = new RelayCommand(param => AddItem(), param=>CanItemBeAdded());
                }

                return myAddItemCommand;
            }
        }

        public ICommand RemoveItemCommand
        {
            get
            {
                if (myRemoveItemCommand == null)
                {
                    myRemoveItemCommand = new RelayCommand(param => RemoveItem(), param => CanItemBeRemoved());
                }

                return myRemoveItemCommand;
            }
        }
        public ICommand StartMonitorCommand
        {
            get
            {
                if (myStartMonitorCommand == null)
                {
                    myStartMonitorCommand = new RelayCommand(param => StartMonitor(), param => !myIsMonitorStarted);
                }

                return myStartMonitorCommand;
            }
        }

        public ICommand StopMonitorCommand
        {
            get
            {
                if (myStopMonitorCommand == null)
                {
                    myStopMonitorCommand = new RelayCommand(param => StopMonitor(), param => myIsMonitorStarted);
                }

                return myStopMonitorCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region private methods

        private void AddItem()
        {
            if (!CurrentInstrumentItems.Any(item => item.Symbol == ItemToAdd.ToUpper()))
            {
                ErrorText = "";

                try
                {
                    myPricingEngine.StartMonitoring(new List<string>() {ItemToAdd.ToUpper()});
                }
                catch (InvalidDataException ex)
                {
                    ErrorText = ex.Message;
                }
            }
            else
            {
                ErrorText = string.Format(" {0} already in the list!", ItemToAdd);
            }
        }

        private void RemoveItem()
        {
            ErrorText = "";
            try
            {
                myPricingEngine.StopMonitoring(new List<string>() { ItemToRemove.Symbol });
            }
            catch (InvalidDataException ex)
            {
                ErrorText = ex.Message;
            }
            
            ItemToRemove = null;
        }

        private bool CanItemBeAdded()
        {
            return !string.IsNullOrWhiteSpace(ItemToAdd) && myIsMonitorStarted;
        }

        private bool CanItemBeRemoved()
        {
            return ItemToRemove != null;
        }

        private void StartMonitor()
        {
            myIsMonitorStarted = true;

            // Start the engine
            myPricingEngine.Start();
            myPricingEngine.InstrumentPricesChanged += OnInstrumentPricesChanged;

        }

        private void OnInstrumentPricesChanged(object sender, InstrumentPricesEventArgs e)
        {
            var updatedItems = new List<InstrumentItem>();
            foreach (var changedItem in e.CurrentInstrumentsPrices)
            {
                updatedItems.Add(new InstrumentItem(changedItem.Key, changedItem.Value));
            }
            UpdateStockPrices(updatedItems);
        }

        private void StopMonitor()
        {
            myIsMonitorStarted = false;

            // Stop engine            
            myPricingEngine.InstrumentPricesChanged += OnInstrumentPricesChanged;
            myPricingEngine.Stop();
        }

        
        /// <summary>
        /// Updates stock prices for the subscribed instruments in the internal list.
        /// </summary>
        /// <param name="updatedInstruments"></param>
        private void UpdateStockPrices(IList<InstrumentItem> updatedInstruments)
        {
            //Update the price or add any new items.
            foreach (var updatedInstrument in updatedInstruments)
            {
                var stockItemToModify = CurrentInstrumentItems.Where(item => item.Symbol == updatedInstrument.Symbol).FirstOrDefault();
                if (stockItemToModify != null)
                {
                    stockItemToModify.Price = updatedInstrument.Price;
                }
                else
                {
                    CurrentInstrumentItems.Add(new InstrumentItem(updatedInstrument.Symbol.ToUpper(), updatedInstrument.Price));
                }
            }

            //Remove any items that may not be present.
            foreach (var currentStockItem in CurrentInstrumentItems.ToList())
            {
                var stockItemToRemove = updatedInstruments.FirstOrDefault(item => item.Symbol == currentStockItem.Symbol);
                if (stockItemToRemove == null)
                {
                    CurrentInstrumentItems.Remove(currentStockItem);
                }
            }
        }
        
        #endregion

    }
}
