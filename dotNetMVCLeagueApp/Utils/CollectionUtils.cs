using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Utils {
    /// <summary>
    /// Trida pro ziskani minima / maxima s indexem prvku - funkcni pro genericke typy, ktere implementuji
    /// IComparable
    /// </summary>
    public static class CollectionUtils {
        
        /// <summary>
        /// Ziska minimum ze seznamu
        /// </summary>
        /// <param name="list">seznam, ktery prohledavame</param>
        /// <typeparam name="T">typ objektu</typeparam>
        /// <returns>referenci na objekt a index, kde se objekt nachazi</returns>
        public static (T, int) MinWithIndex<T>([NotNull] this List<T> list) where T : IComparable {
            var min = list[0];
            var maxIdx = 0;

            for (var i = 1; i < list.Count; i += 1) {
                if (list[i].CompareTo(min) <= 0) {
                    min = list[i];
                    maxIdx = i;
                }
            }

            return (min, maxIdx);
        }

        /// <summary>
        /// Ziska maximum a jeho index pro dany seznam
        /// </summary>
        /// <param name="list">seznam, ktery prohledavame</param>
        /// <typeparam name="T">typ objektu</typeparam>
        /// <returns>maximum a index, kde se prvek nachazi</returns>
        public static (T, int) MaxWithIndex<T>([NotNull] this List<T> list) where T : IComparable {
            var max = list[0];
            var maxIdx = 0;

            for (var i = 1; i < list.Count; i += 1) {
                if (list[i].CompareTo(max) > 0) {
                    max = list[i];
                    maxIdx = i;
                }
            }

            return (max, maxIdx);
        }
    }
}