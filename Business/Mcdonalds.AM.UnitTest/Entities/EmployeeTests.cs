using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class EmployeeTests
    {
        [TestMethod()]
        public void GetFianceManagersTest()
        {
            
        }

        [Test]
        public void GetEmployeee()
        {
            Employee bllEmp = new Employee();
            TaskWork bllTask = new TaskWork();

            string receiverID = "E5001019";
            string taskURL = @"/TempClosure/Main#/LegalReview/Process/param?SN=843_23&ProcInstID=843";
            string creater = "bpm.service";
            int _intProcInstID = 843;
            string k2SN = "843_23";
            //var _empReceiver = bllEmp.Search(o => o.Code.Contains(receiverID) || o.Account.Contains(receiverID) ).FirstOrDefault();

            //Console.WriteLine("Emp:{0}", JsonConvert.SerializeObject(_empReceiver) );



            //Guid _taskGUID = new Guid("04db8d10-59be-4a5d-8a49-571cfbaa1f7f");
            //var _taskWork = bllTask.Search(c => c.Id == _taskGUID).AsNoTracking().FirstOrDefault();

            //// 新建流程任务数据
            //var _empReceiver = bllEmp.Search(o => o.Code.Contains(receiverID) || o.Account.Contains(receiverID)).FirstOrDefault();

            //_taskWork.Id = Guid.NewGuid();
            //_taskWork.Url = taskURL;
            //_taskWork.Status = TaskWorkStatus.UnFinish; //0 - 收到任务的初始状态  2- 执行过的状态
            //_taskWork.CreateTime = DateTime.Now;
            //_taskWork.FinishTime = null;
            //_taskWork.CreateUserAccount = creater;// K2流程后台管理员帐号
            //_taskWork.ProcInstID = _intProcInstID;
            //_taskWork.K2SN = k2SN;
            //Console.WriteLine("Task:{0}", _taskWork.ReceiverAccount);  
            //if (_empReceiver != null && !string.IsNullOrEmpty(_empReceiver.Code))
            //{
            //    _taskWork.ReceiverAccount = _empReceiver.Code;
            //    _taskWork.ReceiverNameZHCN = _empReceiver.NameZHCN;
            //    _taskWork.ReceiverNameENUS = _empReceiver.NameENUS;
            //}

            //Console.WriteLine("Task:{0}", _taskWork.ReceiverAccount);
            //Console.WriteLine("Emp:{0}", JsonConvert.SerializeObject(_empReceiver));
        }
    }
}
