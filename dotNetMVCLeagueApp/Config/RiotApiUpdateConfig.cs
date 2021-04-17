using System;

namespace dotNetMVCLeagueApp.Config {
    
    public record RiotApiUpdateConfig(
        TimeSpan MinUpdateTimeSpan
        );
}