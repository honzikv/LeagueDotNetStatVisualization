using Microsoft.AspNetCore.Mvc;

namespace dotNetMVCLeagueApp.Controllers {
    public class MatchDetailController : Controller {
        // GET
        public IActionResult Index() {
            return View();
        }
    }
}