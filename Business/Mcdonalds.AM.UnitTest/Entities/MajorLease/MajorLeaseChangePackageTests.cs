using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTTMNC.BPM.Fx.K2.Services;

namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class MajorLeaseChangePackageTests
    {
        [TestMethod()]
        public void GetMajorPackageInfoTest()
        {

            var context = new K2FxContext();
            var activityName = context.GetCurrentActivityName("1007_25", "E5006154");
        }
    }
}
