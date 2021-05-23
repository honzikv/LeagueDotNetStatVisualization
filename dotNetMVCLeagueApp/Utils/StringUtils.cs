using System;

namespace dotNetMVCLeagueApp.Utils {
    public static class StringUtils {
        
        /// <summary>
        /// Vytvori string z TimeSpan objektu, ktery reprezentuje dobu trvani hry
        /// Pr. 120 minut se prevede na "2:00:00". 54 minut se prevede na "54:00"
        /// </summary>
        /// <param name="duration">Doba trvani hry</param>
        /// <returns>formatovany string</returns>
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
            var minutes = (int) frameIntervalSeconds.TotalMinutes * framesPassed;
            return $"{minutes:00}:00";
        }

        /// <summary>
        /// Zkrati retezec, pokud prekroci pozadovany pocet znaku a prida suffix
        /// </summary>
        /// <param name="text">text, ktery kontrolujeme</param>
        /// <param name="chars">maximalni pocet znaku</param>
        /// <param name="suffix">string, ktery se prida na konec (...)</param>
        /// <returns>zkraceny string, pokud bylo potreba</returns>
        public static string TruncateIfNecessary(string text, int chars, string suffix = "...") {
            if (text is null) {
                return "";
            }

            return text.Length > chars
                ? text[..chars] + suffix
                : text;
        }
    }
}