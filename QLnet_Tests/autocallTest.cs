
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
           
            Console.ReadLine();
        }
    }
}
