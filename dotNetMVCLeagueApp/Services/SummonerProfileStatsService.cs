using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services.Utils;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro vypocty statistik pro zobrazeni na strance
    /// </summary>
    public class SummonerProfileStatsService {

        private readonly ILogger<SummonerProfileStatsService> logger;

        public SummonerProfileStatsService(ILogger<SummonerProfileStatsService> logger) {
            this.logger = logger;
        }

        public MatchInfoHeaderViewModel GetMatchInfoHeader(SummonerInfoModel summonerInfo,
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
                LargestMultiKill = GameStatsUtils.GetLargestMultiKill(playerStats),
                CsPerMinute =
                    GameStatsUtils.GetCsPerMinute(playerStats.TotalMinionsKilled, matchInfo.GameDuration),
                TotalCs = playerStats.TotalMinionsKilled,
                SummonerSpell1Id = playerInfo.Spell1Id,
                SummonerSpell2Id = playerInfo.Spell2Id,
                VisionScore = playerStats.VisionScore,
                PrimaryRuneId = playerStats.Perk0,
                SecondaryRuneId = playerStats.Perk3,
            };
        }

        public GameListStatsViewModel GetGameListStatsViewModel(List<MatchInfoModel> matchInfoList,
            SummonerInfoModel summonerInfo) {
            var totals = new StatTotals();
            var result = new GameListStatsViewModel();

            foreach (var matchInfo in matchInfoList) {
                CalculateTotals(summonerInfo, matchInfo, result, totals); // vypocet statistik
            }

            GameStatsUtils.CalculateAverages(result, totals);

            return result;
        }

        private void CalculateTotals(SummonerInfoModel summonerInfo, MatchInfoModel matchInfo,
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

            if (GameStatsUtils.IsRemake(matchInfo.GameDuration)) {
                result.Remakes += 1;
                GameStatsUtils.UpdateRoleFrequency(playerInfo, totals.Roles); // Aktualizace role je i pro remake
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
                GameStatsUtils.GetCsPerMinute(playerStats.TotalMinionsKilled, matchInfo.GameDuration));

            // Pridani kill participaci do seznamu
            totals.KillParticipations.Add(
                GameStatsUtils.GetKillParticipation(playerStats, matchInfo, playerTeam.TeamId));

            if (playerInfo.GoldDiffAt10 is not null) {
                totals.GoldDiffsAt10.Add((double) playerInfo.GoldDiffAt10);
            }
        }
    }
}