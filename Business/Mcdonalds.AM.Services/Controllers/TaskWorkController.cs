using System.Data.Entity.Infrastructure;
using System.Web;
using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Entities;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.Services.Controllers
{
    public class TaskWorkController : ApiController
    {
        /// <summary>
        /// 分页获取待办工作数据
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页的数据量</param>
        /// <param name="parentCode">父节点编码</param>
        /// <returns></returns>
        [Route("api/TaskWork/{pageIndex}/{pageSize}/{userCode}")]
        public IHttpActionResult GetTaskWorks(int pageIndex, int pageSize, string userCode)
        {
            try
            {

                var queryString = HttpContext.Current.Request.QueryString;
                IQueryable<TaskWork> result = TaskWork.GetUseableTaskWork();

                if (queryString["Status"] != null)
                {
                    var status = (TaskWorkStatus)int.Parse(queryString["Status"]);
                    if (status == TaskWorkStatus.Finished)
                    {
                        result = result.Where(c => (c.Status == status
                                                    || c.Status == TaskWorkStatus.K2ProcessApproved ||
                                                    c.Status == TaskWorkStatus.K2ProcessDeclined)
                                                   && c.ReceiverAccount == userCode);
                    }
                    else
                    {
                        result = result.Where(c => (c.Status == status) && c.ReceiverAccount == userCode);
                        result = ClosureTool.FilterTaskWork(result);
                    }
                }
                var skipSize = pageSize * (pageIndex - 1);
                string title = queryString["Title"];
                if (!string.IsNullOrEmpty(title))
                {
                    result = result.Where(c => c.Title.Contains(title));
                }
                string sourceCode = queryString["SourceCode"];
                if (!string.IsNullOrEmpty(sourceCode))
                {
                    result = result.Where(c => c.SourceCode == sourceCode);
                }

                var storeCode = queryString["StoreCode"];
                if (!string.IsNullOrEmpty(storeCode))
                {
                    result = result.Where(e => e.StoreCode == storeCode);
                }

                var storeName = queryString["StoreName"];
                if (!string.IsNullOrEmpty(storeName))
                {
                    var storeList =
                        StoreBasicInfo.Search(e => e.NameENUS.Contains(storeName) || e.NameZHCN.Contains(storeName))
                            .Select(e => e.StoreCode).ToList();
                    if (storeList.Any())
                    {
                        result = result.Where(e => storeList.Contains(e.StoreCode));
                    }
                }
                int totalItems = result.Count();

                var list = new List<TaskWork>();
                if (queryString["Status"] != null && queryString["Status"] == "2")
                    list = result.OrderByDescending(c => c.FinishTime).Skip(skipSize)
                        .Take(pageSize).ToList();
                else
                    list = result.OrderByDescending(c => c.CreateTime).Skip(skipSize)
                        .Take(pageSize).ToList();

                foreach (var taskWork in list)
                {
                    var operators = TaskWork.GetOperators(taskWork.TypeCode, taskWork.RefID);
                    var taskWorkOperator = operators.FirstOrDefault(e => e.Code == ClientCookie.UserCode);
                    taskWork.OperateMsg = taskWorkOperator != null ? taskWorkOperator.OperateMsgZHCN : string.Empty;
                    if (taskWork.ProcInstID.HasValue
                        && !string.IsNullOrEmpty(taskWork.RefID) &&
                        taskWork.Url.ToLower().IndexOf(taskWork.RefID.ToLower()) < 0)
                    {
                        taskWork.Url = string.Format("{0}&projectId={1}", taskWork.Url, taskWork.RefID);
                    }

                }
                return Ok(new PagedDataSource(totalItems, list.ToArray()));
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteError(JsonConvert.SerializeObject(ex));
                throw ex;
            }



        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("api/TaskWork/CreateList")]
        public IHttpActionResult CreateList(TaskWork entity, List<RemindUserInfo> users)
        {

            var taskWorkEntity = new TaskWork();
            ObjectCopy objectCopy = new ObjectCopy();
            foreach (var remindUserInfo in users)
            {
                taskWorkEntity = objectCopy.AutoCopy(entity);
                taskWorkEntity.ReceiverAccount = remindUserInfo.UserAccount;
                taskWorkEntity.ReceiverNameENUS = remindUserInfo.UserNameENUS;
                taskWorkEntity.ReceiverNameZHCN = remindUserInfo.UserNameZHCN;
                taskWorkEntity.CreateTime = DateTime.Now;
                Create(taskWorkEntity);
            }
            return Ok();
        }


        /// <summary>
        /// 创建一个
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TaskWork/Create")]
        public IHttpActionResult Create(TaskWork entity)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (entity.Id == new Guid())
                {
                    entity.Id = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    TaskWork.Add(entity);
                }
                else
                {
                    TaskWork.Update(entity);
                }
            }
            catch (DbUpdateException)
            {
                if (TaskWork.Any(e => e.Id == entity.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = entity.Id }, entity);

        }



        /// <summary>
        /// 结束任务（根据引用ID和来源编号）
        /// </summary>
        /// <param name="entity">结束任务的条件</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TaskWork/Finish")]
        public int Finish(TaskWork entity)
        {
            if (string.IsNullOrEmpty(entity.RefID.Trim()) &&
                string.IsNullOrEmpty(entity.SourceCode.Trim()))
            {
                throw new ArgumentException("必须设置引用ID，来源编号");
            }

            string sql = string.Format("SELECT * FROM taskWork WHERE RefID = '{0}' and SourceCode = '{1}'", entity.RefID, entity.SourceCode);
            if (!string.IsNullOrEmpty(entity.ReceiverAccount.Trim()))
            {
                sql += string.Format(" AND ReceiverAccount = '{0}'", entity.ReceiverAccount);
            }
            var list = TaskWork.SqlQuery<TaskWork>(sql, null);
            return TaskWork.Update(list.ToArray());

        }
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="sourceCode">来源编号</param>
        /// <param name="refId">引用的Id</param>
        /// <returns></returns>
        [Route("api/TaskWork/get/{sourceCode}/{refId}")]
        public IHttpActionResult Get(string sourceCode, string refId)
        {
            var task = TaskWork.Search(c => c.SourceCode == sourceCode && c.RefID == refId).FirstOrDefault();

            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [Route("api/TaskWork/GetFinishedTasks/{sourceCode}/{refID}")]
        public IHttpActionResult GetFinishedTasks(string sourceCode, string refID)
        {
            var list = TaskWork.Search(c => c.Status == TaskWorkStatus.Finished && c.SourceCode == sourceCode && c.RefID == refID);
            return Ok(list);
        }

        [Route("api/taskwork/operators")]
        public IHttpActionResult GetOperators(string typeCode, string refID)
        {
            return Ok(TaskWork.GetOperators(typeCode, refID));
        }

        [Route("api/taskwork/ifUndo")]
        [HttpGet]
        public IHttpActionResult IfUndo(string typeCode, string refId)
        {
            return Ok(TaskWork.Any(t => t.TypeCode == typeCode && t.RefID == refId && t.Status == 0));
        }

        [Route("api/taskwork/Complete")]
        [HttpGet]
        public IHttpActionResult Complete(string projectId, string flowCode)
        {
            TaskWork.Complete(projectId, flowCode);

            switch (flowCode)
            {
                case FlowCode.MajorLease_SiteInfo:
                    ProjectInfo.FinishNode(projectId, flowCode, NodeCode.MajorLease_SiteInfo_CheckInfo, ProjectStatus.Finished);
                    break;
                case FlowCode.Reimage_SiteInfo:
                    ProjectInfo.FinishNode(projectId, flowCode, NodeCode.Reimage_SiteInfo_CheckInfo, ProjectStatus.Finished);
                    break;
                case FlowCode.Rebuild_SiteInfo:
                    ProjectInfo.FinishNode(projectId, flowCode, NodeCode.Rebuild_SiteInfo_CheckInfo, ProjectStatus.Finished);
                    break;
            }
            ProjectInfo.CompleteMainIfEnable(projectId);
            return Ok();
        }

        /// <summary>
        /// PMT 调用，用来获取任务数量
        /// </summary>
        /// <param name="eid">用户的EID，经过加密之后的EID</param>
        /// <returns>任务数量对象（代办任务数量/Notice数量/Reminder数量）</returns>
        [Route("api/taskwork/GetTaskItemsCount/{eid}")]
        [HttpGet]
        public IHttpActionResult GetTaskItemsCount(string eid)
        {
            TaskItemsCountModel model = new TaskItemsCountModel();
            try
            {
                //解密eid
                eid = Cryptography.Decrypt(eid, DateTime.Now.ToString("yyyyMMdd"), "oms");
                //获取代办任务数量
                IQueryable<TaskWork> result = TaskWork.GetUseableTaskWork();
                if (string.IsNullOrEmpty(eid) || string.IsNullOrWhiteSpace(eid))
                {
                    model.Successfull = false;
                    model.Message = "PMT调用获取任务数量的接口错误：传递的EID为空！";
                    Log4netHelper.WriteErrorLog(model.Message);
                }
                result = result.Where(c => (c.Status == TaskWorkStatus.UnFinish) && c.ReceiverAccount == eid.Trim());
                model.TaskCount = result.Count();

                //获取Notice 数量的接口
                model.NoticeCount = ModNotices.GetNotifyCountByEid(eid);

                //获取Reminder 数量的接口
                model.ReminderCount = Remind.Count(c => c.ReceiverAccount == eid.Trim() && !c.IsReaded);
                model.Successfull = true;
                model.Message = "接口调用成功！";
            }
            catch (Exception ex)
            {
                model.Successfull = false;
                model.Message = ex.Message;
                Log4netHelper.Log4netWriteErrorLog(model.Message, ex);
            }
            return Ok(model);
        }

    }
}