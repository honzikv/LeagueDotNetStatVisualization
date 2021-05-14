using System.Diagnostics;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.FrontendDtos.Home;
using dotNetMVCLeagueApp.Data.Models;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotNetMVCLeagueApp.Services;
using dotNetMVCLeagueApp.Services.Summoner;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    /// <summary>
    /// Controller pro domovskou stranku (aktual
    /// </summary>
    public class RootController : Controller {
        private readonly SummonerService summonerService;

        public RootController(ILogger<RootController> logger, SummonerService summonerService) {
            this.summonerService = summonerService;
        }


        public IActionResult Index() {
            return View(
                new HomePageDto {
                    ServerList = summonerService.GetQueryableServers,
                    ErrorMessage = (string) TempData["ErrorMessage"]
                });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}