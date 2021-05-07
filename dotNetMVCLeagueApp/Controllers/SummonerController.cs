using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Services.MatchHistory;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using Xunit;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerController : Controller {
        private readonly SummonerInfoService summonerInfoService;
        private readonly MatchHistoryService matchHistoryService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly ILogger<SummonerController> logger;

        /// <summary>
        /// Vsechny typy her, ktere lze filtrovat
        /// </summary>
        public static readonly List<Pair<string, string>> Queues = new() {
            new Pair<string, string>(GameConstants.AllGames, GameConstants.AllGamesName),
            new Pair<string, string>(GameConstants.RankedSolo, GameConstants.RankedSoloName),
            new Pair<string, string>(GameConstants.RankedFlex, GameConstants.RankedFlexName)
        };

        public static readonly int[] NumberOfGames = {10, 20, 30};

        public SummonerController(
            SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService,
            SummonerProfileStatsService summonerProfileStatsService,
            ILogger<SummonerController> logger) {
            this.summonerProfileStatsService = summonerProfileStatsService;
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult MatchList(
            [FromBody] string name, 
            [FromBody] string server, 
            [FromBody] int numberOfGames,
            [FromBody] string queue) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty() || numberOfGames == 0 || queue.IsNullOrEmpty()) {
                return BadRequest("Name or server is empty");
            }

            if (!NumberOfGames.Contains(numberOfGames)) {
                return BadRequest("Illegal number of games to update");
            }

            if (!Queues.Exists(item => item.First == queue)) {
                return BadRequest("Illegal queue");
            }

            try {
                var region = Region.Get(server);
                var summoner = summonerInfoService.GetSummonerInfoAsync(name, region);
                var matchHistory = queue == GameConstants.AllGames
                    ? matchHistoryService.GetGameMatchList(summoner, numberOfGames)
                    : matchHistoryService.GetGameMatchList(summoner, numberOfGames, queue);

                var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
                var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);

                return PartialView("Partial/_MatchListPartial", new MatchListDto(
                    matchHeaders, matchListOverview
                ));
            }
            // todo: redirect
            catch {
                throw;
            }
        }


        /// <summary>
        /// Render index stranky
        /// </summary>
        /// <param name="name">jmeno hrace</param>
        /// <param name="server">server, na kterem se vyskytuje</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty()) {
                return BadRequest("Name or server is empty");
            }

            try {
                var region = Region.Get(server); // server, na kterem hledame
                var summoner = summonerInfoService.GetSummonerInfoAsync(name, region);
                var summonerProfileDto = summonerProfileStatsService.GetSummonerProfileDto(summoner);

                logger.LogDebug($"summoner: {summoner}, region: {region.Key}");

                var matchHistory =
                    matchHistoryService.GetGameMatchList(summoner, ServerConstants.DefaultNumberOfGamesInProfile);

                var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
                var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);

                return View(new SummonerOverviewDto(summonerProfileDto, GameConstants.AllGames, matchListOverview,
                    matchHeaders));
            }

            catch {
                throw; // todo odstranit
                return Redirect("/");
            }
        }
    }

}