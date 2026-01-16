using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalancedBSTTests
{
    [TestClass]
    public class TestGenerate
    {
        [TestMethod]
        public void TwoInDepth()
        {
            int[] inputArray = { 90, 70, 120, 125, 115, 95, 100 };

            int[] balancedArray = BalancedBST.GenerateBBSTArray(inputArray);
            Assert.AreEqual(7, balancedArray.Length);

            int[] expectedArray = { 100, 90, 120, 70, 95, 115, 125 };
            CollectionAssert.AreEqual(expectedArray, balancedArray);
        }

        [TestMethod]
        public void ThreeInDepth()
        {
            int[] inputArray = { 15, 1, 9, 12, 11, 4, 7, 3, 13, 6, 5, 8, 14, 10, 2 };

            int[] balancedArray = BalancedBST.GenerateBBSTArray(inputArray);
            Assert.AreEqual(15, balancedArray.Length);

            int[] expectedArray = { 8, 4, 12, 2, 6, 10, 14, 1, 3, 5, 7, 9, 11, 13, 15 };
            CollectionAssert.AreEqual(expectedArray, balancedArray);
        }

        [TestMethod]
        public void FourInDepth()
        {
            // 31 elements of random numbers from 50 to 100
            int[] inputArray =
                { 1, 100, -5, 23, 47, 12, 554, 45, -100, 5, 6, 123, 321, 111, 125, 665, 789, 18, 3, 4, 7, 8, 10, -123, 1000, 5000, 10000, -1000, -5000, -60000, -10000 };

            int[] balancedArray = BalancedBST.GenerateBBSTArray(inputArray);

            int[] expectedArray = new int[]
            {
                12, 1, 125, -1000, 6, 47, 789, -10000, -100, 4, 8, 23, 111, 554, 5000,
                -60000, -5000, -123, -5, 3, 5, 7, 10, 18, 45, 100, 123, 321, 665, 1000, 10000
            };

            Assert.AreEqual(31, balancedArray.Length);
            CollectionAssert.AreEqual(expectedArray, balancedArray);
        }
    }
}