using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Overview;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Pages {
    public class Match : PageModel {
        private readonly MatchService matchService;
        private readonly MatchStatsService matchStatsService;

        public Match(MatchService matchService, MatchStatsService matchStatsService) {
            this.matchService = matchService;
            this.matchStatsService = matchStatsService;
        }

        [FromQuery]
        [BindProperty(SupportsGet = true)]
        public MatchQueryModel QueryParams { get; set; }

        /// <summary>
        /// Mozne servery - EUW, EUNE a NA
        /// </summary>
        public readonly Dictionary<string, string> Servers = ServerConstants.QueryableServers;

        public MatchOverviewDto MatchOverview { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/");
            }

            try {
                var match = await matchService.LoadMatchWithTimeline(QueryParams.GameId,
                    Region.Get(QueryParams.Server.ToLower()));

                MatchOverview = matchStatsService.GetMatchOverview(match, QueryParams.ParticipantId);
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
    }
}