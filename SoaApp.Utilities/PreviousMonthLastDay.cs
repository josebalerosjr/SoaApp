using System;
using System.Collections.Generic;
using System.Text;

namespace SoaApp.Utilities
{
    public class PreviousMonthLastDay
    {
        public static DateTime GetPreviousMonthLastDay(DateTime AsOfDate)
        {
            return FirstDateOfCurrentMonth.GetFirstDayOfCurrentMonth(AsOfDate).AddDays(-1);
        }
    }
}
