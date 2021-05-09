using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using dotNetMVCLeagueApp.Data.Models.User;
using Microsoft.AspNetCore.Identity;

namespace dotNetMVCLeagueApp.Areas.Identity.Data {
    public class ApplicationUser : IdentityUser { // Pro primarni klic bude pouzito GUID
        
        /// <summary>
        /// Jmeno uzivatele
        /// </summary>
        [PersonalData]
        public string Name { get; set; }
        
        /// <summary>
        /// Preferovana linka - zobrazi se na strance pro vsechny ostatni
        /// </summary>
        public string PreferredLane { get; set; }
        
        /// <summary>
        /// Reference na uzivateluv ucet
        /// </summary>
        public string EncryptedSummonerId { get; set; }
        
        public virtual IEnumerable<ProfileCardModel> ProfileCards { get; set; }
    }
}