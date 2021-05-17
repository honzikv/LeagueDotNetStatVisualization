﻿using System;

namespace dotNetMVCLeagueApp.Utils {
    /// <summary>
    /// Extension metody pro kolekce
    /// </summary>
    public static class CollectionExtensions {
        public static T[] SubArray<T>(this T[] data, int index, int length) {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}