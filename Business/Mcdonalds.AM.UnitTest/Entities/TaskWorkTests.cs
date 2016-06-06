using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class TaskWorkTests
    {
        [TestMethod()]
        public void GetMyTaskWorkTest()
        {
            //var taskWork = new TaskWork();
            //var result = taskWork.GetMyTaskWork();
            //Assert.IsFalse(result.Any(e => e.RefID == "MajorLease14090202"));
            //var projectInfoRepos = new BaseEntity<ProjectInfo>();
            //var flowCodeList = new List<string>()
            //{
            //    FlowCode.MajorLease_ConsInfo,
            //    FlowCode.MajorLease_FinanceAnalysis,
            //    FlowCode.MajorLease_LegalReview
            //};

            var task = TaskWork.Search(c => c.ProcInstID == 2736).OrderByDescending(o => o.Num).FirstOrDefault();


            // 更新Task 状态
            task.Status = ConvertHelper.ToEnumType<TaskWorkStatus>(100);
            int _result = TaskWork.Update(task);


        }
    }
}
