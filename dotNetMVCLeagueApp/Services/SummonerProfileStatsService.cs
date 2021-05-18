using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Pages.Data.Profile;
using dotNetMVCLeagueApp.Repositories.AssetResolver;
using dotNetMVCLeagueApp.Services.Utils;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
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
            var playerInfo = match.PlayerList
                                 .FirstOrDefault(player => player.SummonerId == summoner.EncryptedSummonerId)
                             ?? throw new ActionNotSuccessfulException(
                                 "Error while obtaining the data from the database");

            // Statistika hrace
            var playerStats = playerInfo.PlayerStats;
            
            // Nyni objekt muzeme rovnou inicializovat pomoci {}
            return new MatchHeaderDto {
                Duration = TimeSpan.FromSeconds(match.GameDuration),
                GameId = match.Id,
                Server = summoner.Region,
                SummonerName = summoner.Name,
                QueueType = match.QueueType,
                ParticipantId = playerInfo.ParticipantId,
                TeamId = playerInfo.TeamId,
                Kills = playerStats.Kills,
                Deaths = playerStats.Deaths,
                Assists = playerStats.Assists,
                Gold = playerStats.GoldEarned,
                DamageDealt = playerStats.TotalDamageDealtToChampions,
                VisionScore = playerStats.VisionScore,
                IsRemake = GameStatsUtils.IsRemake(match.GameDuration),
                LargestMultiKill = GameStatsUtils.GetLargestMultiKill(playerStats),
                CsPerMinute = GameStatsUtils.GetCsPerMinute(playerStats, match.GameDuration),
                TotalCs = GameStatsUtils.GetTotalCs(playerStats),
                KillParticipation =
                    GameStatsUtils.GetKillParticipationPercentage(playerStats, match, playerInfo.TeamId),
                Kda = GameStatsUtils.GetKda(playerStats.Kills, playerStats.Deaths, playerStats.Assists),
                PlayTime = TimeUtils.GetTimeFromToday(match.PlayTime),
                Win = match.Teams.FirstOrDefault(
                    team => team.TeamId == playerInfo.TeamId)?.Win == ServerConstants.Win,
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

                var player = match.PlayerList.FirstOrDefault(playerModel =>
                                 playerModel.SummonerId == summoner.EncryptedSummonerId) ??
                             throw new RedirectToHomePageException(
                                 $"There was an error with the database. Data for summoner: {summoner.Name} is " 
                                 + "corrupt. We are sorry for the inconvenience.");

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
            result.AverageGoldDiffAt20 =
                statsCounter.GoldDiffsAt20.IsNullOrEmpty() ? 0.0 : statsCounter.GoldDiffsAt20.Average();

            result.AverageCsDiffAt10 =
                statsCounter.CsDiffsAt10.IsNullOrEmpty() ? 0.0 : statsCounter.CsDiffsAt10.Average();
            result.AverageCsDiffAt20 =
                statsCounter.CsDiffsAt20.IsNullOrEmpty() ? 0.0 : statsCounter.CsDiffsAt20.Average();

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
            var playerTeam = match.PlayerList.Where(matchPlayer =>
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

            // Rozdily jsou nullable - hra mohla skoncit drive nebo nebyly pocitane, proto musime udelat
            // nullcheck

            if (player.GoldDiffAt10 is not null) {
                counter.GoldDiffsAt10.Add((double) player.GoldDiffAt10);
            }

            if (player.GoldDiffAt20 is not null) {
                counter.GoldDiffsAt20.Add((double) player.GoldDiffAt20);
            }

            if (player.CsDiffAt10 is not null) {
                counter.CsDiffsAt10.Add((double) player.CsDiffAt10);
            }

            if (player.CsDiffAt20 is not null) {
                counter.CsDiffsAt20.Add((double) player.CsDiffAt20);
            }
        }

        public SummonerProfileDto GetSummonerProfileDto(SummonerModel summoner) {
            var result = mapper.Map<SummonerProfileDto>(summoner);

            var soloqModel = summoner.QueueInfoList.FirstOrDefault(queue =>
                queue.QueueType == ServerConstants.RankedSolo);

            var flexqModel = summoner.QueueInfoList.FirstOrDefault(queue =>
                queue.QueueType == ServerConstants.RankedFlex);

            result.SoloQueue = GetQueueInfoDto(soloqModel, ServerConstants.RankedSoloDbValue);
            result.FlexQueue = GetQueueInfoDto(flexqModel, ServerConstants.RankedFlexDbValue);
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