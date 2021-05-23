using System;
using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.User;

namespace dotNetMVCLeagueApp.Pages.Data.Profile {
    
    /// <summary>
    /// Obsahuje data o profilu uzivatele
    /// </summary>
    public class SummonerProfileDto {
        
        /// <summary>
        /// Jmeno
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Region (server) ve kterem se uzivatel nachazi
        /// </summary>
        public string Region { get; init; }
        
        /// <summary>
        /// Uroven uzivatele
        /// </summary>
        public long SummonerLevel { get; init; }
        
        /// <summary>
        /// QueueInfo pro Ranked Solo
        /// </summary>
        public QueueInfoDto SoloQueue { get; set; }
        
        /// <summary>
        /// QueueInfo pro Ranked Flex
        /// </summary>
        public QueueInfoDto FlexQueue { get; set; }

        /// <summary>
        /// Cesta k profilove ikonce
        /// </summary>
        public string ProfileIconRelativeAssetPath { get; set; }
        
        
    }
}