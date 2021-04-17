using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile;

namespace dotNetMVCLeagueApp.Services.Utils {
    public static class GameStatsUtils {
        
        /// <summary>
        /// Zjisti nejvetsi killstreak
        /// </summary>
        /// <param name="playerStats"></param>
        /// <returns></returns>
        public static string GetLargestMultiKill(PlayerStatsModel playerStats) {
            if (playerStats.PentaKills > 0) {
                return GameConstants.PentaKill;
            }

            if (playerStats.QuadraKills > 0) {
                return GameConstants.QuadraKill;
            }

            if (playerStats.TripleKills > 0) {
                return GameConstants.TripleKill;
            }

            return playerStats.DoubleKills > 0 ? GameConstants.DoubleKill : null; // Pokud zadny streak tak null
        }

        /// <summary>
        /// Jednoducha funkce pro vypocet CS za minutu z game duration a celkoveho cs
        /// </summary>
        /// <param name="totalCs">Celkovy pocet CS</param>
        /// <param name="gameDuration">Doba trvani v sekundach</param>
        /// <returns></returns>
        public static double GetCsPerMinute(long totalCs, long gameDuration) =>
            totalCs / TimeSpan.FromSeconds(gameDuration).TotalMinutes;

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
        /// <param name="playerInfo">Info o hraci</param>
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

        /// <summary>
        /// Zjisti, zda-li se jedna o remake - pokud je hra do 4 minut (vcetne)
        /// </summary>
        /// <param name="matchInfoGameDuration">Doba trvani hry v s - z Riot Api</param>
        /// <returns></returns>
        public static bool IsRemake(long matchInfoGameDuration) =>
            TimeSpan.FromSeconds(matchInfoGameDuration) <= GameConstants.GameDurationForRemake;

        /// <summary>
        /// Vypocte prumery pro dane hodnoty
        /// </summary>
        /// <param name="gameListStats">ViewModel objekt</param>
        /// <param name="totals">Objekt s celkovym poctem pro dany seznam her</param>
        public static void CalculateAverages(GameListStatsViewModel gameListStats, StatTotals totals) {
            var realGamesPlayed = gameListStats.GamesWon + gameListStats.GamesLost; // Nepocitame remake hry
            if (realGamesPlayed == 0) { // Pokud se nehraly zadne hry vratime se
                return;
            }

            gameListStats.AverageKills = totals.TotalKills / realGamesPlayed;
            gameListStats.AverageDeaths = totals.TotalDeaths / realGamesPlayed;
            gameListStats.AverageAssists = totals.TotalAssists / realGamesPlayed;
            gameListStats.AverageKda = ((double) totals.TotalAssists + totals.TotalKills) / totals.TotalDeaths;
            gameListStats.AverageKillParticipation = totals.KillParticipations.Average();
            gameListStats.AverageGoldDiffAt10 = totals.GoldDiffsAt10.Average();
            gameListStats.AverageCsPerMinute = totals.CsPerMinuteList.Average();

            var mostPlayedRoles = GetTwoMostPlayedRoles(totals.Roles);
            gameListStats.MostPlayedRole = mostPlayedRoles.Item1;
            gameListStats.SecondMostPlayedRole = mostPlayedRoles.Item2;

        }

        /// <summary>
        /// Ziska dve nejhranejsi role (pokud je alespon 1 nebo vice her)
        /// </summary>
        /// <param name="roles">slovnik s rolemi z StatsTotal objektu</param>
        /// <returns></returns>
        public static (string, string) GetTwoMostPlayedRoles(Dictionary<string, int> roles) {
            var rolesFrequenciesList = roles.ToList();
            // Seradime podle frekvenci a vybereme prvni dva
            rolesFrequenciesList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            return (rolesFrequenciesList[0].Key, rolesFrequenciesList[1].Key);
        }
    }
}