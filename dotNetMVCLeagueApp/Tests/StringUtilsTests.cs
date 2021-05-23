using System;
using dotNetMVCLeagueApp.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    public class StringUtilsTests {

        /// <summary>
        /// Test GetGameDuration pro 30 minut
        /// </summary>
        [Fact]
        public void TestGetGameDuration_30Mins() {
            var timeSpan = TimeSpan.FromMinutes(30);

            var expected = "30:00";
            var actual = StringUtils.GetGameDuration(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetGameDuration pro 72 minut
        /// </summary>
        [Fact]
        public void TestGetGameDuration_72Minutes() {
            var timeSpan = TimeSpan.FromMinutes(72);

            var expected = "1:12:00";
            var actual = StringUtils.GetGameDuration(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetGameDuration pro 5 minut
        /// </summary>
        [Fact]
        public void TestGetGameDuration_5Minutes() {
            var timeSpan = TimeSpan.FromMinutes(5);

            var expected = "5:00";
            var actual = StringUtils.GetGameDuration(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetGameDuration pro dobu, ktera trvala 5 minut a 30 sekund
        /// </summary>
        [Fact]
        public void TestGetGameDuration_5Minutes_30Seconds() {
            var timeSpan = new TimeSpan(hours: 0, minutes: 5, seconds: 30);

            var expected = "5:30";
            var actual = StringUtils.GetGameDuration(timeSpan);
            
            Assert.Equal(actual, expected);
        }

        /// <summary>
        /// Test GetPlayTime pro 1 den zpet
        /// </summary>
        [Fact]
        public void TestGetPlayTime_1DayAgo() {
            var timeSpan = TimeSpan.FromDays(1);

            var expected = "1 Day(s) Ago";
            var actual = StringUtils.GetPlayTime(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test vytvoreni stringu pro hru, ktera se odehrala pred 10 minutami
        /// </summary>
        [Fact]
        public void TestGetPlayTime_10MinsAgo() {
            var timeSpan = TimeSpan.FromMinutes(10);

            var expected = "10 Mins ago";
            var actual = StringUtils.GetPlayTime(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test vytvoreni stringu pro hru, ktera se odehrala pred 122 minutami
        /// </summary>
        [Fact]
        public void TestGetPlayTime_122MinsAgo() {
            var timeSpan = TimeSpan.FromMinutes(122);

            var expected = "2 Hr(s) Ago";
            var actual = StringUtils.GetPlayTime(timeSpan);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetFrameIntervalToSeconds pro 60-sekundovou delku framu (vychozi z API) a 22. snimek
        /// </summary>
        [Fact]
        public void TestGetFrameIntervalToSeconds_DefaultApi_22Frames() {
            var frameIntervalSeconds = TimeSpan.FromSeconds(60);
            var framesPassed = 22;

            var expected = "22:00";
            var actual = StringUtils.FrameIntervalToSeconds(framesPassed, frameIntervalSeconds);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetFrameIntervalToSeconds pro "dlouhou hru" pri normalnich hodnotach API
        /// </summary>
        [Fact]
        public void TestGetFrameIntervalToSeconds_DefaultApi_120Frames() {
            var frameIntervalSeconds = TimeSpan.FromSeconds(60);
            var framesPassed = 120;

            var expected = "2:00:00";
            var actual = StringUtils.FrameIntervalToSeconds(framesPassed, frameIntervalSeconds);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test GetFrameIntervalToSeconds kdy frame trva 10 sekund a probehlo 11 framu
        /// </summary>
        [Fact]
        public void TestGetFrameIntervalToSeconds_10SecondFrame_11Frames() {
            var frameIntervalSeconds = TimeSpan.FromSeconds(10);
            var framesPassed = 11;

            var expected = "01:50";
            var actual = StringUtils.FrameIntervalToSeconds(framesPassed, frameIntervalSeconds);
            
            Assert.Equal(expected, actual);
        }
        

        /// <summary>
        /// Test TruncateIfNecessary pro zkraceni stringu kdy string by mel zustat nezmeneny
        /// </summary>
        [Fact]
        public void TestTruncateIfNecessary_RemainUnchanged() {
            var text = "too short";
            var chars = 20;

            var expected = "too short";
            var actual = StringUtils.TruncateIfNecessary(text, chars);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test TruncateIfNecessary pro stav, kdy by se mel retezec zkratit
        /// </summary>
        [Fact]
        public void TestTruncateIfNecessary_Shorten() {
            var text = "too long too long too long";
            var chars = 8;

            var expected = "too long...";
            var actual = StringUtils.TruncateIfNecessary(text, chars);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Trida pro test null property
        /// </summary>
        class TestClass {
            public string String { get; set; }
        }

        /// <summary>
        /// Test pro null property v objektu
        /// </summary>
        [Fact]
        public void TestTruncateIfNecessary_NullProperty() {
            var testClass = new TestClass() {
                String = null
            };
            
            var expected = "";
            var actual = StringUtils.TruncateIfNecessary(testClass.String, 10);
            
            Assert.Equal(expected, actual);
        }
    }
}