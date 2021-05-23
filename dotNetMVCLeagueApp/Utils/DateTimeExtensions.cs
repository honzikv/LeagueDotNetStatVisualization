using System;

namespace dotNetMVCLeagueApp.Utils {
    
    /// <summary>
    /// Trida pro extension metody pro DateTime, aby byl kod lepe citelny
    /// </summary>
    public static class DateTimeExtensions {

        /// <summary>
        /// Odecte tyden od daneho DateTime objektu
        /// </summary>
        /// <param name="dateTime">DateTime od ktereho odecitame tyden</param>
        /// <returns>Novy DateTime objekt, ktery reprezentuje datum - 7 dni</returns>
        public static DateTime SubtractWeek(this DateTime dateTime) => 
            dateTime.Subtract(TimeSpan.FromDays(7));
        
    }
}