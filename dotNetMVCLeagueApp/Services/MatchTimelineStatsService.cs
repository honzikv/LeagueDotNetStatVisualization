using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Services.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.Extensions.Logging;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro vypocet statistik z timeline
    /// </summary>
    public class MatchTimelineStatsService {
        private readonly MatchTimelineRepository matchTimelineRepository;
        private readonly ILogger<MatchTimelineStatsService> logger;

        public MatchTimelineStatsService(MatchTimelineRepository matchTimelineRepository,
            ILogger<MatchTimelineStatsService> logger) {
            this.matchTimelineRepository = matchTimelineRepository;
            this.logger = logger;
        }

        public MatchTimelineOverviewDto GetMatchTimelineOverview(int participantId, MatchModel match) {
            if (!match.MatchTimelineSearched) {
                throw new ArgumentException("Error, game has not loaded match timeline yet");
            }

            if (match.MatchTimeline is null) {
                return null;
            }

            var timeline = match.MatchTimeline;
            var result = new MatchTimelineOverviewDto();
            var allPlayers = match.PlayerList.ToList();

            // Hrac, pro ktereho timeline sestavujeme
            var player = allPlayers.Find(playerModel => playerModel.ParticipantId == participantId);
            if (player is null) {
                throw new ActionNotSuccessfulException("Error, player did not play in the game");
            }

            // Oponent na lince
            var opponent = GetOpponent(player, allPlayers);

            // Zkontrolujeme role hrace a oponenta. Nekdy se muze stat ze api nevypocte role spravne
            // V takoveto situaci data vypocteme ale na frontendu zobrazime ze muzou byt v api spatne
            result.IsOpponentAccurate = GameStatsUtils.GetRole(player.Role, player.Lane) ==
                                        GameStatsUtils.GetRole(opponent.Role, opponent.Lane);

            // Vytvorime si pomocny objekt, ktery provede vypocet statistik
            var matchTimelineStatsComputation = new MatchTimelineStatsComputation(player,
                opponent, allPlayers, timeline);
            
            matchTimelineStatsComputation.Process();
            var (playerDetail, matchTimelineStats) = matchTimelineStatsComputation.GetResult;
            result.PlayerDetail = playerDetail;
            result.MatchTimeline = matchTimelineStats;

            return result;
        }

        private PlayerModel GetOpponent(PlayerModel player, List<PlayerModel> players) {
            var playerTeam = player.TeamId; // id tymu hrace - bude blue side nebo red side
            var opponents = players.Where(playerModel => playerModel.TeamId != playerTeam).ToList();

            if (opponents.Count == 0) {
                throw new ActionNotSuccessfulException("Error, no opponents in the game");
            }

            var playerRole = GameStatsUtils.GetRole(player.Role, player.Lane);
            var opponent = opponents.FirstOrDefault(playerModel =>
                GameStatsUtils.GetRole(playerModel.Role, playerModel.Lane) == playerRole);

            return opponent ?? opponents[0];
        }
    }
}