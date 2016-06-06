using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class MajorLeaseConsInfoTests
    {
        [TestMethod()]
        public void FinishTest()
        {
            var taskWork = TaskWork.FirstOrDefault(e => e.Id == new Guid("6bae9906-8608-49a2-8faa-0bbd5eaa43ee"));

            var wfEntity = BaseWFEntity.GetWorkflowEntity("MajorLease15030405", FlowCode.MajorLease_ConsInfo);
            wfEntity.Finish(TaskWorkStatus.K2ProcessApproved, taskWork);
        }
    }
}
