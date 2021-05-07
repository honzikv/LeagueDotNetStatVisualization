using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Controllers.Forms;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Services.MatchHistory;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

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
            new Pair<string, string>(GameConstants.AllGamesDbValue, GameConstants.AllGamesText),
            new Pair<string, string>(GameConstants.RankedSoloDbValue, GameConstants.RankedSoloText),
            new Pair<string, string>(GameConstants.RankedFlexDbValue, GameConstants.RankedFlexText)
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
        public IActionResult MatchList([FromForm] MatchListFilterForm form) {
            if (form.Name.IsNullOrEmpty() || form.Server.IsNullOrEmpty()
                                          || form.NumberOfGames == 0
                                          || form.Queue.IsNullOrEmpty()) {
                return BadRequest(form.ToString());
            }

            if (!NumberOfGames.Contains(form.NumberOfGames)) {
                return BadRequest("Illegal number of games to update");
            }

            if (!Queues.Exists(item => item.First == form.Queue)) {
                return BadRequest("Illegal queue");
            }

            try {
                var region = Region.Get(form.Server);
                var summoner = summonerInfoService.GetSummonerInfoAsync(form.Name, region);
                var matchHistory = form.Queue == GameConstants.AllGamesDbValue
                    ? matchHistoryService.GetGameMatchList(summoner, form.NumberOfGames)
                    : matchHistoryService.GetGameMatchList(summoner, form.NumberOfGames, form.Queue);

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