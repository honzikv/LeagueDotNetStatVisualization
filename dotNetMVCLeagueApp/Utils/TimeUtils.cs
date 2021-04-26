using System;

namespace dotNetMVCLeagueApp.Utils {
    public class TimeUtils {
        /// <summary>
        /// Slouzi pro prevod timestapu ziskaneho z RiotApi
        /// </summary>
        /// <param name="timestamp">casove razitko</param>
        /// <returns></returns>
        public static DateTime ConvertFromMillisToDateTime(double timestamp) =>
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(timestamp)
                .ToLocalTime();

        /// <summary>
        /// Utilitni funkce pro prevod frameTime udaje z api/db na TimeSpan
        /// </summary>
        /// <param name="frameTime">cislo framu * frameDuration</param>
        /// <returns></returns>
        public static TimeSpan ConvertFrameTimeToTimeSpan(long frameTime) => TimeSpan.FromMilliseconds(frameTime);
    }
}