using System;
using System.Collections.Generic;
using System.Diagnostics;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models;
using dotNetMVCLeagueApp.Services.Summoner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotNetMVCLeagueApp.Pages {
    public class Home : PageModel {

        public readonly Dictionary<string, string> ServerList = ServerConstants.QueryableServers;

        [TempData] public string ErrorMessage { get; set; }

    }
}