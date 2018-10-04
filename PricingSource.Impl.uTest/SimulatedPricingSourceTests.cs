using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using NUnit.Framework;

using PricingSource.Contract;

namespace PricingSource.Impl.uTest
{
    [TestFixture]
    public class SimulatedPricingSourceTests
    {
        private IPricingSource myTestee;
        private IList<string> mySupportedInstruments;

        [SetUp]
        public void TestSetup()
        {
            mySupportedInstruments = new List<string>() { "a", "b", "c", "d" };
            myTestee = new SimulatedPricingSource(mySupportedInstruments);
        }

        [TearDown]
        public void TestTeardown()
        {
            myTestee = null;
        }
        [Test]
        public void TestSupportedInstruments()
        {
            //Assert
            Assert.IsTrue(myTestee.SupportedInstruments.SequenceEqual(mySupportedInstruments));
        }

        [TestCase("a")]
        [TestCase("b")]
        [TestCase("c")]
        [TestCase("d")]
        public void TestGetPrice_ReturnsValueBetweenZeroAndHundred(string instrumentName)
        {
            //Act
            var instrumentPrice = myTestee.GetPrice(instrumentName);

            //Assert
            Assert.IsTrue(instrumentPrice > 0 );
            Assert.IsTrue(instrumentPrice < 100);
        }

        
        [Test]
        public void TestGetPrice_ThrowsInvalidDataExceptionForUnsupportedInstrument()
        {
            //Assert            
            Assert.Throws<InvalidDataException>(() => myTestee.GetPrice("NotSupportedInstrument"));
        }

        [Test]
        public void TestGetPrice_SubsequentGetPriceCallReturnsDifferentPrice()
        {
            //Act
            var price1 = myTestee.GetPrice("a");
            var price2 = myTestee.GetPrice("a");

            //Assert            
            Assert.AreNotEqual(price1, price2);
        }
    }
}