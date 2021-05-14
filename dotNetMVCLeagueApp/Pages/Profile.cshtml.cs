using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.FrontendDtos.Summoner;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Pages.Data.Profile;
using dotNetMVCLeagueApp.Services.MatchHistory;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Pages {
    public class Profile : PageModel {

        private readonly SummonerService summonerService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly MatchService matchService;

        public Profile(SummonerService summonerService, 
            SummonerProfileStatsService summonerProfileStatsService,
            MatchService matchService) {
            this.summonerService = summonerService;
            this.summonerProfileStatsService = summonerProfileStatsService;
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
        public readonly HashSet<int> NumberOfGames = new() {ServerConstants.DefaultNumberOfGamesInProfile, 20, 30};

        [BindProperty(SupportsGet = true)] public ProfileQueryModel QueryParams { get; set; }
        
        /// <summary>
        /// Data daneho summonera
        /// </summary>
        public SummonerOverviewDto SummonerData { get; set; }

        public async Task<IActionResult> OnGetAsync() {
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/Index");
            }

            // Filtr nastavime bud jako vsechny hry, nebo specificky pokud je spravne
            var queueType = QueueFilters.ContainsKey(QueryParams.Filter ?? "")
                ? QueueFilters[QueryParams.Filter!]
                : ServerConstants.AllGamesDbValue;
            
            // Pocet her je bud vychozich 10, nebo specificky pokud je spravne
            var pageSize = NumberOfGames.Contains(QueryParams.PageSize)
                ? QueryParams.PageSize
                : ServerConstants.DefaultNumberOfGamesInProfile;

            var pageNumber = QueryParams.PageNumber > 0 ? QueryParams.PageNumber : 0;
            
            // Region, pro ktery budeme hledat
            var server = Region.Get(QueryParams.Server.ToUpper());

            try {
                var summoner = await summonerService.GetSummoner(QueryParams.Name, server);
                var summonerProfileDto = summonerProfileStatsService.GetSummonerProfileDto(summoner);

                var update = summonerService.IsSummonerUpdateable(summoner) && pageNumber != 0;
                var (matchHistory, updated) = await matchService.GetFilteredMatchHistory(summoner,
                    pageSize, pageNumber, queueType, update);
                
                // pokud doslo k aktualizaci, aktualizujeme i profil uzivatele
                if (updated) {
                    summoner = await summonerService.UpdateSummoner(summoner.Id, true);
                }

                var matchHeaders = summonerProfileStatsService.GetMatchInfoHeaderList(summoner, matchHistory);
                var matchListOverview = summonerProfileStatsService.GetMatchListOverview(summoner, matchHistory);

                SummonerData = new SummonerOverviewDto(summonerProfileDto, ServerConstants.AllGames, matchListOverview,
                    matchHeaders);

                return Page();
            }
            catch (Exception ex) {
                if (ex is RedirectToHomePageException) {
                    TempData["ErrorMessage"] = ex.Message;
                    return Redirect("/Index");
                }

                TempData["ErrorMessage"] =
                    "There was an error while fetching the data, we are sorry. Please try again later...";
                return Redirect("/Index");
            }
        }
    }
}