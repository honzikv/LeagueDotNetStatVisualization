using System.Diagnostics;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotNetMVCLeagueApp.Services;
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

        public IActionResult Index() => View(
            new HomePageViewModel {
                ServerList = summonerInfoService.GetQueryableServers
            }
        );

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}