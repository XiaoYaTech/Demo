using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mcdonalds.AM.DataAccess.Constants;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class ReimageConsInfoTests
    {
        [TestMethod()]
        public void FinishTest()
        {
            //var reimagePackage = new ReimagePackage();
            //ProjectInfo.Update("Reimage14110303", FlowCode.Reimage_ConsInfo, NodeCode.Finish,ProjectStatus.Finished);
            //reimagePackage.GeneratePackageTask("Reimage14110303");
        }

        [TestMethod()]
        public void FinishTest1()
        {
            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == "Reimage15012901");
            if (reimageInfo != null)
            {
                reimageInfo.GenerateTaskWork(
                FlowCode.Reimage_Summary,
                FlowCode.Reimage_Summary,
                FlowCode.Reimage_Summary,
                string.Format(@"/Reimage/Main#/Summary?projectId={0}", "Reimage15012901"));
            }
        }




    }
}
