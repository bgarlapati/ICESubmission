Assumptions:
===========
-Assumed only one pricing source suports each of the instrument.
-Only supported instruments currently are "AAPL", "WMT", "FB", "MSFT", "TSLA", "INTC", "JNJ", "XOM", "MMM", "AXP", "BA", "CAT", "CVX", "CSCO", "KO", "WBA", "AA" 
 ( this can be configurable for each pricing source)

 -Used NUnit and Moq frameworks for unit tests.

Summary:
========
 The solution is divided into three pieces: Pricing Sources, Pricing Engine and APplication
   Pricing Sources: supports some specific instruments and can provide the price for supported instrument.
   Pricing Engine: In periodical intervals, it provides update to the clients for the subscribed instruments.
 
Further enhancements:
====================
The data should be serializable if it needs to be passed across the WCF.
The glueing logic of Pricing sources/Pricing Engine and application can be configured using dependency inversion framework( ex: spring.net)
Integration test ( component test) can be added at the Pricing Engine level, view model level.
Can be optimized to provide info only for price changed instruments.
Tracing/Logging/Error logging enhancements 
