using System.ComponentModel;

namespace InstrumentMonitor
{
    /// <summary>
    /// Stock instrument information
    /// </summary>
    public class InstrumentItem : INotifyPropertyChanged
    {
        private double myPrice;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instrumentSymbol">instrument symbol</param>
        /// <param name="price">price of the instrument</param>
        public InstrumentItem(string instrumentSymbol, double price)
        {
            Symbol = instrumentSymbol;
            Price = price;
        }

        /// <summary>
        /// Instrument symbol
        /// </summary>
        public string Symbol { get; set; }


        /// <summary>
        /// Instrument price
        /// </summary>
        public double Price
        {
            get => myPrice;
            set
            {
                if (value != myPrice)
                {
                    myPrice = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Price"));
                }
            }
        }

        /// <summary>
        /// Property Changed event of INotifyPropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
