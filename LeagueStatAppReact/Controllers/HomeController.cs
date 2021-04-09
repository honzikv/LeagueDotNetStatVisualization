using System.Diagnostics;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models;
using LeagueStatAppReact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> logger;
        private readonly SummonerInfoService summonerInfoService;

        public HomeController(ILogger<HomeController> logger, SummonerInfoService summonerInfoService) {
            this.logger = logger;
            this.summonerInfoService = summonerInfoService;
        }

    }
}