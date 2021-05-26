using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Util
{
    public static int getDiffDay(string day1, string day2, int offsetDay = 0)
    {
        DateTime dt1 = System.Convert.ToDateTime(day1);
        DateTime dt2 = System.Convert.ToDateTime(day2);
        dt2 = dt2.AddDays(offsetDay);
        TimeSpan span = dt2.Subtract(dt1);
        int dayDiff = span.Days;
        return dayDiff;
    }

    public static string GetWeeks(DateTime time)
    {
        string week = null;
        switch (time.DayOfWeek)
        {
            case DayOfWeek.Monday:
                week = "星期一";
                break;
            case DayOfWeek.Tuesday:
                week = "星期二";
                break;
            case DayOfWeek.Wednesday:
                week = "星期三";
                break;
            case DayOfWeek.Thursday:
                week = "星期四";
                break;
            case DayOfWeek.Friday:
                week = "星期五";
                break;
            case DayOfWeek.Saturday:
                week = "星期六";
                break;
            default:
                week = "星期天";
                break;
        }
        return week;
    }
}
