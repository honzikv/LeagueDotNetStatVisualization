using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;
using Newtonsoft.Json;

namespace dotNetMVCLeagueApp.Pages {
    public class Match : PageModel {
        private readonly MatchService matchService;
        private readonly MatchStatsService matchStatsService;
        private readonly MatchTimelineStatsService matchTimelineStatsService;
        private readonly ILogger<Match> logger;

        public Match(MatchService matchService, MatchStatsService matchStatsService,
            MatchTimelineStatsService matchTimelineStatsService,
            ILogger<Match> logger) {
            this.matchService = matchService;
            this.matchStatsService = matchStatsService;
            this.matchTimelineStatsService = matchTimelineStatsService;
            this.logger = logger;
        }

        [FromQuery]
        [BindProperty(SupportsGet = true)]
        public MatchQueryDto QueryParams { get; set; }

        /// <summary>
        /// Mozne servery - EUW, EUNE a NA
        /// </summary>
        public readonly Dictionary<string, string> Servers = ServerConstants.QueryableServers;

        public MatchOverviewDto MatchOverview { get; set; }

        public MatchTimelineOverviewDto MatchTimelineOverview { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/");
            }

            try {
                var match = await matchService.LoadMatchWithTimeline(QueryParams.GameId,
                    Region.Get(QueryParams.Server.ToLower()));

                MatchOverview =
                    matchStatsService.GetMatchOverview(match, QueryParams.ParticipantId, QueryParams.Server);
                return Page();
            }
            catch (Exception ex) {
                if (ex is ActionNotSuccessfulException) {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToPage("Profile",
                        new {
                            Name = QueryParams.SummonerName,
                            Server = QueryParams.Server
                        });
                }

                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/");
            }
        }

        public async Task<IActionResult> OnGetMatchTimelineAsync() {
            logger.LogDebug("Getting match timeline from ajax");
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/");
            }

            try {
                var match = await matchService.LoadMatchWithTimeline(QueryParams.GameId,
                    Region.Get(QueryParams.Server.ToLower()));

                MatchTimelineOverview =
                    matchTimelineStatsService.GetMatchTimelineOverview(QueryParams.ParticipantId, match);

                logger.LogDebug(
                    "Match timeline overview constructed, returing as partial view. " +
                    $"Timeline was null = {MatchTimelineOverview is null}");

                var serializedMatchTimelineOverview = JsonConvert.SerializeObject(MatchTimelineOverview,
                    new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                logger.LogDebug(serializedMatchTimelineOverview);
                return Content(serializedMatchTimelineOverview, "application/json");
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                return new JsonResult(new {
                    StatusMessage = "Could not load match timeline"
                });
            }
        }
    }
}