﻿using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview;
using dotNetMVCLeagueApp.Repositories.AssetResolver;
using dotNetMVCLeagueApp.Services.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    public class MatchStatsService {
        private readonly AssetRepository assetRepository;
        private readonly ILogger<MatchService> logger;

        public MatchStatsService(AssetRepository assetRepository, ILogger<MatchService> logger) {
            this.assetRepository = assetRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Ziska overview objekt pro frontend
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public MatchOverviewDto GetMatchOverview(MatchModel match) {
            // Nejprve vytvorime slovnik kde jsou (nebo by mely byt) dva tymy - RedSide (200) a BlueSide (100)
            // Slovnik dale pouzijeme k mapovani dat z hracu a data ze slovniku ulozime do objektu pro frontend
            var teamsDictionary = GetTeamDtoDictionary(match);

            if (!teamsDictionary.ContainsKey(ServerConstants.BlueSideId) ||
                !teamsDictionary.ContainsKey(ServerConstants.RedSideId)) {
                throw new ActionNotSuccessfulException(
                    "Error, data in database is corrupt, cannot show match overview");
            }
            
            var teams = new MatchTeamsDto() {
                BlueSide = teamsDictionary[ServerConstants.BlueSideId],
                RedSide = teamsDictionary[ServerConstants.RedSideId]
            };
            
            return new MatchOverviewDto {
                IsRemake = GameStatsUtils.IsRemake(match.GameDuration),
                PlayTime = match.PlayTime,
                Teams = teams,
                Players = MapPlayers(match, teamsDictionary)
            };
        }

        private Dictionary<int, TeamDto> GetTeamDtoDictionary(MatchModel match) {
            var result = new Dictionary<int, TeamDto>();

            foreach (var team in match.Teams) {
                var teamPlayers = match.PlayerList.Where(player => player.TeamId == team.TeamId).ToList();
                logger.LogDebug($"Team: id - {team.Id}, teamId: {team.TeamId}");
                result[team.TeamId] = new() {
                    Win = team.Win == ServerConstants.Win,
                    TeamName = team.TeamId == ServerConstants.BlueSideId ? ServerConstants.BlueSide
                        : ServerConstants.RedSide,
                    Gold = teamPlayers.Sum(x => x.PlayerStats.GoldEarned),
                    TotalKills = teamPlayers.Sum(x => x.PlayerStats.Kills),
                    TotalDeaths = teamPlayers.Sum(x => x.PlayerStats.Deaths),
                    Barons = team.BaronKills,
                    Dragons = team.DragonKills,
                    TurretKills = team.TowerKills,
                    InhibitorKills = team.InhibitorKills
                };

            }

            return result;
        }

        private Dictionary<int, PlayerDto> MapPlayers(MatchModel match, Dictionary<int, TeamDto> teams) {
            var result = new Dictionary<int, PlayerDto>();
            foreach (var player in match.PlayerList) {
                var playerTeam = teams[player.TeamId] ??
                                 throw new ActionNotSuccessfulException("Error, database data is corrupt");
                var playerStats = player.PlayerStats;
                result[player.ParticipantId] = new() {
                    ParticipantId = player.ParticipantId,
                    TeamId = player.TeamId,
                    SummonerName = player.SummonerName,
                    ChampionId = player.ChampionId,
                    ChampionAsset = assetRepository.GetChampionAsset(player.ChampionId),
                    Kills = playerStats.Kills,
                    Deaths = playerStats.Deaths,
                    Assists = playerStats.Assists,
                    KillParticipation = GameStatsUtils.GetKillParticipationFromTeamInfoDto(player, playerTeam),
                    Items = new() {
                        assetRepository.GetItemAsset(playerStats.Item0),
                        assetRepository.GetItemAsset(playerStats.Item1),
                        assetRepository.GetItemAsset(playerStats.Item2),
                        assetRepository.GetItemAsset(playerStats.Item3),
                        assetRepository.GetItemAsset(playerStats.Item4),
                        assetRepository.GetItemAsset(playerStats.Item5),
                        assetRepository.GetItemAsset(playerStats.Item6)
                    },
                    Cs = GameStatsUtils.GetTotalCs(playerStats),
                    Kda = GameStatsUtils.GetKda(playerStats.Kills, playerStats.Deaths, playerStats.Assists),
                    CsPerMinute = GameStatsUtils.GetCsPerMinute(playerStats, match.GameDuration),
                    SummonerSpell1 = assetRepository.GetSummonerSpellAsset(player.Spell1Id),
                    SummonerSpell2 = assetRepository.GetSummonerSpellAsset(player.Spell2Id),
                    VisionScore = playerStats.VisionScore,
                    PrimaryRune = assetRepository.GetRuneAsset(playerStats.Perk0),
                    SecondaryRune = assetRepository.GetRuneAsset(playerStats.PerkSubStyle),
                    Gold = playerStats.GoldEarned,
                    DamageDealt = playerStats.TotalDamageDealtToChampions,
                    GoldDiffAt10 = player.GoldDiffAt10,
                    GoldDiffAt20 = player.GoldDiffAt20,
                    CsDiffAt10 = player.CsDiffAt10,
                    CsDiffAt20 = player.CsDiffAt20,
                    Level = playerStats.ChampLevel
                };
            }

            return result;
        }
    }
}