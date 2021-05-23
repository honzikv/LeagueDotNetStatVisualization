using System;
using System.Collections.Generic;
using System.Diagnostics;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Pages {
    
    /// <summary>
    /// Objekt pro obsluhu domovske stranky - obsahuje seznam serveru a zobrazeni chybove zpravy pri presmerovani
    /// </summary>
    public class Home : PageModel {

        /// <summary>
        /// Seznam serveru
        /// </summary>
        public readonly Dictionary<string, string> ServerList = ServerConstants.QueryableServers;

        /// <summary>
        /// Chybova zprava, ktera se zobrazi pri presmerovani
        /// </summary>
        [TempData] public string ErrorMessage { get; set; }

    }
}