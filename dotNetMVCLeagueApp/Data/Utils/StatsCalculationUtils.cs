using System;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Data.Utils {
    public static class StatsCalculationUtils {
        public static string GetLargestMultiKill(PlayerStatsModel playerStats) {
            if (playerStats.PentaKills > 0) {
                return MultiKills.PentaKill;
            }

            if (playerStats.QuadraKills > 0) {
                return MultiKills.QuadraKill;
            }

            if (playerStats.TripleKills > 0) {
                return MultiKills.TripleKill;
            }

            return playerStats.DoubleKills > 0 ? MultiKills.DoubleKill : null; // Pokud zadny streak tak null
        }

        /// <summary>
        /// Jednoducha funkce pro vypocet CS za minutu z game duration a celkoveho cs
        /// </summary>
        /// <param name="totalCs"></param>
        /// <param name="gameDuration"></param>
        /// <returns></returns>
        public static double GetCsPerMinute(long totalCs, long gameDuration) =>
            totalCs / TimeSpan.FromTicks(gameDuration).TotalMinutes;
    }
}