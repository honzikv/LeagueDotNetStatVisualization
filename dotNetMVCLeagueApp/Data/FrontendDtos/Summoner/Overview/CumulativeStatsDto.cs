using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.JsonMappings;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner.Overview {
    /// <summary>
    /// Obsahuje kumulativni statistiky pro daneho hrace - tuto tridu nasledne rozsiruje
    /// ChampionCumulativeStats, ktery obsahuje informace specificke pro danou postavu
    /// </summary>
    public class CumulativeStatsDto {
        
        public string PrimaryRole { get; set; }
        
        public double PrimaryRoleFrequency { get; set; }
        
        public string SecondaryRole { get; set; }
        
        public double SecondaryRoleFrequency { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public double AverageKills { get; set; }
        

        public double AverageDeaths { get; set; }

        public double AverageAssists { get; set; }

        public double Winrate { get; set; }

        public double AverageGoldDiffAt10 { get; set; }

        public double AverageKda { get; set; }

        public double AverageKillParticipation { get; set; }

        public double AverageGoldShare { get; set; }

        public double AverageDamageShare { get; set; }
    }

    public class ChampionCumulativeStatsDto : CumulativeStatsDto {
        public ChampionAsset ChampionAsset { get; set; }
    }

    public class StatsCounter {
        
        public int Games { get; set; }
        public List<int> Kills { get; } = new();

        public List<int> Deaths { get; } = new();

        public List<int> Assists { get; } = new();

        public List<double> Kdas { get; } = new();

        public List<int> Cs { get; } = new();

        public int Wins { get; set; }

        public int Losses { get; set; }

        public List<double> KillParticipations { get; } = new();

        public List<double> TeamGoldShares { get; } = new();

        public List<double> CsPerMinute { get; } = new();

        public List<double> DamageShares { get; } = new();

        public Dictionary<string, int> Roles { get; } = new();
        
        public List<double> GoldDiffsAt10 { get; } = new();
    }
}