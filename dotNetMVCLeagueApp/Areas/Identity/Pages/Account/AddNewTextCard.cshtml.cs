using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Areas.Identity.Pages.Data;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Account {
    public class AddNewCard : PageModel {
        
        private readonly UserManager<ApplicationUser> userManager;

        public AddNewCard(UserManager<ApplicationUser> userManager) {
            this.userManager = userManager;
        }

        [BindProperty] public AddNewProfileCardDto Input { get; set; }
        
        [TempData] public string StatusMessage { get; set; }
        
        /// <summary>
        /// Pocet karet, ktere uzivatel vytvoril
        /// </summary>
        public int UserProfileCards { get; set; }

        public int CardLimit = ServerConstants.CardLimit;

        public async Task<IActionResult> OnGetAsync() {
            var user = await userManager.GetUserAsync(User);
            if (user is null) {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}.'");
            }
            
            var profileCards = user?.ProfileCards.ToList() ?? new();
            UserProfileCards = profileCards.Count;

            if (UserProfileCards >= CardLimit) {
                StatusMessage = "Error, you cannot create more cards and must delete some first.";
            }

            return Page();
        }
    }
}