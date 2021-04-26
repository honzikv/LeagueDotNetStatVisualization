using System;
using dotNetMVCLeagueApp.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    /// <summary>
    /// Obsahuje test pro tridu DateTimeUtils, ktera se pouziva pro konverzi casu
    /// </summary>
    public class DateTimeUtilsTests {

        [Fact]
        public void TestConvertFromMillisToDateTime_16April1717CEST() {
            var expected = DateTime.Parse("Friday, April 16, 2021 3:17:11.476 PM").ToLocalTime();
            var actual = TimeUtils.ConvertFromMillisToDateTime(1618586231476);
            
            Assert.Equal(expected, actual);
        }

    }
}