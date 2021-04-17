using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Services;
using Microsoft.AspNetCore.Mvc;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerController : Controller {
        private readonly SummonerInfoService summonerInfoService;
        private readonly MatchHistoryService matchHistoryService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;

        public SummonerController(SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService, SummonerProfileStatsService summonerProfileStatsService) {
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
            this.summonerProfileStatsService = summonerProfileStatsService;
        }

        public IActionResult Index() {
            return Json(new {message = "No content yet"});
        }

        public IActionResult Search(string name, string server) {
            if (name is null || server is null) {
                Redirect("/");
            }

            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                region = Region.EUW;
            }

            ViewBag.SummonerInfo = summonerInfoService.GetSummonerInfo(name, region).GetAwaiter().GetResult();
            return View("Index");
        }


        public IActionResult MatchHistory(string name, string server) {
            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return null;
            }

            var summoner = summonerInfoService.GetSummonerInfo(name, region).GetAwaiter().GetResult();
            
            // Ziskame match list ze sluzby
            var matchList = matchHistoryService.GetGameMatchList(summoner, 20);

            // Vytvorime objekty pro view
            foreach (var matchInfo in matchList) {
                
            }
            
            ViewBag.MatchList = matchList;
            return View();
        }

        public async Task<ActionResult> Update(string summonerName, string server) {
            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return null;
            }

            try {
                var summoner = await summonerInfoService.GetSummonerInfo(summonerName, region);
                summoner = await summonerInfoService.UpdateSummonerInfo(summoner.Id);
                var matchList = await matchHistoryService.UpdateGameMatchList(summoner, 20);

                return Json(matchList);
            }
            catch (ActionNotSuccessfulException ex) {
                return Json(new {message = ex.Message});
            }
        }
    }
}