﻿using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Data.Models.Match.Timeline {
    /// <summary>
    /// 2D pozice na mape - int X a int Y 
    /// </summary>
    public class MapPositionModel {

        /// <summary>
        /// X-ova pozice na mape
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// Y-ova pozice na mape
        /// </summary>
        public int Y { get; set; }

        public MapPositionModel(MatchPosition position) {
            X = position.X;
            Y = position.Y;
        }
    }
}