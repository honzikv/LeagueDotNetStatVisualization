using System;
using dotNetMVCLeagueApp.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    public class DateTimeUtilsTests {

        [Fact]
        public void TestConvertFromUnixTimestamp_16July1717CEST() {
            var expected = DateTime.Parse("Friday, April 16, 2021 3:17:11.476 PM");
            var actual = DateTimeUtils.ConvertFromUnixTimestamp(1618586231476);
            
            Assert.Equal(expected, actual);
        }
    }
}