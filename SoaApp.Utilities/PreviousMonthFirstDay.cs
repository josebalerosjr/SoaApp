using System;
using System.Collections.Generic;
using System.Text;

namespace SoaApp.Utilities
{
    public class PreviousMonthFirstDay
    {
        public static DateTime GetPreviousMonthFirstDay(DateTime AsOfDate)
        {
            return FirstDateOfCurrentMonth.GetFirstDayOfCurrentMonth(AsOfDate).AddMonths(-1); 
        }
    }
}
