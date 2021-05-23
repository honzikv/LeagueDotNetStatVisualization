using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Pages.Data.Profile.Overview {
    /// <summary>
    /// Obsahuje kumulativni statistiky pro daneho hrace - tuto tridu nasledne rozsiruje
    /// ChampionCumulativeStats, ktery obsahuje informace specificke pro danou postavu
    /// </summary>
    public class CumulativeStatsDto {
        
        /// <summary>
        /// Detekovana primarni role
        /// </summary>
        public string PrimaryRole { get; set; }

        /// <summary>
        /// % pocet her primarni role
        /// </summary>
        public double PrimaryRoleFrequency { get; set; }

        /// <summary>
        /// Detekovana druha nejhranejsi role
        /// </summary>
        public string SecondaryRole { get; set; }

        /// <summary>
        /// Cetnost her v druhe nejhranejsi roli
        /// </summary>
        public double SecondaryRoleFrequency { get; set; }

        /// <summary>
        /// Pocet vyher
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Pocet proher
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Prumerny pocet zabiti
        /// </summary>
        public double AverageKills { get; set; }

        /// <summary>
        /// Prumerny pocet smrti
        /// </summary>
        public double AverageDeaths { get; set; }

        /// <summary>
        /// Prumerny pocet asistenci
        /// </summary>
        public double AverageAssists { get; set; }

        /// <summary>
        /// % vyher
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// Prumerny rozdil zlata v 10. minute
        /// </summary>
        public double AverageGoldDiffAt10 { get; set; }

        /// <summary>
        /// Prumerny rozdil zlata v 20. minute
        /// </summary>
        public double AverageGoldDiffAt20 { get; set; }

        /// <summary>
        /// Prumerny rozdil CS v 10. minute
        /// </summary>
        public double AverageCsDiffAt10 { get; set; }

        /// <summary>
        /// Prumerny rozdil CS v 20. minute
        /// </summary>
        public double AverageCsDiffAt20 { get; set; }

        /// <summary>
        /// Prumerne KDA
        /// </summary>
        public double AverageKda { get; set; }

        /// <summary>
        /// Prumerny podil na zabiti
        /// </summary>
        public double AverageKillParticipation { get; set; }

        /// <summary>
        /// Prumerny % podil na zlate v tymui
        /// </summary>
        public double AverageGoldShare { get; set; }

        /// <summary>
        /// Prumerny podil na poskozeni
        /// </summary>
        public double AverageDamageShare { get; set; }

        /// <summary>
        /// Prumerne CS
        /// </summary>
        public double AverageCs { get; set; }

        /// <summary>
        /// Prumerne CS za minutu
        /// </summary>
        public double AverageCsPerMinute { get; set; }

        /// <summary>
        /// Prumerny podil na vizi na mape
        /// </summary>
        public double AverageVisionShare { get; set; }
    }

    public class ChampionCumulativeStatsDto : CumulativeStatsDto {
        public ChampionAsset ChampionAsset { get; set; }
    }

    /// <summary>
    /// Pocitadlo statistik
    /// </summary>
    public class StatsCounter {
        /// <summary>
        /// Pocet her
        /// </summary>
        public int Games { get; set; }

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
        /// KDA z kazde hry
        /// </summary>
        public List<double> Kdas { get; } = new();

        /// <summary>
        /// CS z kazde hry
        /// </summary>
        public List<int> Cs { get; } = new();

        /// <summary>
        /// Vyhry
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Prohry
        /// </summary>
        public int Losses { get; set; }
        
        /// <summary>
        /// Ucast na zabitich za kazdou hru
        /// </summary>
        public List<double> KillParticipations { get; } = new();

        /// <summary>
        /// % cast zlata tymu za kazdou hru
        /// </summary>
        public List<double> TeamGoldShares { get; } = new();

        /// <summary>
        /// Cs za minutu z kazde hry
        /// </summary>
        public List<double> CsPerMinute { get; } = new();

        /// <summary>
        /// % cast poskozeni z kazde hry
        /// </summary>
        public List<double> DamageShares { get; } = new();

        /// <summary>
        /// Role a jejich cetnosti
        /// </summary>
        public Dictionary<string, int> Roles { get; } = new();

        /// <summary>
        /// Rozdil zlata v 10. minute
        /// </summary>
        public List<double> GoldDiffsAt10 { get; } = new();

        /// <summary>
        /// Rozdil zlata v 20. minute
        /// </summary>
        public List<double> GoldDiffsAt20 { get; } = new();

        /// <summary>
        /// Rozdil CS v 10. minute
        /// </summary>
        public List<double> CsDiffsAt10 { get; } = new();

        /// <summary>
        /// Rozdil CS v 20. minute
        /// </summary>
        public List<double> CsDiffsAt20 { get; } = new();

        /// <summary>
        /// % podil na vizi na mape
        /// </summary>
        public List<double> VisionShare { get; } = new();
    }
}