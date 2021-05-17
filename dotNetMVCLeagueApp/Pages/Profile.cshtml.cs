using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.Core.Logging;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Pages.Data.Profile;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Services.Summoner;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Pages {
    public class Profile : PageModel {
        private readonly SummonerService summonerService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly MatchService matchService;
        private readonly ILogger<Profile> logger;

        public Profile(SummonerService summonerService,
            SummonerProfileStatsService summonerProfileStatsService,
            ILogger<Profile> logger,
            MatchService matchService) {
            this.summonerService = summonerService;
            this.summonerProfileStatsService = summonerProfileStatsService;
            this.logger = logger;
            this.matchService = matchService;
        }

        /// <summary>
        /// Mozne filtry pro typ queue - vsechny, solo queue a flex queue
        /// </summary>
        public readonly Dictionary<string, string> QueueFilters = ServerConstants.QueueFilters;

        /// <summary>
        /// Mozne servery - EUW, EUNE a NA
        /// </summary>
        public readonly Dictionary<string, string> Servers = ServerConstants.QueryableServers;

        /// <summary>
        /// Pocet her na strance - 10, 20 nebo 30. Vetsi pocet by take fungoval, nicmene realne bychom
        /// nechteli pocitat a iterovat pro jakkoliv velka data, takze je to zde omezeno takto
        /// </summary>
        public readonly HashSet<int> NumberOfGames = new() {ServerConstants.DefaultPageSize, 15, 20};

        [BindProperty(SupportsGet = true)] public ProfileQueryModel QueryParams { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        /// <summary>
        /// Data daneho summonera
        /// </summary>
        public SummonerOverviewDto SummonerData { get; set; }

        /// <summary>
        /// Zkontroluje a pripadne upravi query parametry, abychom nemuseli uzivatele presmerovavat
        /// na Index. Tyto parametry jsou pouze pro p
        /// </summary>
        private void CheckQueryParams() {
            // Filtr nastavime bud jako vsechny hry, nebo specificky pokud je spravne
            QueryParams.Filter = QueueFilters.ContainsKey(QueryParams.Filter ?? "")
                ? QueueFilters[QueryParams.Filter!]
                : ServerConstants.AllGamesDbValue;

            // Pocet her je bud vychozich 10, nebo specificky pokud je spravne
            QueryParams.PageSize = NumberOfGames.Contains(QueryParams.PageSize)
                ? QueryParams.PageSize
                : ServerConstants.DefaultPageSize;
        }

        public async Task<IActionResult> OnGetAsync() {
            logger.LogDebug("Profile.cshtml -> OnGet");
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/Index");
            }

            CheckQueryParams();

            // Region, pro ktery budeme hledat
            var server = Region.Get(QueryParams.Server.ToUpper());

            try {
                var summoner = await summonerService.GetSummoner(QueryParams.Name, server);

                List<MatchModel> matchHistory;
                if (QueryParams.Filter == ServerConstants.AllGamesDbValue
                    && QueryParams.PageSize == ServerConstants.DefaultPageSize
                    && QueryParams.Offset == 0) {
                    matchHistory = matchService.GetFrontPage(summoner);
                }
                else {
                    var queues = QueryParams.Filter == ServerConstants.AllGamesDbValue
                        ? ServerConstants.RelevantQueues
                        : new[] {ServerConstants.GetQueueId(QueryParams.Filter)};
                    matchHistory = await matchService.GetSpecificPage(summoner, QueryParams.Offset,
                        QueryParams.PageSize, queues);
                }

                SummonerData = GetSummonerData(summoner, matchHistory);
                return Page();
            }
            catch (Exception ex) {
                if (ex is RedirectToHomePageException) {
                    TempData["ErrorMessage"] = ex.Message;
                    return Redirect("/Index");
                }


                logger.LogCritical(ex.Message);
                TempData["ErrorMessage"] = "Summoner does not exist";
                return Redirect("/Index");
            }
        }

        private SummonerOverviewDto GetSummonerData(SummonerModel summoner,
            IReadOnlyCollection<MatchModel> matchHistory) {
            var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
            var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);
            var summonerProfileDto = summonerProfileStatsService.GetSummonerProfileDto(summoner);
            return new SummonerOverviewDto(summonerProfileDto, QueryParams, matchListOverview,
                matchHeaders);
        }

        public async Task<IActionResult> OnPostRefreshAsync() {
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                ErrorMessage = "Error, summoner could not be updated due to invalid parameters";
                return Page();
            }

            logger.LogDebug("Profile.cshtml -> onRefresh");

            // Ziskame server, pro ktery budeme updatovat
            var server = Region.Get(QueryParams.Server.ToUpper());

            try {
                var summoner = await summonerService.GetSummoner(QueryParams.Name, server);
                summoner = await summonerService.UpdateSummoner(summoner.Id, true);
                var matchHistory =
                    await matchService.GetUpdatedMatchHistory(summoner, ServerConstants.DefaultPageSize);
                SummonerData = GetSummonerData(summoner, matchHistory);
                return Page();
            }

            catch (Exception ex) {
                logger.LogDebug(ex.Message);
                switch (ex) {
                    case RedirectToHomePageException:
                        TempData["ErrorMessage"] = ex.Message;
                        return Redirect("/Index");
                    case ActionNotSuccessfulException:
                        ErrorMessage = ex.Message;
                        return await OnGetAsync();
                    default:
                        ErrorMessage = "Error while refreshing user profile";
                        return Page();
                }
            }
        }
    }
}