using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Services.MatchHistory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerController : Controller {
        private readonly SummonerInfoService summonerInfoService;
        private readonly MatchHistoryService matchHistoryService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly ILogger<SummonerController> logger;

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

        // GET
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Profile(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty()) {
                return Json("Error");
            }
            // todo exception handling
            var region = Region.Get(server); // server, na kterem hledame
            var summoner = summonerInfoService.GetSummonerInfoAsync(name, region);

            logger.LogDebug($"summoner: {summoner}, region: {region.Key}");

            // Pokud byl v DateTime.MinValue tak to indikuje, ze je summoner prave ziskany z api, takze muzeme
            // zapasy aktualizovat, jinak pouze prineseme posledni z db
            var matchHistory = summoner.LastUpdate == DateTime.MinValue
                ? matchHistoryService.UpdateGameMatchListAsync(summoner,
                    ServerConstants.DefaultNumberOfGamesInProfile)
                : matchHistoryService.GetGameMatchList(summoner, ServerConstants.DefaultNumberOfGamesInProfile);

            var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
            var matchListStats = summonerProfileStatsService.GetMatchListStats(summoner, matchHistory);

            return View(new SummonerOverviewDto(summoner, matchListStats, matchHeaders));
        }
    }
}