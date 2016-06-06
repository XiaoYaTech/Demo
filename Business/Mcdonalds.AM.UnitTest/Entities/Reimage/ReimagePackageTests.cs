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
    public class ReimagePackageTests
    {
        [TestMethod()]
        public void FinishTest()
        {
          //  var reimageConsInvtChecking = new ReimageConsInvtChecking();
           
          //  ProjectInfo.Update("Reimage14102405", FlowCode.Reimage_ConsInfo, NodeCode.Finish, ProjectStatus.Finished);

          //  var reimagePackage = new ReimagePackage();
          //  //reimageConsInvtChecking.GenerateConsInvertTask("Reimage14102405");
          //  var listTask = new List<TaskWork>
          //              {
          //                  //GenerateTaskWork(FlowCode.Rebuild_ContractInfo),
          //                  //GenerateTaskWork(FlowCode.Rebuild_SiteInfo),
          //                 reimagePackage.GenerateTaskWork(FlowCode.Reimage_ReopenMemo),
          //                  reimagePackage.GenerateTaskWork(FlowCode.Reimage_GBMemo),
          //                  //GenerateTaskWork(FlowCode.Rebuild_TempClosureMemo)
          //              };
          //  TaskWork.Add(listTask.ToArray());
          //// reimagePackage.GenerateSiteInfoTask();
        }

        [TestMethod()]
        public void GeneratePackageTaskTest()
        {
            var wf = new ReimagePackage();
            wf.GeneratePackageTask("Reimage14120902");
        }

      
    }
}
