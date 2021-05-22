using System;

namespace dotNetMVCLeagueApp.Utils {
    public static class StringUtils {
        public static string GetGameDuration(TimeSpan duration) =>
            duration.Hours == 0 ? $"{duration.Minutes:0}:{duration.Seconds:00}" : duration.ToString();

        /// <summary>
        /// Zformatuje TimeSpan s dobou trvani hry do retezce
        /// </summary>
        /// <param name="duration">Doba trvani hry</param>
        /// <returns>Naformatovany retezec</returns>
        public static string GetPlayTime(TimeSpan duration) {
            if (duration.Days > 0) {
                return $"{duration.Days} Day(s) Ago";
            }

            if (duration.Hours > 0) {
                return $"{duration.TotalHours:0} Hr(s) Ago";
            }

            return duration.Minutes > 0 ? $"{duration.Minutes} Mins ago" : "Just now";
        }

        public static string FrameIntervalToSeconds(int framesPassed, TimeSpan frameIntervalSeconds) {
            // Api me pro vsechny pozadavky vratilo frame time 1 minutu, takze zde budeme vytvaret string,
            // ktery reprezentuje minuty

            var minutes = (int) frameIntervalSeconds.TotalMinutes * framesPassed;
            return $"{minutes:00}:00";
        }
    }
}