using System;
using System.Globalization;

namespace Utilities.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime value)
        {
            try
            {
                PersianCalendar pc = new PersianCalendar();
                return pc.GetYear(value) + "/" +
                       pc.GetMonth(value).ToString("00") + "/" +
                       pc.GetDayOfMonth(value).ToString("00");
            }
            catch (Exception)
            {

                return "فرمت تاریخ اشتباه است";
            }
            
        }
    }
}
