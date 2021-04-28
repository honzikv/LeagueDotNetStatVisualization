﻿using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;

namespace dotNetMVCLeagueApp.Services.Utils {
    public static class GameStatsUtils {
        /// <summary>
        /// Zjisti nejvetsi multi kill, pokud neni vrati null
        /// </summary>
        /// <param name="playerStats">Statistiky hrace</param>
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
        /// <param name="playerStats">Reference na statistiky hrace</param>
        /// <param name="gameDuration">Doba trvani v sekundach</param>
        /// <returns></returns>
        public static double GetCsPerMinute(PlayerStatsModel playerStats, long gameDuration) =>
            GetTotalCs(playerStats) / TimeSpan.FromSeconds(gameDuration).TotalMinutes;

        public static int GetTotalCs(PlayerStatsModel playerStats) {
            return playerStats.NeutralMinionsKilled + playerStats.TotalMinionsKilled;
        }

        /// <summary>
        /// Ziska podil na celkovemu poctu zabiti - kill participation
        /// </summary>
        /// <param name="playerStats"></param>
        /// <param name="matchModel"></param>
        /// <param name="teamId"></param>
        /// <returns>Kill participaci pro daneho hrace</returns>
        public static double GetKillParticipation(PlayerStatsModel playerStats, MatchModel matchModel,
            int teamId) {
            var totalKills = 0; // celkovy pocet zabiti
            foreach (var player in matchModel.PlayerInfoList) {
                if (player.TeamId == teamId) {
                    totalKills += player.PlayerStatsModel.Kills;
                }
            }

            // Pokud je total 0 tak vratime 1.0, jinak vratime kills + assists / total kills
            return totalKills == 0 ? 1.0 : (double) (playerStats.Kills + playerStats.Assists) / totalKills;
        }

        /// <summary>
        /// Aktualizuje slovnik s frekvencemi roli - pricte 1 pro spravnou roli
        /// </summary>
        /// <param name="player">Info o hraci</param>
        /// <param name="roles"></param>
        public static void UpdateRoleFrequency(PlayerModel player, Dictionary<string, int> roles) {
            var role = player.Role;
            var lane = player.Lane;

            roles[GetRole(role, lane)] += 1;
        }

        /// <summary>
        /// Ziska roli podle parametru role a lane (z nejakeho duvodu ma Riot Api dva retezce, ze kterych je potreba
        /// vyparsovat)
        /// </summary>
        /// <param name="role">role z riot api</param>
        /// <param name="lane">linka (lane) z riot api</param>
        /// <returns></returns>
        public static string GetRole(string role, string lane) =>
            role switch {
                GameConstants.RoleJg when lane == GameConstants.LaneJg => GameConstants.Jg,
                GameConstants.RoleAdc when lane == GameConstants.LaneBot => GameConstants.Adc,
                GameConstants.RoleMid when lane == GameConstants.LaneMid => GameConstants.Mid,
                GameConstants.RoleTop when lane == GameConstants.LaneTop => GameConstants.Top,
                _ => GameConstants.Sup
            };

        /// <summary>
        /// Zjisti, zda-li se jedna o remake - pokud je hra do 4 minut (vcetne)
        /// </summary>
        /// <param name="matchInfoGameDuration">Doba trvani hry v sekundach - z Riot Api</param>
        /// <returns></returns>
        public static bool IsRemake(long matchInfoGameDuration) =>
            TimeSpan.FromSeconds(matchInfoGameDuration) <= GameConstants.GameDurationForRemake;

        public static void UpdateStatTotals(GameListStats stats, MatchModel match, PlayerModel player,
            TeamStatsModel playerTeam) {
            var playerStats = player.PlayerStatsModel;
            // Celkovy pocet pro zabiti, smrti a asistence
            stats.Kills.Add(playerStats.Kills);
            stats.Assists.Add(playerStats.Assists);
            stats.Deaths.Add(playerStats.Deaths);
            stats.Gold.Add(playerStats.GoldEarned);

            // Pridani CS (creep score) za minutu do seznamu
            stats.CsPerMinuteList.Add(GetCsPerMinute(playerStats, match.GameDuration));

            // Pridani kill participaci do seznamu
            stats.KillParticipations.Add(GetKillParticipation(playerStats, match, playerTeam.TeamId));

            if (player.GoldDiffAt10 is not null) {
                stats.GoldDiffsAt10.Add((double) player.GoldDiffAt10);
            }
        }

        public static double CalculateKda(int kills, int deaths, int assists) =>
            // Pokud je deaths 0 vratime, jako kdyby bylo deaths 1 tzn (kills + assists) / 1
            // Jinak klasicky (kills + assists) / deaths
            deaths is 0 ? kills + assists : ((double) kills + assists) / deaths;

        /// <summary>
        /// Vypocte prumery pro dane hodnoty v GameListStatsViewModel
        /// </summary>
        /// <param name="statsDto">ViewModel objekt</param>
        /// <param name="totals">Objekt s celkovym poctem pro dany seznam her</param>
        public static void CalculateAverages(MatchListStatsDto statsDto, GameListStats totals) {
            var realGamesPlayed = statsDto.GamesWon + statsDto.GamesLost; // Nepocitame remake hry
            if (realGamesPlayed == 0) { // Pokud se nehraly zadne hry vratime se
                return;
            }

            statsDto.AverageKills = totals.Kills.Average();
            statsDto.AverageDeaths = totals.Deaths.Average();
            statsDto.AverageAssists = totals.Assists.Average();

            statsDto.AverageKda = CalculateKda(totals.Kills.Sum(), totals.Deaths.Sum(), totals.Assists.Sum());

            statsDto.AverageKillParticipation = totals.KillParticipations.Average();
            statsDto.AverageGoldDiffAt10 = totals.GoldDiffsAt10.Average();
            statsDto.AverageCsPerMinute = totals.CsPerMinuteList.Average();

            var mostPlayedRoles = GetTwoMostPlayedRoles(totals.Roles);
            statsDto.MostPlayedRole = mostPlayedRoles.Item1;
            statsDto.SecondMostPlayedRole = mostPlayedRoles.Item2;
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