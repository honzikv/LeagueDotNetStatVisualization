using System;

namespace dotNetMVCLeagueApp.Utils {
    public static class StringUtils {

        public static string GetGameDuration(TimeSpan duration) => 
            duration.Hours == 0 ? $"{duration.Minutes:00}:{duration.Seconds:00}" : duration.ToString();

        public static string GetPlayTime(TimeSpan duration) {
            if (duration.Days > 0) {
                return $"{duration.Days} Day(s) Ago";
            }
            if (duration.Hours > 0) {
                return $"{duration.TotalHours:00} Hr(s) Ago";
            }

            return duration.Minutes > 0 ? $"{duration.Minutes} Mins ago" : "Just now";
        }
    }
}