using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;

using NUnit.Framework;

using PricingSource.Contract;


namespace PricingSource.Impl.uTest
{
    [TestFixture]
    public class CompositePricingSourceTest
    {
        private IPricingSource myTestee;
        private IList<string> mySupportedInstruments;
        
        private readonly Mock<IPricingSource> myPricingSource1 = new Mock<IPricingSource>();
        private readonly Mock<IPricingSource> myPricingSource2 = new Mock<IPricingSource>();

        [SetUp]
        public void TestSetup()
        {
            mySupportedInstruments = new List<string>() { "a", "b", "c", "d" };
            myTestee = new CompositePricingSource(new List<IPricingSource>(){myPricingSource1.Object, myPricingSource2.Object});

            myPricingSource1.SetupGet(m => m.SupportedInstruments).Returns(new List<string>() { "a", "b" });
            myPricingSource1.Setup(m => m.GetPrice("a")).Returns(10.25);
            myPricingSource1.Setup(m => m.GetPrice("b")).Returns(15.25);

            myPricingSource2.SetupGet(m => m.SupportedInstruments).Returns(new List<string>() { "c", "d" });
            myPricingSource2.Setup(m => m.GetPrice("c")).Returns(80.25);
            myPricingSource2.Setup(m => m.GetPrice("d")).Returns(45.25);

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
        public void TestGetPrice_ReturnsValue(string instrumentName)
        {
            //Act
            var instrumentPrice = myTestee.GetPrice(instrumentName);

            //Assert
            Assert.IsTrue(instrumentPrice > 0);
            Assert.IsTrue(instrumentPrice < 100);
        }


        [Test]
        public void TestGetPrice_ThrowsInvalidDataExceptionForUnsupportedInstrument()
        {
            //Assert            
            Assert.Throws<InvalidDataException>(() => myTestee.GetPrice("NotSupportedInstrument"));
        }
    }
}