using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.MatchDetail.Overview;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;
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
        private readonly IMapper mapper;

        public SummonerProfileStatsService(ILogger<SummonerProfileStatsService> logger,
            AssetRepository assetRepository, IMapper mapper) {
            this.logger = logger;
            this.assetRepository = assetRepository;
            this.mapper = mapper;
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
                                 .FirstOrDefault(player => player.SummonerId == summoner.EncryptedSummonerId)
                             ?? throw new ActionNotSuccessfulException(
                                 "Error while obtaining the data from the database");

            // Statistika hrace
            var playerStats = playerInfo.PlayerStats;

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
                Kda = GameStatsUtils.GetKda(playerStats.Kills, playerStats.Deaths, playerStats.Assists),
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

        public MatchListOverviewDto GetMatchListOverview(SummonerModel summoner, IEnumerable<MatchModel> matchList) {
            var result = new MatchListOverviewDto();

            // Dictionary, do ktereho budeme ukladat data pro danou postavu (podle championId)
            // Z danych objektu pote zjistime statistiky pro jednotlive postavy, ktere muzeme zobrazit ve view
            var championCounters = new Dictionary<int, StatsCounter>();

            // Zaroven chceme i pocitat pres vsechny hry
            var overallStatsCounter = new StatsCounter();
            foreach (var match in matchList) {
                // Pokud je remake preskocime a pouze pricteme pocet remaku
                if (GameStatsUtils.IsRemake(match.GameDuration)) {
                    result.Remakes += 1;
                    continue;
                }

                var player =
                    match.PlayerInfoList.FirstOrDefault(playerModel =>
                        playerModel.SummonerId == summoner.EncryptedSummonerId) ??
                    throw new ObjectNotFoundException($"Player participant is missing for {summoner}");

                if (!championCounters.ContainsKey(player.ChampionId)) {
                    championCounters[player.ChampionId] = new();
                }

                // Aktualizujeme counter pro postavu a s celkovymi staty
                UpdateCounter(championCounters[player.ChampionId], player, match);
                UpdateCounter(overallStatsCounter, player, match);
            }

            // Nyni muzeme vypocitat statistiky pro postavy a vsechny zapasy
            foreach (var (championId, championCounter) in championCounters) {
                var stats = CalculateStats(championCounter, championId) as ChampionCumulativeStatsDto;
                if (stats is null) {
                    continue;
                }

                // Nacteme champion asset
                stats.ChampionAsset = assetRepository.GetChampionAsset(championId);

                // pridame do vysledneho dto
                result.ChampionCumulativeStatsDict[championId] = stats;
            }

            var totalStats = CalculateStats(overallStatsCounter);
            result.TotalStats = totalStats ?? new();

            return result;
        }

        private static CumulativeStatsDto CalculateStats(StatsCounter statsCounter, int? championId = null) {
            // Protoze ChampionCumulativeStatsDto extenduje CumulativeStatsDto muzeme vratit rodice a ten
            // se pote pretypuje zpet - nemusime tak mit 2 funkce pro temer stejne objekty
            var result = championId is null
                ? new CumulativeStatsDto()
                : new ChampionCumulativeStatsDto();

            // Nema smysl nic pocitat pokud se nehrali zadne hry
            if (statsCounter.Wins == 0 && statsCounter.Losses == 0) {
                return result;
            }

            result.Wins = statsCounter.Wins;
            result.Losses = statsCounter.Losses;
            result.Winrate = GameStatsUtils.GetWinratePercentage(result.Wins, result.Losses);
            result.AverageKills = statsCounter.Kills.Average();
            result.AverageDeaths = statsCounter.Deaths.Average();
            result.AverageAssists = statsCounter.Assists.Average();
            result.AverageGoldDiffAt10 =
                statsCounter.GoldDiffsAt10.IsNullOrEmpty() ? 0.0 : statsCounter.GoldDiffsAt10.Average();
            // Kda nechceme pocitat jako prumer vsech kda ale jako sumu zabiti, smrti a asistenci
            result.AverageKda = GameStatsUtils.GetKda(
                statsCounter.Kills.Sum(), statsCounter.Deaths.Sum(), statsCounter.Assists.Sum());
            result.AverageDamageShare = statsCounter.DamageShares.Average() * 100;
            result.AverageGoldShare = statsCounter.TeamGoldShares.Average() * 100;
            result.AverageKillParticipation = statsCounter.KillParticipations.Average() * 100;
            result.AverageCsPerMinute = statsCounter.CsPerMinute.Average();
            result.AverageCs = statsCounter.Cs.Average();
            result.AverageVisionShare = statsCounter.VisionShare.Average() * 100;

            // Chceme serazeny seznam sestupne ze slovniku obsahujici string : cetnost
            // tzn prevedeme na list KeyValuePair objektu a seradime podle hodnoty cetnosti
            var rolesByFrequency = statsCounter.Roles.ToList();
            rolesByFrequency.Sort((x, y) => y.Value.CompareTo(x.Value));

            var totalGames = statsCounter.Wins + statsCounter.Losses;
            if (rolesByFrequency.Count > 1) {
                result.SecondaryRole = rolesByFrequency[1].Key;
                result.SecondaryRoleFrequency =
                    (double) rolesByFrequency[1].Value / totalGames * 100;
            }

            result.PrimaryRole = rolesByFrequency[0].Key;
            result.PrimaryRoleFrequency = (double) rolesByFrequency[0].Value / totalGames * 100;

            return result;
        }

        private static void UpdateCounter(StatsCounter counter, PlayerModel player, MatchModel match) {
            // Ziskame hracovi spoluhrace
            var playerTeam = match.PlayerInfoList.Where(matchPlayer =>
                matchPlayer.TeamId == player.TeamId).ToList();

            // Mapping statistik
            var playerTeamStats = playerTeam.Select(teammate =>
                teammate.PlayerStats).ToList();

            var playerStats = player.PlayerStats;
            counter.Games += 1;
            counter.Kills.Add(playerStats.Kills);
            counter.Deaths.Add(playerStats.Deaths);
            counter.Assists.Add(playerStats.Assists);
            counter.Kdas.Add(
                GameStatsUtils.GetKda(playerStats.Kills, playerStats.Deaths, playerStats.Assists));
            counter.Cs.Add(GameStatsUtils.GetTotalCs(playerStats));
            counter.TeamGoldShares.Add(GameStatsUtils.GetGoldShare(playerStats, playerTeamStats));
            counter.CsPerMinute.Add(GameStatsUtils.GetCsPerMinute(playerStats, match.GameDuration));
            counter.DamageShares.Add(GameStatsUtils.GetDamageShare(playerStats, playerTeamStats));
            counter.KillParticipations.Add(GameStatsUtils.GetKillParticipation(playerStats, playerTeamStats));
            counter.VisionShare.Add(GameStatsUtils.GetVisionShare(playerStats, playerTeamStats));
            var role = GameStatsUtils.GetRole(player.Role, player.Lane);
            if (!counter.Roles.ContainsKey(role)) {
                counter.Roles[role] = 0;
            }

            counter.Roles[role] += 1;

            if (playerStats.Win) {
                counter.Wins += 1;
            }
            else {
                counter.Losses += 1;
            }

            // GoldDiffAt10 je nullable
            if (player.GoldDiffAt10 is not null) {
                // Z nejakeho duvodu se to muselo pretypovat na double i kdyz je predtim null check
                counter.GoldDiffsAt10.Add((double) player.GoldDiffAt10);
            }
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
            GameStatsUtils.UpdateChampionFrequency(playerInfo, totals.Champions);
            if (GameStatsUtils.IsRemake(match.GameDuration)) {
                logger.LogDebug("Found a remake game, increasing number of remakes");
                matchListStats.Remakes += 1;
                return;
            }

            // Jinak pricteme win / loss podle stringu
            if (playerTeam.Win == GameConstants.Win) {
                matchListStats.GamesWon += 1;
            }
            else {
                matchListStats.GamesLost += 1;
            }

            // Aktualizace stat totals - pricteme celkove smrti, zabiti, asistence ...
            GameStatsUtils.UpdateStatTotals(totals, match, playerInfo, playerTeam);
        }

        public SummonerProfileDto GetSummonerProfileDto(SummonerModel summoner) {
            var result = mapper.Map<SummonerProfileDto>(summoner);

            var soloqModel = summoner.QueueInfoList.FirstOrDefault(queue =>
                queue.QueueType == GameConstants.RankedSolo);

            var flexqModel = summoner.QueueInfoList.FirstOrDefault(queue =>
                queue.QueueType == GameConstants.RankedFlex);

            result.SoloQueue = GetQueueInfoDto(soloqModel, GameConstants.RankedSoloDbValue);
            result.FlexQueue = GetQueueInfoDto(flexqModel, GameConstants.RankedFlexDbValue);
            result.ProfileIconRelativeAssetPath = assetRepository.GetProfileIcon(summoner.ProfileIconId);
            return result;
        }

        private QueueInfoDto GetQueueInfoDto(QueueInfoModel queueInfoModel, string name) {
            if (queueInfoModel is null) {
                return null;
            }

            var queueDto = mapper.Map<QueueInfoDto>(queueInfoModel);
            queueDto.Name = name;
            queueDto.Winrate = GameStatsUtils.GetWinratePercentage(queueInfoModel.Wins, queueInfoModel.Losses);
            queueDto.RankAsset = assetRepository.GetRankedIcon(queueInfoModel.Tier);
            return queueDto;
        }
    }
}