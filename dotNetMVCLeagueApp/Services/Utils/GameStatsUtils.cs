using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview;

namespace dotNetMVCLeagueApp.Services.Utils {
    public static class GameStatsUtils {
        /// <summary>
        /// Zjisti nejvetsi multi kill, pokud neni vrati null
        /// </summary>
        /// <param name="playerStats">Statistiky hrace</param>
        /// <returns></returns>
        public static string GetLargestMultiKill(PlayerStatsModel playerStats) {
            if (playerStats.PentaKills > 0) {
                return ServerConstants.PentaKill;
            }

            if (playerStats.QuadraKills > 0) {
                return ServerConstants.QuadraKill;
            }

            if (playerStats.TripleKills > 0) {
                return ServerConstants.TripleKill;
            }

            return playerStats.DoubleKills > 0 ? ServerConstants.DoubleKill : null; // Pokud zadny streak tak null
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
            var totalKills = matchModel.PlayerList.Where(player => player.TeamId == teamId)
                .Sum(player => player.PlayerStats.Kills); // celkovy pocet zabiti

            // Pokud je total 0 tak vratime 1.0, jinak vratime kills + assists / total kills
            return totalKills == 0 ? 1.0 : (double) (playerStats.Kills + playerStats.Assists) / totalKills;
        }

        public static double GetKillParticipation(PlayerStatsModel player, IEnumerable<PlayerStatsModel> team) {
            var playerKillsAssists = player.Kills + player.Assists;
            var totalKills = team.Sum(playerStats => playerStats.Kills);

            // Pokud je celkove 0 tak vratime 1.0, jinak vratime (pocet zabiti a asistenci) / celkovym poctem
            return totalKills == 0 ? 1.0 : (double) playerKillsAssists / totalKills;
        }

        public static double GetKillParticipationPercentage(PlayerStatsModel playerStatsModel, MatchModel matchModel,
            int teamId)
            => GetKillParticipation(playerStatsModel, matchModel, teamId) * 100;

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
                ServerConstants.RoleJg when lane == ServerConstants.LaneJg => ServerConstants.Jg,
                ServerConstants.RoleAdc when lane == ServerConstants.LaneBot => ServerConstants.Adc,
                ServerConstants.RoleMid when lane == ServerConstants.LaneMid => ServerConstants.Mid,
                ServerConstants.RoleTop when lane == ServerConstants.LaneTop => ServerConstants.Top,
                _ => ServerConstants.Sup
            };

        /// <summary>
        /// Zjisti, zda-li se jedna o remake - pokud je hra do 4 minut (vcetne)
        /// </summary>
        /// <param name="gameDuration">Doba trvani hry v sekundach - z Riot Api</param>
        /// <returns></returns>
        public static bool IsRemake(long gameDuration) =>
            TimeSpan.FromSeconds(gameDuration) <= ServerConstants.GameDurationForRemake;

        public static double GetKda(int kills, int deaths, int assists) =>
            // Pokud je deaths 0 vratime, jako kdyby bylo deaths 1 tzn (kills + assists) / 1
            // Jinak klasicky (kills + assists) / deaths
            deaths is 0 ? kills + assists : ((double) kills + assists) / deaths;

        public static double GetGoldShare(PlayerStatsModel playerStats, List<PlayerStatsModel> playerTeamStats) {
            var totalGold = playerTeamStats.Sum(player => player.GoldEarned);
            return totalGold == 0.0 ? 1.0 : (double) playerStats.GoldEarned / totalGold;
        }

        public static double GetDamageShare(PlayerStatsModel playerStats,
            IEnumerable<PlayerStatsModel> playerTeamStats) {
            var totalDamageDealt = playerTeamStats.Sum(player => player.TotalDamageDealtToChampions);
            return totalDamageDealt == 0.0 ? 1.0 : (double) playerStats.TotalDamageDealtToChampions / totalDamageDealt;
        }

        public static double GetWinrate(int wins, int losses) =>
            losses == 0 ? 1.0 : (double) wins / (wins + losses);

        public static double GetWinratePercentage(int wins, int losses) => GetWinrate(wins, losses) * 100;

        public static double GetVisionShare(PlayerStatsModel playerStats, List<PlayerStatsModel> playerTeamStats) {
            var totalVision = playerTeamStats.Sum(player => player.VisionScore);
            return totalVision == 0.0 ? 1.0 : (double) playerStats.VisionScore / totalVision;
        }

        public static double GetKillParticipationFromTeamInfoDto(PlayerModel player, TeamDto team) =>
            team.TotalKills == 0
                ? 0.0
                : (double) (player.PlayerStats.Kills + player.PlayerStats.Assists) / team.TotalKills;
    }
}