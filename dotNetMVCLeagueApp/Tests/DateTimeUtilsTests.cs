using System;
using dotNetMVCLeagueApp.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    /// <summary>
    /// Obsahuje test pro tridu DateTimeUtils, ktera se pouziva pro konverzi casu
    /// </summary>
    public class DateTimeUtilsTests {

        /// <summary>
        /// Test prevedeni datumu z milisekund od epochy do C# DateTime
        /// </summary>
        [Fact]
        public void TestConvertFromMillisToDateTime_16April1717CEST() {
            var expected = DateTime.Parse("Friday, April 16, 2021 3:17:11.476 PM").ToLocalTime();
            var actual = TimeUtils.ConvertFromMillisToDateTime(1618586231476);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test prevodu z DateTime do milisekund od epochy
        /// </summary>
        [Fact]
        public void TestConvertDateTimeToMillis_15May2021221732GMT() {
            var expected = 1621117052000;
            var actual = TimeUtils.ConvertDateTimeToMillis(DateTime.Parse("Sat, 15 May 2021 22:17:32 GMT"));
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test extension funkce "odecteni tydne"
        /// </summary>
        [Fact]
        public void TestSubtractWeekDateTimeExtension() {
            var date = DateTime.Parse("Sat, 15 May 2021 22:17:32 GMT");
            var expected = DateTime.Parse("Sat, 8 May 2021 22:17:32 GMT");
            var actual = date.SubtractWeek();
            
            Assert.Equal(expected, actual);
        }
    }
}