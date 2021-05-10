﻿using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;

namespace dotNetMVCLeagueApp.Areas.Identity.Data {
    public class ApplicationUser : IdentityUser { // Pro primarni klic bude pouzito GUID

        /// <summary>
        /// Preferovana linka - zobrazi se na strance pro vsechny ostatni
        /// </summary>
        public string PreferredLane { get; set; }

        /// <summary>
        /// Reference na uzivateluv ucet
        /// </summary>
        public virtual SummonerModel Summoner { get; set; }

        public virtual IEnumerable<ProfileCardModel> ProfileCards { get; set; }
    }
}