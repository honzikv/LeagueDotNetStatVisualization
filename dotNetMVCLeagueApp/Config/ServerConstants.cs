using System;
using System.Collections.Generic;
using System.Linq;

namespace dotNetMVCLeagueApp.Config {
    /// <summary>
    /// Obsahuje konstanty pro web server
    /// </summary>
    public class ServerConstants {
        public const string Win = "Win";

        public const string Loss = "Fail";

        public const string DoubleKill = "Double Kill";

        public const string TripleKill = "Triple Kill";

        public const string QuadraKill = "Quadra Kill";

        public const string PentaKill = "Penta Kill";

        public static readonly TimeSpan
            GameDurationForRemake = TimeSpan.FromMinutes(4); // do 4 minut se bere jako remake

        public const string RoleAdc = "DUO_CARRY";
        public const string RoleSup = "DUO_SUPPORT";
        public const string LaneBot = "BOTTOM"; // jak pro support tak pro adc

        public const string RoleTop = "SOLO";
        public const string LaneTop = "TOP";

        public const string RoleJg = "NONE";
        public const string LaneJg = "JUNGLE";

        public const string RoleMid = "SOLO";
        public const string LaneMid = "MIDDLE";

        public const string Top = "TOP";
        public const string Mid = "MID";
        public const string Jg = "JG";
        public const string Adc = "ADC";
        public const string Sup = "SUP";

        public const string AllGames = "ALL_GAMES";
        public const string RankedFlex = "RANKED_FLEX_SR";
        public const string RankedSolo = "RANKED_SOLO_5x5";

        public const string AllGamesDbValue = "All Games";
        public const string RankedSoloDbValue = "Solo Ranked";
        public const string RankedFlexDbValue = "Flex pick";
        public const string AllGamesText = "All Games";
        public const string RankedSoloText = "Ranked Solo";
        public const string RankedFlexText = "Ranked Flex";


        /// <summary>
        /// Vychozi pocet her, ktery nacteme - at uz z API nebo z DB
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Slouzi k ziskani jmena queue z jeho id
        /// </summary>
        private static readonly Dictionary<int, string> QueueNames = new() {
            {430, "Blind pick"}, {400, "Draft pick"},
            {440, "Flex pick"}, {420, "Solo ranked"}
        };

        public static readonly Dictionary<string, string> QueueFilters = new() {
            {AllGames, AllGamesDbValue},
            {RankedSolo, RankedSoloDbValue},
            {RankedFlex, RankedFlexDbValue}
        };

        /// <summary>
        /// Id relevantnich hernich modu
        /// </summary>
        public static readonly int[] RelevantQueues = {400, 420, 430, 440};

        public static readonly Dictionary<string, int> QueueIdToQueueNames =
            QueueNames.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// Limit pro pocet her, ktere muzeme hledat
        /// </summary>
        public const int GamesLimit = 200;

        public const int BlueSideId = 100;
        public const int RedSideId = 200;
        public const string RedSide = "Red side";
        public const string BlueSide = "Blue side";

        /// <summary>
        /// Vrati nazev queue z jeho id, pokud je queueId nevalidni, vrati prazdny retezec
        /// </summary>
        /// <param name="queueId">QueueId ziskane z Match objektu</param>
        /// <returns></returns>
        public static string GetQueueNameFromQueueId(int queueId) =>
            !QueueNames.ContainsKey(queueId) ? string.Empty : QueueNames[queueId];

        public static readonly Dictionary<string, string> QueryableServers = new() {
            {"euw", "EUW"}, {"eune", "EUNE"}, {"na", "NA"}
        };

        public static int GetQueueId(string queueType) => QueueIdToQueueNames[queueType];

        /// <summary>
        /// Ziska queue id (cislo) z filtru
        /// </summary>
        /// <param name="filter">jmeno filtru v QueueFilters</param>
        /// <returns></returns>
        public static int GetQueueIdFromFilter(string filter) => GetQueueId(QueueFilters[filter]);
    };
}