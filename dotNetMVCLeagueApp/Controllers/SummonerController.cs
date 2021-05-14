using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services.MatchHistory;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerController : Controller {
        private readonly SummonerService summonerService;
        private readonly MatchHistoryService matchHistoryService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly ILogger<SummonerController> logger;

        /// <summary>
        /// Vsechny typy her, ktere lze filtrovat
        /// </summary>
        public static readonly List<Pair<string, string>> Queues = new() {
            new Pair<string, string>(ServerConstants.AllGamesDbValue, ServerConstants.AllGamesText),
            new Pair<string, string>(ServerConstants.RankedSoloDbValue, ServerConstants.RankedSoloText),
            new Pair<string, string>(ServerConstants.RankedFlexDbValue, ServerConstants.RankedFlexText)
        };

        public class MatchListFilterForm {
            public string Name { get; set; }
            public string Server { get; set; }
            public int NumberOfGames { get; set; }
            public string Queue { get; set; }
        }

        public static readonly int[] PageSize = {10, 20, 30};

        private const string SummonerNotFound = "Error, summoner does not exist";
        private const string ServerOrSummonerNull = "Error, either summoner does not exist or the server is invalid";

        public SummonerController(
            SummonerService summonerService,
            MatchHistoryService matchHistoryService,
            SummonerProfileStatsService summonerProfileStatsService,
            ILogger<SummonerController> logger) {
            this.summonerProfileStatsService = summonerProfileStatsService;
            this.summonerService = summonerService;
            this.matchHistoryService = matchHistoryService;
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult MatchList([FromForm] MatchListFilterForm form) {
            if (form.Name.IsNullOrEmpty() || form.Server.IsNullOrEmpty()
                                          || form.NumberOfGames == 0
                                          || form.Queue.IsNullOrEmpty()) {
                return Redirect("/");
            }

            if (!PageSize.Contains(form.NumberOfGames)) {
                return Redirect("/");
            }

            if (!Queues.Exists(item => item.First == form.Queue)) {
                return Redirect("/");
            }

            try {
                var region = Region.Get(form.Server);
                var summoner = summonerService.GetSummonerAsync(form.Name, region);
                var matchHistory = form.Queue == ServerConstants.AllGamesDbValue
                    ? matchHistoryService.GetMatchlist(summoner, form.NumberOfGames)
                    : matchHistoryService.GetMatchlist(summoner, form.NumberOfGames, form.Queue);

                var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
                var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);

                return PartialView("Partial/_MatchListPartial", new MatchListDto(
                    matchHeaders, matchListOverview
                ));
            }
            catch {
                return Redirect("/");
            }
        }

        // [HttpGet]
        // public IActionResult Refresh(string name, string server) {
        //     if (name.IsNullOrEmpty() || server.IsNullOrEmpty() ||
        //         !ServerConstants.QueryableServers.ContainsKey(server.ToLower())) {
        //         TempData["ErrorMessage"] = ServerOrSummonerNull;
        //         return RedirectToAction("Index", "Root");
        //     }
        //
        //     try {
        //         var region = Region.Get(server);
        //         var summoner = summonerService.GetSummonerAsync(name, region);
        //         summonerService.UpdateSummonerInfoAsync(summoner.Id);
        //         matchHistoryService.UpdateGameMatchListAsync(summoner, ServerConstants.DefaultNumberOfGamesInProfile);
        //         return RedirectToAction("Index", "Summoner", new {name = summoner.Name, server = server});
        //     }
        //     catch (Exception ex) {
        //         TempData["ErrorMessage"] = ex.Message;
        //         switch (ex) {
        //             case ActionNotSuccessfulException: {
        //                 return RedirectToAction("Index", "Summoner", new {name = name, server = server});
        //             }
        //             case RedirectToHomePageException: {
        //                 return RedirectToAction("Index", "Root");
        //             }
        //             default: {
        //                 TempData["ErrorMessage"] = null;
        //                 return RedirectToAction("Index", "Root");
        //             }
        //         }
        //     }
        // }


        /// <summary>
        /// Render index stranky
        /// </summary>
        /// <param name="name">jmeno hrace</param>
        /// <param name="server">server, na kterem se vyskytuje</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty() ||
                !ServerConstants.QueryableServers.ContainsKey(server.ToLower())) {
                TempData["ErrorMessage"] = ServerOrSummonerNull;
                return RedirectToAction("Index", "Root");
            }

            try {
                var region = Region.Get(server);
                var summoner = summonerService.GetSummonerAsync(name, region);
                var summonerProfileDto = summonerProfileStatsService.GetSummonerProfileDto(summoner);

                var matchHistory =
                    matchHistoryService.GetMatchlist(summoner, ServerConstants.DefaultNumberOfGamesInProfile);

                var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
                var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);

                return View(
                    new SummonerOverviewDto(summonerProfileDto, ServerConstants.AllGames, matchListOverview,
                        matchHeaders)
                );
            }
            // Odchytime exception a pokud to jsou "nase" tak vratime uzivatele s upozornenim, jinak
            // presmerujeme na Index bez zpravy
            catch (Exception exception) {
                if (exception is ActionNotSuccessfulException or RiotApiException) {
                    TempData["ErrorMessage"] = exception.Message;
                    return RedirectToAction("Index", "Root");
                }

                logger.LogCritical(exception.Message);
                return Redirect("/");
            }
        }
    }
}