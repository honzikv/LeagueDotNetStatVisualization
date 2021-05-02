using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Repositories.AssetResolver;
using dotNetMVCLeagueApp.Services.Utils;
using dotNetMVCLeagueApp.Utils;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services.Summoner {
    /// <summary>
    /// Sluzba pro vypocty statistik pro zobrazeni na strance
    /// </summary>
    public class SummonerProfileStatsService {
        private readonly ILogger<SummonerProfileStatsService> logger;
        private readonly AssetRepository assetRepository;

        public SummonerProfileStatsService(ILogger<SummonerProfileStatsService> logger,
            AssetRepository assetRepository) {
            this.logger = logger;
            this.assetRepository = assetRepository;
        }

        /// <summary>
        /// Funkce, ktera provede GetMatchInfoHeader() pro seznam se zapasy misto jednoho objektu
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="matchInfoList"></param>
        /// <returns></returns>
        public List<MatchHeaderDto> GetMatchInfoHeaderList(SummonerModel summoner,
            IEnumerable<MatchModel> matchInfoList) =>
            matchInfoList.Select(matchInfo => GetMatchInfoHeader(summoner, matchInfo)).ToList();

        /// <summary>
        /// Vytvori match info header pro jednu hru
        /// </summary>
        /// <param name="summoner">hrac, pro ktereho header vytvarime</param>
        /// <param name="match">reference na zapas, pro ktery header vytvarime</param>
        /// <returns></returns>
        /// <exception cref="ActionNotSuccessfulException">Pokud neexistuje info o hraci</exception>
        public MatchHeaderDto GetMatchInfoHeader(SummonerModel summoner,
            MatchModel match) {
            var playerInfo = match.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summoner.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            // Statistika hrace
            var playerStats = playerInfo.PlayerStatsModel;

            // Nyni objekt muzeme rovnou inicializovat pomoci {}
            return new MatchHeaderDto {
                Duration = TimeSpan.FromSeconds(match.GameDuration),
                QueueType = match.QueueType,
                TeamId = playerInfo.TeamId,
                Kills = playerStats.Kills,
                Deaths = playerStats.Deaths,
                Assists = playerStats.Assists,
                Gold = playerStats.GoldEarned,
                DamageDealt = playerStats.TotalDamageDealtToChampions,
                VisionScore = playerStats.VisionScore,
                LargestMultiKill = GameStatsUtils.GetLargestMultiKill(playerStats),
                CsPerMinute = GameStatsUtils.GetCsPerMinute(playerStats, match.GameDuration),
                TotalCs = GameStatsUtils.GetTotalCs(playerStats),
                KillParticipation =
                    GameStatsUtils.GetKillParticipationPercentage(playerStats, match, playerInfo.TeamId),
                Kda = GameStatsUtils.CalculateKda(playerStats.Kills, playerStats.Deaths, playerStats.Assists),
                PlayTime = TimeUtils.GetTimeFromToday(match.PlayTime),
                Win = match.Teams.FirstOrDefault(
                    team => team.TeamId == playerInfo.TeamId)?.Win == GameConstants.Win,
                Role = GameStatsUtils.GetRole(playerInfo.Role, playerInfo.Lane),
                ChampionAsset = assetRepository.GetChampionAsset(playerInfo.ChampionId),
                Items = new() {
                    assetRepository.GetItemAsset(playerStats.Item0),
                    assetRepository.GetItemAsset(playerStats.Item1),
                    assetRepository.GetItemAsset(playerStats.Item2),
                    assetRepository.GetItemAsset(playerStats.Item3),
                    assetRepository.GetItemAsset(playerStats.Item4),
                    assetRepository.GetItemAsset(playerStats.Item5),
                    assetRepository.GetItemAsset(playerStats.Item6),
                },
                SummonerSpell1 = assetRepository.GetSummonerSpellAsset(playerInfo.Spell1Id),
                SummonerSpell2 = assetRepository.GetSummonerSpellAsset(playerInfo.Spell2Id),
                PrimaryRune = assetRepository.GetRuneAsset(playerStats.Perk0),
                SecondaryRune = assetRepository.GetRuneAsset(playerStats.PerkSubStyle)
            };
        }

        /// <summary>
        /// Vypocte statistiky pro dany seznam her pro daneho hrace
        /// </summary>
        /// <param name="matchInfoList">Seznam her, pro ktere se vypoctou statistiky</param>
        /// <param name="summoner">Info o hraci v danych hrach - pro nej se statistiky pocitaji</param>
        /// <returns></returns>
        public MatchListStatsDto GetMatchListStats(SummonerModel summoner, IEnumerable<MatchModel> matchInfoList) {
            var totals = new GameListStats();
            var result = new MatchListStatsDto();

            foreach (var matchInfo in matchInfoList) {
                CalculateStatTotals(summoner, matchInfo, result, totals); // vypocet statistik
            }

            GameStatsUtils.CalculateAverages(result, totals);

            logger.LogDebug($"GameStats calculated, result: {result}");
            return result;
        }

        /// <summary>
        /// Pomocna metoda, ktera vypocte data z kazde hry a ulozi je do objektu "totals", ktery slouzi pro prehlednejsi
        /// ukladani
        /// </summary>
        /// <param name="summoner">Reference na summoner info</param>
        /// <param name="match">Reference na match info</param>
        /// <param name="matchListStats">Statistiky pro seznam her, ktery zobrazujeme</param>
        /// <param name="totals">Objekt s celkovymi pocty</param>
        /// <exception cref="ActionNotSuccessfulException">Pokud je hrac null nebo je hracuv team null</exception>
        private void CalculateStatTotals(SummonerModel summoner, MatchModel match,
            MatchListStatsDto matchListStats, GameListStats totals) {
            var playerInfo = match.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summoner.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error player info is null for the given match");
            }

            var playerTeam = match.Teams.FirstOrDefault(team => team.TeamId == playerInfo.TeamId);
            if (playerTeam is null) {
                throw new ActionNotSuccessfulException("Error player team is null for the given match");
            }

            GameStatsUtils.UpdateRoleFrequency(playerInfo, totals.Roles); // Aktualizace role je i pro remake

            if (GameStatsUtils.IsRemake(match.GameDuration)) {
                logger.LogDebug("Found a remake game, increasing number of remakes");
                matchListStats.Remakes += 1;
                return;
            }

            // Jinak pricteme win / loss podle stringu
            if (playerTeam.Win == GameConstants.Win) {
                logger.LogDebug("Found win");
                matchListStats.GamesWon += 1;
            }
            else {
                logger.LogDebug("Found loss");
                matchListStats.GamesLost += 1;
            }

            // Aktualizace stat totals - pricteme celkove smrti, zabiti, asistence ...
            GameStatsUtils.UpdateStatTotals(totals, match, playerInfo, playerTeam);
        }
    }
}