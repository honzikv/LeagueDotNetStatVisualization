using System;

namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail {
    /// <summary>
    ///     Par cas a hodnota
    /// </summary>
    /// <typeparam name="T">Jakykoliv primitivni typ co dava smysl - double, float, int, long, short, byte ...</typeparam>
    public class TimeValue<T> where T : IComparable {
        /// <summary>
        /// Cas v sekundach
        /// </summary>
        public TimeSpan TimeSpan { get; }

        /// <summary>
        /// Hodnota, ktera implementuje IComparable
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Konstruktor pro vytvoreni objektu z dane IComparable hodnoty a daneho casoveho udaje
        /// </summary>
        /// <param name="value">Hodnota, trida implementuje IComparable</param>
        /// <param name="timeSpan">Casovy udaj pro danou hodnotu</param>
        public TimeValue(T value, TimeSpan timeSpan) {
            Value = value;
            TimeSpan = timeSpan;
        }
        
    }
}