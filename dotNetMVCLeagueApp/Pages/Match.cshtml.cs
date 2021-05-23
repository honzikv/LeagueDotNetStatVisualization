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
using MingweiSamuel.Camille.Util;
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

        /// <summary>
        /// Barvy pro ucastniky, ziskano z https://www.carbondesignsystem.com/data-visualization/color-palettes/
        /// </summary>
        public readonly List<string> ParticipantColors = new() {
            "#6929c4", "#1192e8", "#005d5d", "#9f1853", "#fa4d56", "#002d9c", "#ee538b", "#b28600",
            "#012749", "#570408"
        };

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
                switch (ex) {
                    case ActionNotSuccessfulException:
                        TempData["ErrorMessage"] = ex.Message;
                        return RedirectToPage("Profile",
                            new {
                                Name = QueryParams.SummonerName,
                                Server = QueryParams.Server
                            });
                    case RiotResponseException:
                        TempData["ErrorMessage"] =
                            "There was an error while communicating with Riot servers. Match could not be loaded.";
                        return RedirectToPage("Profile",
                            new {
                                Name = QueryParams.SummonerName,
                                Server = QueryParams.Server
                            });
                    default:
                        TempData["ErrorMessage"] = "There was an unknown error while searching for match details.";
                        return Redirect("/");
                }

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

                return Content(serializedMatchTimelineOverview, "application/json");
            }
            catch (Exception ex) {
                if (ex is RiotApiException) {
                    TempData["ErrorMessage"] =
                        "There was an error while communicating with Riot servers. Match could not be loaded.";
                    return RedirectToPage("Profile",
                        new {
                            Name = QueryParams.SummonerName,
                            Server = QueryParams.Server
                        });
                }

                logger.LogCritical(ex.Message);
                TempData["ErrorMessage"] = "There was an unknown error while loading Match Timeline.";
                return RedirectToPage("Profile",
                    new {
                        Name = QueryParams.SummonerName,
                        Server = QueryParams.Server
                    });
            }
        }
    }
}