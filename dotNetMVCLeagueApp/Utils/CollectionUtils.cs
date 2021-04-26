using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace dotNetMVCLeagueApp.Utils {
    public static class CollectionUtils {
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