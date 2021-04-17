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

        /// <summary>
        /// Funkce, ktera provede GetMatchInfoHeader() pro seznam se zapasy misto jednoho objektu
        /// </summary>
        /// <param name="summonerInfo"></param>
        /// <param name="matchInfoList"></param>
        /// <returns></returns>
        public List<MatchInfoHeaderViewModel> GetMatchInfoHeaderList(SummonerInfoModel summonerInfo,
            List<MatchInfoModel> matchInfoList) =>
            matchInfoList.Select(matchInfo => GetMatchInfoHeader(summonerInfo, matchInfo)).ToList();

        public MatchInfoHeaderViewModel GetMatchInfoHeader(SummonerInfoModel summonerInfo,
            MatchInfoModel matchInfo) {
            var playerInfo = matchInfo.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summonerInfo.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            // Statistika hrace
            var playerStats = playerInfo.PlayerStatsModel;

            // Mapping do jednoho objektu
            return new MatchInfoHeaderViewModel {
                PlayTime = matchInfo.PlayTime,
                ChampionIconId = playerInfo.ChampionId,
                TeamId = playerInfo.TeamId,
                Kills = playerStats.Kills,
                Deaths = playerStats.Deaths,
                Assists = playerStats.Assists,
                KillParticipation = GameStatsUtils.GetKillParticipation(playerStats, matchInfo, playerInfo.TeamId),
                Kda = ((double) playerStats.Kills + playerStats.Assists) / playerStats.Deaths,
                DamageDealt = playerStats.TotalDamageDealtToChampions,
                Role = GameStatsUtils.GetRole(playerInfo.Role, playerInfo.Lane),
                Win = matchInfo.Teams.FirstOrDefault(
                    team => team.TeamId == playerInfo.TeamId)?.Win == GameConstants.Win,
                QueueType = matchInfo.QueueType,
                Items = new() {
                    playerStats.Item0, playerStats.Item1, playerStats.Item2, playerStats.Item3, playerStats.Item4,
                    playerStats.Item5, playerStats.Item6
                },
                LargestMultiKill = GameStatsUtils.GetLargestMultiKill(playerStats),
                CsPerMinute = GameStatsUtils.GetCsPerMinute(playerStats, matchInfo.GameDuration),
                TotalCs = GameStatsUtils.GetTotalCs(playerStats),
                SummonerSpell1Id = playerInfo.Spell1Id,
                SummonerSpell2Id = playerInfo.Spell2Id,
                VisionScore = playerStats.VisionScore,
                PrimaryRuneId = playerStats.Perk0,
                SecondaryRuneId = playerStats.Perk3,
            };
        }

        /// <summary>
        /// Vypocte statistiky pro dany seznam her pro daneho hrace
        /// </summary>
        /// <param name="matchInfoList">Seznam her, pro ktere se vypoctou statistiky</param>
        /// <param name="summonerInfo">Info o hraci v danych hrach - pro nej se statistiky pocitaji</param>
        /// <returns></returns>
        public GameListStatsViewModel GetGameListStatsViewModel(List<MatchInfoModel> matchInfoList,
            SummonerInfoModel summonerInfo) {
            var totals = new GameListStats();
            var result = new GameListStatsViewModel();

            foreach (var matchInfo in matchInfoList) {
                CalculateStatTotals(summonerInfo, matchInfo, result, totals); // vypocet statistik
            }

            GameStatsUtils.CalculateAverages(result, totals);

            logger.LogDebug($"GameStats calculated, result: {result}");
            return result;
        }

        /// <summary>
        /// Vypocte celkove pocty pro dane metriky a ulozi je do StatsTotals objektu
        /// </summary>
        /// <param name="summonerInfo">Reference na summoner info</param>
        /// <param name="matchInfo">Reference na match info</param>
        /// <param name="gameListStats">Statistiky pro seznam her</param>
        /// <param name="totals">Objekt s celkovymi pocty</param>
        /// <exception cref="ActionNotSuccessfulException">Pokud je hrac null nebo je hracuv team null</exception>
        private void CalculateStatTotals(SummonerInfoModel summonerInfo, MatchInfoModel matchInfo,
            GameListStatsViewModel gameListStats, GameListStats totals) {
            var playerInfo = matchInfo.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summonerInfo.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error player info is null for the given match");
            }

            var playerTeam = matchInfo.Teams.FirstOrDefault(team => team.TeamId == playerInfo.TeamId);
            if (playerTeam is null) {
                throw new ActionNotSuccessfulException("Error player team is null for the given match");
            }

            GameStatsUtils.UpdateRoleFrequency(playerInfo, totals.Roles); // Aktualizace role je i pro remake

            if (GameStatsUtils.IsRemake(matchInfo.GameDuration)) {
                logger.LogDebug("Found a remake game, increasing number of remakes");
                gameListStats.Remakes += 1;
                return;
            }

            // Jinak pricteme win / loss podle stringu
            if (playerTeam.Win == GameConstants.Win) {
                logger.LogDebug("Found win");
                gameListStats.GamesWon += 1;
            }
            else {
                logger.LogDebug("Found loss");
                gameListStats.GamesLost += 1;
            }

            // Aktualizace stat totals - pricteme celkove smrti, zabiti, asistence ...
            GameStatsUtils.UpdateStatTotals(totals, matchInfo, playerInfo, playerTeam);
        }
    }
}