using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Services;
using Microsoft.AspNetCore.Mvc;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerInfoController : Controller {
        private readonly SummonerInfoService summonerInfoService;

        private readonly MatchHistoryService matchHistoryService;

        public SummonerInfoController(SummonerInfoService summonerInfoService,
            MatchHistoryService matchHistoryService) {
            this.summonerInfoService = summonerInfoService;
            this.matchHistoryService = matchHistoryService;
        }

        public IActionResult Index() {
            return View();
        }

        public SummonerInfoModel SummonerInfo(string summonerName, string server) {
            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return null;
            }

            return summonerInfoService.GetSummonerInfo(summonerName, region).GetAwaiter().GetResult();
        }

        public List<MatchInfoModel> MatchHistory(string summonerName, string server) {
            Region region;
            try {
                region = Region.Get(server);
            }
            catch {
                return null;
            }

            var summoner = summonerInfoService.GetSummonerInfo(summonerName, region).GetAwaiter().GetResult();
            return matchHistoryService.GetGameMatchList(summoner, 20);
        }
    }
}