using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotNetMVCLeagueApp.Models;
using dotNetMVCLeagueApp.Services;

namespace dotNetMVCLeagueApp.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> logger;
        private readonly SummonerInfoService summonerInfoService;

        public HomeController(ILogger<HomeController> logger, SummonerInfoService summonerInfoService) {
            this.logger = logger;
            this.summonerInfoService = summonerInfoService;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}