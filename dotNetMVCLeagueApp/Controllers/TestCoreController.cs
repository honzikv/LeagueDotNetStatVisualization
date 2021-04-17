using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class TestCoreController : Controller {
        private readonly SummonerInfoService summonerInfoService;
        private readonly MatchHistoryService matchHistoryService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly ILogger<TestCoreController> logger;

        public TestCoreController(SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService, SummonerProfileStatsService summonerProfileStatsService,
            ILogger<TestCoreController> logger) {
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
            this.summonerProfileStatsService = summonerProfileStatsService;
            this.logger = logger;
        }

        public IActionResult Index() {
            return Json(new {message = "No content yet"});
        }

        public IActionResult Search(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty()) {
                return Redirect("/");
            }

            // Prevod do lowercase
            name = name.ToLower();
            server = server.ToLower();

            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return Error("Unknown server");
            }

            ViewBag.SummonerInfo = summonerInfoService.GetSummonerInfoAsync(name, region);
            return View("Index");
        }


        public IActionResult MatchHistory(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty()) {
                return Redirect("/");
            }

            // Prevod do lowercase
            name = name.ToLower();
            server = server.ToLower();
            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return Error("Unknown server");
            }

            // Ziskame info o hraci
            var summoner = summonerInfoService.GetSummonerInfoAsync(name, region);

            // Ziskame match list ze sluzby
            var matchList = matchHistoryService.GetGameMatchList(summoner, 20);

            // Vytvorime seznam headeru pro view
            var matchInfoHeaders = 
                summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchList);

            var gameListStats = summonerProfileStatsService.GetGameListStatsViewModel(matchList, summoner);

            ViewBag.MatchHeaders = matchInfoHeaders;
            ViewBag.GameListStats = gameListStats;
            return View();
        }

        public IActionResult Refresh(string name, string server) {
            if (name.IsNullOrEmpty() || server.IsNullOrEmpty()) {
                return Redirect("/");
            }
            
            // Prevod do lowercase
            name = name.ToLower();
            server = server.ToLower();
            // Zde staci zavolat update info, odchytit exception a provest redirect
            try {
                logger.LogDebug("");
                var region = Region.Get(server);
                var summoner = summonerInfoService.GetSummonerInfoAsync(name, region);
                var update = summonerInfoService.UpdateSummonerInfoAsync(summoner.Id);
                logger.LogDebug($"Update: {update}");
                
                // Todo: zatim zobrazime puvodni stranku se summonerem takto
                // Tohle v praxi samozrejme nedava moc velky smysl a je lepsi ziskat refresh treba pres ajax
                return RedirectToAction("Search", new {
                    name = name,
                    server = server
                });
            }
            catch (Exception ex) {
                return Error(ex.Message);
            }
            
        }

        public IActionResult Error(string message) {
            ViewBag.Message = message;
            return View("Error");
        }

    }
}