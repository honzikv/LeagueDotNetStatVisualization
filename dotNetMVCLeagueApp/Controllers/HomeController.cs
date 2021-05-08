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
    public class HomeController : Controller {
        private readonly ILogger<HomeController> logger;
        private readonly SummonerInfoService summonerInfoService;

        public HomeController(ILogger<HomeController> logger, SummonerInfoService summonerInfoService) {
            this.logger = logger;
            this.summonerInfoService = summonerInfoService;
        }


        public IActionResult Index() {
            return View(
                new HomePageDto {
                    ServerList = summonerInfoService.GetQueryableServers,
                    ErrorMessage = (string) TempData["ErrorMessage"]
                });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}