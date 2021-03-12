using Microsoft.AspNetCore.Mvc;

namespace dotNetMVCLeagueApp.Controllers {
    public class SummonerInfoController : Controller {
        // GET
        public IActionResult Index() {
            return View();
        }
    }
}