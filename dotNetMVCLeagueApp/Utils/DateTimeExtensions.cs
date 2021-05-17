using System;

namespace dotNetMVCLeagueApp.Utils {
    
    /// <summary>
    /// Trida pro extension metody pro DateTime, aby byl kod lepe citelny
    /// </summary>
    public static class DateTimeExtensions {

        public static DateTime SubtractWeek(this DateTime dateTime) => 
            dateTime.Subtract(TimeSpan.FromDays(7));
        
    }
}