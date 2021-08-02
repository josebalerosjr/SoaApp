using System;

namespace SoaApp.Utilities
{
    public static class DateMangement
    {
        public static DateTime GetFirstDayOfCurrentMonth(DateTime AsOfDate)
        {
            return new DateTime(AsOfDate.Year, AsOfDate.Month, 1);
        }

        public static DateTime GetPreviousMonthFirstDay(DateTime AsOfDate)
        {
            return GetFirstDayOfCurrentMonth(AsOfDate).AddMonths(-1);
        }

        public static DateTime GetPreviousMonthLastDay(DateTime AsOfDate)
        {
            return GetFirstDayOfCurrentMonth(AsOfDate).AddDays(-1);
        }
    }
}