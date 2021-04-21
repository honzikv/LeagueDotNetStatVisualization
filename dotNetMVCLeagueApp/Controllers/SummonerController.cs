using System;
using Microsoft.AspNetCore.Mvc;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerController : Controller {
        // GET
        public IActionResult Index() {
            return View();
        }

        public IActionResult SummonerInfo(string name, string server) {
            throw new NotImplementedException();
        }
    }
}