using System.Collections.Generic;
using dotNetMVCLeagueApp.Const;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Trida pro ulozeni celkovych poctu pri GetGameListStatsViewModel
    /// Pri iteraci se do kazdeho seznamu ukladaji data, ze kterych se pak vypocitaji statistiky
    /// </summary>
    public class GameListStats {

        /// <summary>
        /// Pocet zabiti
        /// </summary>
        public List<int> Kills { get; } = new();

        /// <summary>
        /// Pocet smrti
        /// </summary>
        public List<int> Deaths { get; } = new();

        /// <summary>
        /// Pocet asistenci
        /// </summary>
        public List<int> Assists { get; } = new();
       
        /// <summary>
        /// Participace na zabitich pro kazdou hru
        /// </summary>
        public List<double> KillParticipations { get; } = new();

        /// <summary>
        /// Roli sledujeme pomoci slovniku
        /// </summary>
        public Dictionary<string, int> Roles { get; } = new() {
            {ServerConstants.Top, 0},
            {ServerConstants.Mid, 0},
            {ServerConstants.Adc, 0},
            {ServerConstants.Sup, 0},
            {ServerConstants.Jg, 0}
        };

        public Dictionary<int, int> Champions { get; } = new();

        /// <summary>
        /// Rozdil zlata v 10 minute
        /// </summary>
        public List<double> GoldDiffsAt10 { get; } = new();

        /// <summary>
        /// Zlato na konci hry
        /// </summary>
        public List<int> Gold { get; } = new();

        /// <summary>
        /// CS za minutu pro  kazdou hru
        /// </summary>
        public List<double> CsPerMinuteList { get; } = new();
    };
}