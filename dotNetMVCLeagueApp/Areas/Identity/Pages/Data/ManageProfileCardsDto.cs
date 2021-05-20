using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.User;

namespace dotNetMVCLeagueApp.Areas.Identity.Pages.Data {
    public class ManageProfileCardsDto {
        
        public List<ProfileCardModel> ProfileCards { get; set; }
        
        public string StatusMessage { get; set; }
    }
}