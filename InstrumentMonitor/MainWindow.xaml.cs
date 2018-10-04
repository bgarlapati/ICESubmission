using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PricingSource.Contract;
using PricingSource.Impl;


namespace InstrumentMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InstrumentMonitorViewModel myStockMonitorViewModel;
        IList<string> mySupportedForPricingSource1 = new List<string>() { "AAPL", "WMT", "FB", "MSFT", "TSLA", "INTC", "JNJ" };
        IList<string> mySupportedForPricingSource2 = new List<string>() { "XOM", "MMM", "AXP", "BA", "CAT", "CVX", "CSCO", "KO", "WBA", "AA" };

        public MainWindow()
        {
            InitializeComponent();

            //The following glueing logic is configurable with any dependency inversion supported frameworks ( such as spring.net)
            //Setup Pricing Sources
            var techPricingSource = new SimulatedPricingSource(mySupportedForPricingSource1);
            var nonTechPricingSource = new SimulatedPricingSource(mySupportedForPricingSource2);

            //Adding to Composite Pricing Source
            var compositePricingSource = new CompositePricingSource(new List<IPricingSource>() { techPricingSource, nonTechPricingSource });

            //Creating PricingEngine with Composite pricing source.
            var pricingEngine = new PricingEngine.Impl.PricingEngine(compositePricingSource, 300);

            myStockMonitorViewModel = new InstrumentMonitorViewModel(pricingEngine);
            DataContext = myStockMonitorViewModel;
        }

        
        private void OnListViewItemSelected(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                if (listView.SelectedItem != null)
                {
                    myStockMonitorViewModel.ItemToRemove = (InstrumentItem)listView.SelectedItem;
                }
            }
        }
    }
}
