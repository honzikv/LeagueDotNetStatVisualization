using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Home {
    public class HomePageDto {
        public Dictionary<string, string> ServerList { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}