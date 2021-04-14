using System;
using System.Collections.Generic;
using System.Text;

namespace SoaApp.Utilities
{
    public class FirstDateOfCurrentMonth
    {
        public static DateTime GetFirstDayOfCurrentMonth(DateTime AsOfDate)
        { 
            return new DateTime(AsOfDate.Year, AsOfDate.Month, 1);
        }
    }
}
