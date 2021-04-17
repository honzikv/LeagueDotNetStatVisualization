using System;

namespace dotNetMVCLeagueApp.Utils {
    public class DateTimeUtils {
        /// <summary>
        /// Slouzi pro prevod timestapu ziskaneho z RiotApi
        /// </summary>
        /// <param name="timestamp">casove razitko</param>
        /// <returns></returns>
        public static DateTime ConvertFromUnixTimestamp(double timestamp) =>
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddMilliseconds(timestamp);
    }
}