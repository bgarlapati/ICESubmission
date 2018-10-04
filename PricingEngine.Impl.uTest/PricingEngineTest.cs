using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

using PricingEngine.Contract;
using PricingSource.Contract;


namespace PricingEngine.Impl.uTest
{
    [TestFixture]
    public class PricingEngineTest
    {
        private IPricingEngine myTestee;
        private readonly Mock<IPricingSource> myPricingSource1 = new Mock<IPricingSource>();
                private IEnumerable<KeyValuePair<string, double>> myReceivedInstrumentPrices;
        int myUpdateInterval;

        [SetUp]
        public void TestSetup()
        {
            myPricingSource1.SetupGet(m=> m.SupportedInstruments).Returns(new List<string>(){"a","b","c","d"});
            myPricingSource1.Setup(m => m.GetPrice("a")).Returns(10.25);
            myPricingSource1.Setup(m => m.GetPrice("b")).Returns(15.25);
            myPricingSource1.Setup(m => m.GetPrice("c")).Returns(80.25);
            myPricingSource1.Setup(m => m.GetPrice("d")).Returns(45.25);
            myReceivedInstrumentPrices = null;
            myUpdateInterval = 10; //Updates every 10ms

            myTestee = new PricingEngine(myPricingSource1.Object, myUpdateInterval );
            myTestee.InstrumentPricesChanged += OnInstrumentPricesChanged;
        }

        private void OnInstrumentPricesChanged(object sender, InstrumentPricesEventArgs e)
        {
            myReceivedInstrumentPrices = e.CurrentInstrumentsPrices;
        }

        [TearDown]
        public void TestTeardown()
        {
            myTestee.InstrumentPricesChanged -= OnInstrumentPricesChanged;
            myTestee = null;
        }


        [Test]
        public void TestStartMonitoring_ReceivesPriceNotifications()
        {
            //Arrange
            myTestee.Start();

            //Act
            myTestee.StartMonitoring(new List<string>(){"a"});
            System.Threading.Thread.Sleep(myUpdateInterval*3);
            
            //Assert
            Assert.NotNull(myReceivedInstrumentPrices);
            Assert.IsTrue(myReceivedInstrumentPrices.ToList().ToDictionary(x => x.Key, x => x.Value).ContainsKey("a"));
            Assert.AreEqual(1, myReceivedInstrumentPrices.ToList().Count); //Should contain only one item
        }


        [Test]
        public void TestStartMonitoring_AddingAdditionalInstrumentForMonitoring()
        {
            //Arrange
            myTestee.Start();
            myTestee.StartMonitoring(new List<string>() { "a" });

            //Act
            myTestee.StartMonitoring(new List<string>() { "c" });

            System.Threading.Thread.Sleep(myUpdateInterval*3);

            //Assert
            Assert.NotNull(myReceivedInstrumentPrices);
            Assert.IsTrue(myReceivedInstrumentPrices.ToList().ToDictionary(x => x.Key, x => x.Value).ContainsKey("a"));
            Assert.IsTrue(myReceivedInstrumentPrices.ToList().ToDictionary(x => x.Key, x => x.Value).ContainsKey("c"));
            Assert.AreEqual(2, myReceivedInstrumentPrices.ToList().Count); //Should contain only one item
        }

        [Test]
        public void TestStopMonitoring_RemovesUpdatesForRemovedInstrument()
        {
            //Arrange
            myTestee.Start();
            myTestee.StartMonitoring(new List<string>() { "a" });
            myTestee.StartMonitoring(new List<string>() { "c" });

            //Act
            myTestee.StopMonitoring(new List<string>() { "c" });
            System.Threading.Thread.Sleep(myUpdateInterval);

            myReceivedInstrumentPrices = null;

            System.Threading.Thread.Sleep(myUpdateInterval*3);

            //Assert
            Assert.NotNull(myReceivedInstrumentPrices);
            Assert.IsTrue(myReceivedInstrumentPrices.ToList().ToDictionary(x => x.Key, x => x.Value).ContainsKey("a"));
            Assert.AreEqual(1, myReceivedInstrumentPrices.ToList().Count); //Should contain only one item
        }


        [Test]
        public void TestStopMonitoring_RemovingLastInstrumentStillUpdates()
        {
            //Arrange
            myTestee.Start();
            myTestee.StartMonitoring(new List<string>() { "a" });
            myTestee.StartMonitoring(new List<string>() { "c" });
            myTestee.StopMonitoring(new List<string>() { "c" });
            //Act
            myTestee.StopMonitoring(new List<string>() { "a" });
            System.Threading.Thread.Sleep(myUpdateInterval*3);

            myReceivedInstrumentPrices = null;
            System.Threading.Thread.Sleep(myUpdateInterval*3);

            //Assert
            Assert.NotNull(myReceivedInstrumentPrices);
            Assert.AreEqual(0, myReceivedInstrumentPrices.ToList().Count);
        }
    }
}