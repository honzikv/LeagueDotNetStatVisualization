using System;
using System.Collections.Generic;
using AutoMapper.Internal;
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
        /// <param name="totalCs">Celkovy pocet CS</param>
        /// <param name="gameDuration">Doba trvani v sekundach</param>
        /// <returns></returns>
        public static double GetCsPerMinute(long totalCs, long gameDuration) =>
            totalCs / TimeSpan.FromSeconds(gameDuration).TotalMinutes;

        public static TimeSpan ConvertGameDurationToTimespan(long gameDuration) => TimeSpan.FromSeconds(gameDuration);

        /// <summary>
        /// Ziska podil na celkovemu poctu zabiti - kill participation
        /// </summary>
        /// <param name="playerStats"></param>
        /// <param name="matchInfoModel"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public static double GetKillParticipation(PlayerStatsModel playerStats, MatchInfoModel matchInfoModel,
            int teamId) {
            var total = 0; // celkovy pocet zabiti
            foreach (var player in matchInfoModel.PlayerInfoList) {
                if (player.TeamId == teamId) {
                    total += player.PlayerStatsModel.Kills;
                }
            }

            // Pokud je total 0 tak vratime 1.0, jinak vratime kills / total kills
            return total == 0 ? 1.0 : (double) playerStats.Kills / total;
        }

        /// <summary>
        /// Aktualizuje slovnik s frekvencemi roli - pricte 1 pro spravnou roli
        /// </summary>
        /// <param name="playerStats"></param>
        /// <param name="roles"></param>
        public static void UpdateRoleFrequency(PlayerInfoModel playerInfo, Dictionary<string, int> roles) {
            var role = playerInfo.Role;
            var lane = playerInfo.Lane;

            if (role == GameConstants.ROLE_JG && lane == GameConstants.LANE_JG) {
                roles[GameConstants.JG] += 1;
            }
            else if (role == GameConstants.RoleAdc && lane == GameConstants.LANE_BOT) {
                roles[GameConstants.RoleAdc] += 1;
            }
            else if (role == GameConstants.ROLE_MID && lane == GameConstants.LANE_MID) {
                roles[GameConstants.MID] += 1;
            }
            else if (role == GameConstants.ROLE_TOP && lane == GameConstants.LANE_TOP) {
                roles[GameConstants.TOP] += 1;
            }
            else {
                roles[GameConstants.SUP] += 1; // jedina linka, ktera zbyva
            }
        }
    }
}