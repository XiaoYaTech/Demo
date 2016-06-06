using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class ReimageSummaryTests
    {
        [TestMethod()]
        public void LoadFinancialPreanalysisInfoTest()
        {
            var summary = new ReimageSummary();
            //var financeInfo=summary.LoadFinancialPreanalysisInfo();
        }

        [TestMethod()]
        public void FinishTest()
        {
            var reimagePackage = new ReimagePackage();
            reimagePackage.GeneratePackageTask("Reimage14102305");
        }
    }
}
