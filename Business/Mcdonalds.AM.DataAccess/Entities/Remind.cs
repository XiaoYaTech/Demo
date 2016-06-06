using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.DataModels.Condition;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;

namespace Mcdonalds.AM.DataAccess
{
    public class RemindUserInfo
    {
        public string UserAccount
        {
            get;
            set;
        }

        public string UserNameENUS { get; set; }
        public string UserNameZHCN { get; set; }
    }
    public partial class Remind : BaseEntity<Remind>
    {
        public static void SendRemind(string strProjectId, string strFlowCode, List<ProjectUsers> remindUsers)
        {
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.FlowCode == strFlowCode
                                                              && e.ProjectId == strProjectId);
            if (projectInfo != null)
            {
                var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == projectInfo.USCode);

                var employee = Employee.GetEmployeeByCode(projectInfo.CreateUserAccount);

                if (store != null
                    && employee != null)
                {
                    var remind = new Remind();
                    remind.SenderAccount = employee.Code;
                    remind.SenderNameENUS = employee.NameENUS;
                    remind.SenderNameZHCN = employee.NameZHCN;
                    remind.Title = string.Format("【{0} {1} ({2})】{3}流程已创建", projectInfo.USCode, store.NameZHCN, store.NameENUS, strFlowCode);
                    remind.Url = string.Format("/Home/Main#/project/detail/{0}?flowCode={1}", strProjectId, strFlowCode);
                    remind.RegisterCode = strFlowCode;
                    remind.IsReaded = false;
                    PostRemaindList(remind, remindUsers);
                }

            }

        }

        public static void PostRemaindList(Remind remind, List<ProjectUsers> remindUsers)
        {
            var objectCopy = new ObjectCopy();
            List<Remind> listRemind = new List<Remind>();
            try
            {
                foreach (var users in remindUsers)
                {
                    var entity = new Remind();
                    entity = objectCopy.AutoCopy(remind);
                    entity.Id = Guid.NewGuid();
                    entity.ReceiverNameENUS = users.UserNameENUS;
                    entity.ReceiverNameZHCN = users.UserNameZHCN;
                    entity.ReceiverAccount = users.UserAccount;
                    entity.CreateTime = DateTime.Now;
                    listRemind.Add(entity);
                }
                Add(listRemind.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PostRemind(Remind remind)
        {
            try
            {
                if (remind.Id == new Guid())
                {
                    remind.Id = Guid.NewGuid();
                    remind.CreateTime = DateTime.Now;
                    Add(remind);
                }
                else
                    Update(remind);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IQueryable<Remind> Query(RemindCondition searchCondition, out int totalSize)
        {
            var predicate = PredicateBuilder.True<Remind>();

            if (!string.IsNullOrEmpty(searchCondition.Title))
            {
                predicate = predicate.And(e => e.Title.Contains(searchCondition.Title));
            }

            if (!string.IsNullOrEmpty(searchCondition.SenderZHCN))
            {
                //var employeeList = Employee.Search(e => e.NameZHCN.Contains(searchCondition.SenderZHCN));
                //var employeeCodeList = employeeList.Select(e => e.Code).ToList();

                predicate = predicate.And(e => e.SenderNameZHCN.Contains(searchCondition.SenderNameZHCN));
            }

            if (!string.IsNullOrEmpty(searchCondition.SenderNameENUS))
            {
                //var employeeList = Employee.Search(e => e.NameZHCN.Contains(searchCondition.SenderZHCN));
                //var employeeCodeList = employeeList.Select(e => e.Code).ToList();

                predicate = predicate.And(e => e.SenderNameENUS.Contains(searchCondition.SenderNameENUS));
            }

            if (!string.IsNullOrEmpty(searchCondition.ReceiverAccount))
            {
                predicate = predicate.And(e => e.ReceiverAccount == searchCondition.ReceiverAccount);
            }

            //从多少页开始取数据
            var remindList = Search(predicate, e => e.CreateTime, searchCondition.PageIndex, searchCondition.PageSize,
                out totalSize, true);

            return remindList;
        }

        public static void Send(Remind remindInfo, List<SimpleEmployee> receiverUserList, string projectId, string flowCode)
        {
            var objectCopy = new ObjectCopy();
            using (var scope = new TransactionScope())
            {
                foreach (var receiver in receiverUserList)
                {
                    var remind = objectCopy.AutoCopy(remindInfo);
                    remind.Id = Guid.NewGuid();
                    remind.IsReaded = false;
                    remind.ReceiverAccount = receiver.Code;
                    remind.ReceiverNameENUS = receiver.NameENUS;
                    remind.ReceiverNameZHCN = receiver.NameZHCN;
                    remind.CreateTime = DateTime.Now;
                    remind.Url = string.Format("/Home/Main#/project/detail/{0}?flowCode={1}", projectId, flowCode);
                    remind.Add();
                }
                scope.Complete();
            }
        }
    }
}
