using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Utils {
    public static class RiotApiUtils {

        public static Region? GetRegionOrNull(string regionString) {
            try {
                return Region.Get(regionString);
            }
            catch {
                return null;
            }
        }
    }
}