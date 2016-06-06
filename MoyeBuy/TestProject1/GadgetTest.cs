using MoyeBuy.Com.MoyeBuyUtility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for GadgetTest and is intended
    ///to contain all GadgetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GadgetTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Split
        ///</summary>
        public void SplitTestHelper<T>()
        {
            string strValue = "1|2|3|4"; // TODO: Initialize to an appropriate value
            string strDelimiter ="|"; // TODO: Initialize to an appropriate value
            IList<int> test = new List<int>();
            test.Add(1);
            test.Add(2);
            test.Add(3);
            test.Add(4);
            IList<int> expected = test; // TODO: Initialize to an appropriate value
            IList<int> actual;
            actual = Gadget.Split<int>(strValue, strDelimiter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void SplitTest()
        {
            SplitTestHelper<GenericParameterHelper>();
        }
    }
}
