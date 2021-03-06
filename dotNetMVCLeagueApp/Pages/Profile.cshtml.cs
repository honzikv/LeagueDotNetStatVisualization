using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using dotNetMVCLeagueApp.Pages.Data.Profile;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.Util;

namespace dotNetMVCLeagueApp.Pages {
    public class Profile : PageModel {
        private readonly SummonerService summonerService;
        private readonly ProfileCardService profileCardService;
        private readonly SummonerProfileStatsService summonerProfileStatsService;
        private readonly MatchService matchService;
        private readonly ILogger<Profile> logger;

        public Profile(SummonerService summonerService,
            SummonerProfileStatsService summonerProfileStatsService,
            ILogger<Profile> logger,
            MatchService matchService,
            ProfileCardService profileCardService) {
            this.summonerService = summonerService;
            this.summonerProfileStatsService = summonerProfileStatsService;
            this.logger = logger;
            this.matchService = matchService;
            this.profileCardService = profileCardService;
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

        /// <summary>
        /// Parametry z GET requestu
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public ProfileQueryDto QueryParams { get; set; }

        /// <summary>
        /// Chybova zprava - mapuje se z TempData
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Data daneho summonera
        /// </summary>
        public SummonerOverviewDto SummonerData { get; set; }

        /// <summary>
        /// Pocet karet na profilu
        /// </summary>
        public const int CardsInProfile = 3;

        public List<ProfileCardModel> ProfileCards { get; set; } = new();

        /// <summary>
        /// Zkontroluje a pripadne upravi query parametry, abychom nemuseli uzivatele presmerovavat
        /// na Index. Tyto parametry jsou pouze pro p
        /// </summary>
        private void CheckQueryParams() {
            // Filtr nastavime bud jako vsechny hry, nebo specificky pokud je spravne
            QueryParams.Filter = QueueFilters.ContainsKey(QueryParams.Filter)
                ? QueryParams.Filter
                : ServerConstants.AllGamesDbValue;

            // Pocet her je bud vychozich 10, nebo specificky pokud je spravne
            QueryParams.PageSize = NumberOfGames.Contains(QueryParams.PageSize)
                ? QueryParams.PageSize
                : ServerConstants.DefaultPageSize;
        }

        /// <summary>
        /// GET pro ziskani stranky
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync() {
            logger.LogDebug("Profile.cshtml -> OnGet");
            logger.LogDebug($"{QueryParams}");
            if (QueryParams is null || !ModelState.IsValid || !Servers.ContainsKey(QueryParams.Server.ToLower())) {
                TempData["ErrorMessage"] = "Invalid search parameters";
                return Redirect("/Index");
            }

            CheckQueryParams();

            // Region, pro ktery budeme hledat
            var server = Region.Get(QueryParams.Server.ToUpper());

            try {
                var summoner = await summonerService.GetSummoner(QueryParams.Name, server);
                var profileCardsTask = profileCardService.GetProfileCardsForSummonerByPosition(summoner);
                List<MatchModel> matchHistory;
                var filterDbValue = QueueFilters[QueryParams.Filter];
                if (filterDbValue == ServerConstants.AllGamesDbValue
                    && QueryParams.PageSize == ServerConstants.DefaultPageSize
                    && QueryParams.Offset == 0) {
                    matchHistory = matchService.GetFrontPage(summoner);
                }
                else {
                    var queues = filterDbValue == ServerConstants.AllGamesDbValue
                        ? ServerConstants.RelevantQueues
                        : new[] {ServerConstants.GetQueueId(filterDbValue)};
                    matchHistory = await matchService.GetMatchHistory(summoner, QueryParams.Offset,
                        QueryParams.PageSize, queues);
                }

                var profileCards = await profileCardsTask;

                if (profileCards.Count > CardsInProfile) {
                    profileCards = profileCards.GetRange(0, CardsInProfile);
                }

                ProfileCards = profileCards;
                SummonerData = GetSummonerData(summoner, matchHistory);
                return Page();
            }
            catch (Exception ex) {
                switch (ex) {
                    case RedirectToHomePageException:
                        TempData["ErrorMessage"] = ex.Message;
                        return Redirect("/Index");
                    case RiotResponseException:
                        TempData["ErrorMessage"] = "There was an error while communicating with the Riot servers.";
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
            return new SummonerOverviewDto(summonerProfileDto, matchListOverview,
                matchHeaders);
        }

        /// <summary>
        /// POST pro refresh profilu
        /// </summary>
        /// <returns></returns>
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
                await matchService.GetUpdatedMatchHistory(summoner, ServerConstants.DefaultPageSize);
                return RedirectToPage("", new ProfileQueryDto {
                    Name = QueryParams.Name,
                    Server = QueryParams.Server
                });
            }

            catch (Exception ex) {
                logger.LogDebug(ex.Message);
                switch (ex) {
                    case RedirectToHomePageException:
                        TempData["ErrorMessage"] = ex.Message;
                        return Redirect("/Index");
                    case ActionNotSuccessfulException:
                        ErrorMessage = ex.Message;
                        return RedirectToPage("", new ProfileQueryDto {
                            Name = QueryParams.Name,
                            Server = QueryParams.Server
                        });
                    case RiotResponseException:
                        ErrorMessage =
                            "There was an error while communicating with Riot servers. Profile could not be updated.";
                        return RedirectToPage("", new ProfileQueryDto {
                            Name = QueryParams.Name,
                            Server = QueryParams.Server
                        });
                    default:
                        ErrorMessage = "Error while refreshing user profile";
                        return Page();
                }
            }
        }
    }
}