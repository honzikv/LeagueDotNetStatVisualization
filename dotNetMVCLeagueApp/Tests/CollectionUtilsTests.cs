using System.Collections.Generic;
using dotNetMVCLeagueApp.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    public class CollectionUtilsTests {

        [Fact]
        public void TestMinWithIndex_IntList_Sorted() {
            var list = new List<int> {1, 2, 3, 4};

            var expectedValue = 1;
            var expectedIdx = 0;

            var (value, idx) = list.MinWithIndex();
            Assert.Equal(value, expectedValue);
            Assert.Equal(idx, expectedIdx);
        }
        
        [Fact]
        public void TestMinWithIndex_IntList_NotSorted() {
            var list = new List<int> {3, 3, 2, 1, 4, 12, 1};

            var expectedValue = 1;
            var expectedIdx = 6;

            var (value, idx) = list.MinWithIndex();
            Assert.Equal(value, expectedValue);
            Assert.Equal(idx, expectedIdx);
        }

        [Fact]
        public void TestMaxWithIndex_IntList_Sorted() {
            var list = new List<int> {1, 2, 3, 4};
            var expectedValue = 4;
            var expectedIdx = 3;
            
            var (value, idx) = list.MaxWithIndex();
            Assert.Equal(value, expectedValue);
            Assert.Equal(idx, expectedIdx);
        }

        [Fact]
        public void TestMaxWithIndex_IntList_NotSorted() {
            var list = new List<int> {3, 3, 2, 1, 4, 12,  1};

            var expectedValue = 12;
            var expectedIdx = 5;

            var (value, idx) = list.MaxWithIndex();
            Assert.Equal(value, expectedValue);
            Assert.Equal(idx, expectedIdx);
        }
    }
}