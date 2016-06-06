using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.DataAccess.Entities.Tests
{
    [TestClass()]
    public class BaseWFEntityTests
    {
        [TestMethod()]
        public void PendingProjectTest()
        {
            var legalReview = new MajorLeaseLegalReview();
            legalReview.PendingProject("MajorLease14090202");
        }

        [TestMethod()]
        public void CheckIfFreezeProjectTest()
        {
            var legalReview = new MajorLeaseLegalReview();
            var isFreeze = legalReview.CheckIfFreezeProject("MajorLease14090202");
        }

        [TestMethod()]
        public void EditTest()
        {
            var task =
                JsonConvert.DeserializeObject<TaskWork>(
                    "{'TypeName':'Reimage_Package','SourceName':'Reimage','TaskType':2,'Id':'9372e8a1-d769-4844-a938-5bd153daec5e','Num':9194,'Title':'Reimage14112502 Changle Airport 长乐机场','Url':'/Reimage/Main#/Package/Process/Approval?SN=3401_33&ProcInstID=3401','SourceCode':'Reimage','RefID':'Reimage14112502','Status':100,'ReceiverAccount':'E5009955','CreateTime':'2014-11-25T16:34:24.103','CreateUserAccount':'bpm.service','FinishTime':null,'Sequence':null,'TypeCode':'Reimage_Package','ProcInstID':3401,'DoActionUser':null,'ActionName':null,'K2SN':'3401_33','StoreCode':'1410096','ReceiverNameZHCN':'聂淼','ReceiverNameENUS':'Karuna Nie','SourceNameENUS':'Reimage','SourceNameZHCN':'Reimage','TypeNameZHCN':'Reimage_Package','TypeNameENUS':'Reimage_Package','StatusNameZHCN':'任务','StatusNameENUS':'任务','RefTableId':null,'RefTableName':null,'ActivityName':'Market Manager'}");
            var wfEntity = BaseWFEntity.GetWorkflowEntity("Reimage14112502", "Reimage_Package");
            
            if (wfEntity != null)
            {
                wfEntity.Finish(TaskWorkStatus.K2ProcessDeclined, task);
            }
        }

        [TestMethod()]
        public void GetWorkflowEntityTest()
        {
            var wfEntity = BaseWFEntity.GetWorkflowEntity("MajorLease15032702", FlowCode.MajorLease_ConsInfo);
        }
    }
}
