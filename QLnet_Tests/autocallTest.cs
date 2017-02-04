
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace AuticallPricer
{
    class AuticallPricer
    {
        static void Main(string[] args)
        {

            // set up dates
            Calendar calendar = new TARGET();
            Date todaysDate = new Date(1, Month.February, 2017);
            Date settlementDate = todaysDate;
            Settings.setEvaluationDate(todaysDate);
            
            //build autocall
            double coupon = 0.05;
            double barrierlvl = 0.6;
            List<Date> fixingdates = new List<Date>(); ;
            for (int i = 1; i <= 4; i++)
                fixingdates.Add(settlementDate + new Period( i, TimeUnit.Years));

            Autocall myautocall = new Autocall(coupon, fixingdates, barrierlvl);

            Console.WriteLine("coupon =  {0:0.000%}" , coupon);
            Console.WriteLine("Barrier =  {0:0.000%}" , barrierlvl);
            Console.Write("\n");

            // Market
            double dividendYield = 0.03;
            double riskFreeRate = 0.01;
            double volatility = 0.15;
            DayCounter dayCounter = new Actual365Fixed();
            double underlying = 100;

            Console.WriteLine("Underlying price = " + underlying);
            Console.WriteLine("Risk-free interest rate = {0:0.000%}", riskFreeRate);
            Console.WriteLine("Dividend yield = {0:0.000%}", dividendYield);
            Console.WriteLine("Volatility = {0:0.000%}", volatility);
            Console.Write("\n");

            Handle<Quote> underlyingH = new Handle<Quote>(new SimpleQuote(underlying));

            // bootstrap the yield/dividend/vol curves
            var flatTermStructure = new Handle<YieldTermStructure>(new FlatForward(settlementDate, riskFreeRate, dayCounter));
            var flatDividendTS = new Handle<YieldTermStructure>(new FlatForward(settlementDate, dividendYield, dayCounter));
            var flatVolTS = new Handle<BlackVolTermStructure>(new BlackConstantVol(settlementDate, calendar, volatility, dayCounter));
            var bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS, flatTermStructure, flatVolTS);

            int timeSteps = 1;
            ulong mcSeed = 42;
            IPricingEngine mcengine1 = new MakeMCautocallEngine<PseudoRandom>(bsmProcess)
                                            .withSteps(timeSteps)
                                            .withAbsoluteTolerance(0.02)
                                            .withSeed(mcSeed)
                                            .value();

            myautocall.setPricingEngine(mcengine1);
            Console.ReadLine();
        }
    }
}
