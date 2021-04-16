using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Utils;
using dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile;
using dotNetMVCLeagueApp.Exceptions;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro vypocty statistik pro zobrazeni na strance
    /// </summary>
    public class SummonerProfileStatsService {

        public static MatchInfoHeaderViewModel GetMatchInfoHeader(SummonerInfoModel summonerInfo,
            MatchInfoModel matchInfo) {
            var playerInfo = matchInfo.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summonerInfo.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            var playerStats = playerInfo.PlayerStatsModel;

            // Mapping do jednoho objektu
            return new MatchInfoHeaderViewModel {
                PlayTime = matchInfo.PlayTime,
                ChampionIconId = playerInfo.ChampionId,
                TeamId = playerInfo.TeamId,
                Win = matchInfo.Teams.FirstOrDefault(team => team.Id == playerInfo.TeamId)?.Win,
                QueueType = matchInfo.GameType,
                Items = new() {
                    playerStats.Item0, playerStats.Item1, playerStats.Item2, playerStats.Item3, playerStats.Item4,
                    playerStats.Item5, playerStats.Item6
                },
                LargestMultiKill = StatsCalculationUtils.GetLargestMultiKill(playerStats),
                CsPerMinute =
                    StatsCalculationUtils.GetCsPerMinute(playerStats.TotalMinionsKilled, matchInfo.GameDuration),
                TotalCs = playerStats.TotalMinionsKilled,
                SummonerSpell1Id = playerInfo.Spell1Id,
                SummonerSpell2Id = playerInfo.Spell2Id,
                VisionScore = playerStats.VisionScore,
                PrimaryRuneId = playerStats.Perk0,
                SecondaryRuneId = playerStats.Perk3,
            };
        }

        /// <summary>
        /// Trida pro ulozeni celkovych poctu pri GetGameListStatsViewModel
        /// </summary>
        private class StatTotals {
            public int TotalKills { get; set; }
            public int TotalDeaths { get; set; }
            public int TotalAssists { get; set; }
            public List<double> KillParticipations { get; } = new();

            public Dictionary<string, int> Roles { get; set; } = new() {
                // Frekvenci roli muzeme sledovat napr. pomoci dictionary (nebo polem)
                {GameConstants.TOP, 0},
                {GameConstants.MID, 0},
                {GameConstants.ADC, 0},
                {GameConstants.SUP, 0},
                {GameConstants.JG, 0}
            };

            public List<double> GoldDiffsAt10 { get; } = new();

            public List<double> CsPerMinuteList { get; } = new();
        };

        public static GameListStatsViewModel GetGameListStatsViewModel(List<MatchInfoModel> matchInfoList,
            SummonerInfoModel summonerInfo) {
            var totals = new StatTotals();
            var result = new GameListStatsViewModel();

            foreach (var matchInfo in matchInfoList) {
                CalculateTotals(summonerInfo, matchInfo, result, totals); // vypocet statistik
            }

            var realGamesPlayed = result.GamesWon + result.GamesLost; // Nepocitame remake  hry
            if (realGamesPlayed == 0) { // Pokud se nehraly zadne hry vratime rovnou vysledek
                return result;
            }

            result.AverageKills = totals.TotalKills / realGamesPlayed;
            result.AverageDeaths = totals.TotalDeaths / realGamesPlayed;
            result.AverageAssists = totals.TotalAssists / realGamesPlayed;
            result.AverageKda = ((double) totals.TotalAssists + totals.TotalKills) / totals.TotalDeaths;
            result.AverageKillParticipation = totals.KillParticipations.Average();
            result.AverageGoldDiffAt10 = totals.GoldDiffsAt10.Average();
            result.AverageCsPerMinute = totals.CsPerMinuteList.Average();

            var rolesFrequenciesList = totals.Roles.ToList();

            // Seradime podle frekvenci a vybereme prvni dva
            rolesFrequenciesList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            result.MostPlayedRole = rolesFrequenciesList[0].Key;
            result.SecondMostPlayedRole = rolesFrequenciesList[1].Key;

            return result;
        }

        private static void CalculateTotals(SummonerInfoModel summonerInfo, MatchInfoModel matchInfo,
            GameListStatsViewModel result, StatTotals totals) {
            var playerInfo = matchInfo.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summonerInfo.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            var playerTeam = matchInfo.Teams.FirstOrDefault(team => team.TeamId == playerInfo.TeamId);
            if (playerTeam is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            var playerStats = playerInfo.PlayerStatsModel;

            if (StatsCalculationUtils.ConvertGameDurationToTimespan(matchInfo.GameDuration) <=
                GameConstants.GameDurationForRemake) {
                result.Remakes += 1;

                // Aktualizace role je i pro remake
                StatsCalculationUtils.UpdateRoleFrequency(playerInfo, totals.Roles);
                return;
            }

            // Jinak pricteme win / loss podle stringu
            if (playerTeam.Win == GameConstants.Win) {
                result.GamesWon += 1;
            }
            else {
                result.GamesLost += 1;
            }

            // Celkovy pocet pro zabiti, smrti a asistence
            totals.TotalKills += playerStats.Kills;
            totals.TotalAssists += playerStats.Assists;
            totals.TotalDeaths += playerStats.Deaths;
            
            totals.CsPerMinuteList.Add(
                StatsCalculationUtils.GetCsPerMinute(playerStats.TotalMinionsKilled, matchInfo.GameDuration));

            // Pridani kill participaci do seznamu
            totals.KillParticipations.Add(
                StatsCalculationUtils.GetKillParticipation(playerStats, matchInfo, playerTeam.TeamId));

            if (playerInfo.GoldDiffAt10 is not null) {
                totals.GoldDiffsAt10.Add((double) playerInfo.GoldDiffAt10);
            }
        }
    }
}