using System.Data.Entity.Validation;
using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using System.Transactions;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.Services.Common.Closure;
using Newtonsoft.Json.Linq;
using AutoMapper;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ClosureController : ApiController
    {
        private ProjectInfo projectInfoBLL = new ProjectInfo();

        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IHttpActionResult Get(Guid id)
        {
            var dic = ClosureInfo.Get(id);

            if (dic == null)
            {
                return NotFound();
            }

            return Ok(dic);
        }

        [Route("api/closure/project/{projectId}")]
        public IHttpActionResult Get(string projectId)
        {
            return Ok(ClosureInfo.FirstOrDefault(c => c.ProjectId == projectId));
        }

        [Route("api/closure/get")]
        public IHttpActionResult GetClosureInfo(string projectId)
        {
            return Ok(ClosureInfo.FirstOrDefault(c => c.ProjectId == projectId));
        }

        [Route("api/Closure/SearchPosition/{storeCode}")]
        [HttpGet]
        public IHttpActionResult SearchPosition(string storeCode)
        {
            var sql = string.Format(@"SELECT tb_employee.Code,tb_employee.NameZHCN,tb_employee.NameENUS,tb_position.NameENUS as PositionName,tb_position.PositionCode 
                FROM dbo.Store tb_store
                INNER JOIN dbo.UserMiniMarket tb_mini
                ON tb_mini.MiniMarketCode = tb_store.MiniMarketCode
                INNER JOIN dbo.UserPosition tb_position
                ON tb_position.Id = tb_mini.PositionId
                INNER JOIN dbo.Employee tb_employee
                ON tb_employee.Id = tb_position.UserId
                WHERE tb_store.code = '{0}' AND tb_position.PositionCode in ('suoya303051','suoya303055','suoya303057','suoya450007','suoya303050','suoya303054')
                AND tb_employee.Status = 1
                ORDER BY PositionCode", storeCode);
            //AND tb_employee.Status = 1
            var list = ClosureInfo.SqlQuery<SPosition>(sql, null).ToList();
            return Ok(list);
        }

        /// <summary>
        /// 提交Closure
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IHttpActionResult PostClosure(ClosureInfo entity)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {

                //    bll.BeginTransAction();
                //    try
                //    {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (entity.ClosureReasonCode != "Others")
                {
                    entity.ClosureReasonRemark = string.Empty;
                }

                if (entity.Id == new Guid())
                {
                    entity.Id = Guid.NewGuid();

                    entity.CreateDate = DateTime.Now;

                    entity.ProjectId = ProjectInfo.CreateMainProject(FlowCode.Closure, entity.USCode, NodeCode.Start, entity.CreateUserAccount);



                    ClosureInfo.Add(entity);
                }
                else
                {
                    ClosureInfo.Update(entity);
                }
                //    bll.Commit();
                //}
                //catch (Exception ex)
                //{
                //    bll.Rollback();
                //    return BadRequest(ex.Message);
                //}
                var usersList = new List<ProjectUsers>();
                var assetActor = GetClosureUsers(ProjectUserRoleCode.AssetActor, ProjectUserRoleCode.AssetActor, ProjectUserRoleCode.AssetActor, entity.AssetActorAccount, entity.AssetActorNameENUS, entity.AssetActorNameZHCN, entity.ProjectId);
                usersList.Add(assetActor);
                var assetRep = GetClosureUsers(ProjectUserRoleCode.AssetRep, ProjectUserRoleCode.AssetRep, ProjectUserRoleCode.AssetRep, entity.AssetRepAccount, entity.AssetRepNameENUS, entity.AssetRepNameZHCN, entity.ProjectId);
                usersList.Add(assetRep);
                var finance = GetClosureUsers(ProjectUserRoleCode.Finance, ProjectUserRoleCode.Finance, ProjectUserRoleCode.Finance, entity.FinanceAccount, entity.FinanceNameENUS, entity.FinanceNameZHCN, entity.ProjectId);
                usersList.Add(finance);
                var Legal = GetClosureUsers(ProjectUserRoleCode.Legal, ProjectUserRoleCode.Legal, ProjectUserRoleCode.Legal, entity.LegalAccount, entity.LegalNameENUS, entity.LegalNameZHCN, entity.ProjectId);
                usersList.Add(Legal);
                var PM = GetClosureUsers(ProjectUserRoleCode.PM, ProjectUserRoleCode.PM, ProjectUserRoleCode.PM, entity.PMAccount, entity.PMNameZHCN, entity.PMNameENUS, entity.ProjectId);
                usersList.Add(PM);
                var assertMgr = GetClosureUsers(ProjectUserRoleCode.AssetManager, ProjectUserRoleCode.AssetManager, ProjectUserRoleCode.AssetManager, entity.AssetManagerAccount, entity.AssetManagerNameENUS,
                    entity.AssetManagerNameZHCN, entity.ProjectId);
                usersList.Add(assertMgr);

                var cm = GetClosureUsers(ProjectUserRoleCode.CM, ProjectUserRoleCode.CM, ProjectUserRoleCode.CM, entity.CMAccount, entity.CMNameENUS,
                entity.CMNameZHCN, entity.ProjectId);
                usersList.Add(cm);
                if (entity.NecessaryNoticeUserList != null && entity.NecessaryNoticeUserList.Count > 0)
                {
                    usersList.AddRange(entity.NecessaryNoticeUserList.Select(
                        user => GetClosureUsers(ProjectUserRoleCode.View, ProjectUserRoleCode.View, ProjectUserRoleCode.View, user.Code, user.NameENUS, user.NameZHCN,
                            entity.ProjectId)));
                }
                if (entity.NoticeUserList != null && entity.NoticeUserList.Count > 0)
                {
                    usersList.AddRange(entity.NoticeUserList.Select(
                        user => GetClosureUsers(ProjectUserRoleCode.View, ProjectUserRoleCode.View, ProjectUserRoleCode.View, user.Code, user.NameENUS, user.NameZHCN,
                            entity.ProjectId)));
                }

                ProjectUsers.Add(usersList.ToArray());
                SendRemind(entity);
                SendWorkTaskAndEmail(entity);

                //初始化项目信息

                ProjectInfo.CreateSubProject(FlowCode.Closure_WOCheckList, entity.ProjectId, entity.USCode, NodeCode.Start, PM.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Closure_LegalReview, entity.ProjectId, entity.USCode, NodeCode.Start, assetActor.UserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Closure_ClosureTool, entity.ProjectId, entity.USCode, NodeCode.Start, finance.UserAccount);

                var store = StoreBasicInfo.GetStorInfo(entity.USCode);

                var closureTool = new ClosureTool();
                closureTool.Id = Guid.NewGuid();
                closureTool.ProjectId = entity.ProjectId;
                closureTool.IsHistory = false;
                closureTool.IsOptionOffered = entity.IsRelocation();
                closureTool.PipelineName = store.PipelineNameENUS;
                closureTool.RelocationPipelineID = store.PipelineID;
                closureTool.Add();

                var woCheckList = new ClosureWOCheckList
                {
                    Id = Guid.NewGuid(),
                    ProjectId = entity.ProjectId,
                    CreateTime = DateTime.Now,
                    CreateUserAccount = ClientCookie.UserCode,
                    CreateUserName = ClientCookie.UserNameENUS,
                    IsHistory = false
                };
                woCheckList.Add();

                var legalReview = new ClosureLegalReview
                {
                    Id = Guid.NewGuid(),
                    ProjectId = entity.ProjectId,
                    CreateTime = DateTime.Now,
                    CreateUserAccount = ClientCookie.UserCode,
                    CreateUserName = ClientCookie.UserNameENUS,
                    IsHistory = false
                };
                legalReview.Add();

                var executiveSummary = new ClosureExecutiveSummary
                {
                    Id = Guid.NewGuid(),
                    ProjectId = entity.ProjectId,
                    CreateTime = DateTime.Now,
                    CreatorAccount = ClientCookie.UserCode,
                    CreatorName = ClientCookie.UserNameENUS,
                    IsHistory = false
                };
                executiveSummary.Add();

                //var package = new ClosurePackage
                //{
                //    Id = Guid.NewGuid(),
                //    ProjectId = entity.ProjectId,
                //    CreateTime = DateTime.Now,
                //    CreateUserAccount = ClientCookie.UserCode,
                //    IsHistory = false
                //};
                //package.Add();

                var projectContractInfo = ProjectContractInfo.GetContractWithHistory(entity.ProjectId).Current;
                projectContractInfo.Add();

                ProjectInfo.CreateSubProject(FlowCode.Closure_ExecutiveSummary, entity.ProjectId, entity.USCode, NodeCode.Start, assetActor.UserAccount);

                ProjectInfo.CreateSubProject(FlowCode.Closure_ClosurePackage, entity.ProjectId, entity.USCode, NodeCode.Start, assetActor.UserAccount);

                ProjectInfo.CreateSubProject(FlowCode.Closure_ConsInvtChecking, entity.ProjectId, entity.USCode, NodeCode.Start, entity.PMAccount);


                ProjectInfo.CreateSubProject(FlowCode.Closure_ContractInfo, entity.ProjectId, entity.USCode,
                    NodeCode.Start, entity.CreateUserAccount);
                ProjectInfo.CreateSubProject(FlowCode.Closure_Memo, entity.ProjectId, entity.USCode,
               NodeCode.Start, entity.CreateUserAccount);
                ProjectNode.GenerateOnCreate(FlowCode.Closure, entity.ProjectId);
                try
                {

                    //bllActionLog.Add(new ActionLog
                    //{
                    //    Id = Guid.NewGuid(),
                    //    ProjectId = entity.ProjectId,
                    //    Action = ActionType.Submit,
                    //    CreateTime = DateTime.Now,
                    //    Operator = entity.CreateUserAccount,
                    //    OperatorENUS = entity.CreateUserNameENUS,
                    //    OperatorZHCN = entity.CreateUserNameZHCN,
                    //    Remark = "",
                    //    OperatorTitle = "创建流程"
                    //});
                    tranScope.Complete();
                    //bll.GetDb().SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    tranScope.Dispose();
                }


                return Ok(entity);
            }
        }

        private ProjectUsers GetClosureUsers(string roleNameENUS, string roleNameZHCN, string roleCode, string userAccount, string userNameENUS, string userNAMEZHCN, string projectId)
        {
            var closureUsers = new ProjectUsers();
            closureUsers.Id = Guid.NewGuid();
            closureUsers.RoleNameENUS = roleNameENUS;
            closureUsers.RoleNameZHCN = roleNameZHCN;
            closureUsers.RoleCode = roleCode;
            closureUsers.UserAccount = userAccount;
            closureUsers.UserNameENUS = userNameENUS;
            closureUsers.UserNameZHCN = userNAMEZHCN;

            closureUsers.Sequence = 0;
            closureUsers.ProjectId = projectId;
            closureUsers.CreateDate = DateTime.Now;

            return closureUsers;
        }



        private void SendWorkTaskAndEmail(ClosureInfo entity)
        {
            var newGuid = new Guid();

            var taskWorkController = new TaskWorkController();
            //任务通用部分
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Closure;
            taskWork.SourceNameZHCN = "关店流程";
            taskWork.SourceNameENUS = taskWork.SourceCode;
            taskWork.Status = 0;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            var title = TaskWork.BuildTitle(entity.ProjectId, entity.StoreNameZHCN, entity.StoreNameENUS);

            taskWork.Title = title; //string.Format("{0} {1}", entity.StoreNameCN, entity.StoreNameEN);
            taskWork.RefID = entity.ProjectId.ToString();
            taskWork.StoreCode = entity.USCode;
            taskWork.ActionName = NodeCode.Start;


            var objectCopy = new ObjectCopy();
            var taskWorkEntity = objectCopy.AutoCopy(taskWork);


            //创建WOChecklist任务
            taskWorkEntity.TypeCode = FlowCode.Closure_WOCheckList;
            taskWorkEntity.TypeNameENUS = "WOChecklist";
            taskWorkEntity.TypeNameZHCN = "WOChecklist";
            taskWorkEntity.ReceiverAccount = entity.PMAccount;
            taskWorkEntity.ReceiverNameENUS = entity.PMNameENUS;
            taskWorkEntity.ReceiverNameZHCN = entity.PMNameZHCN;
            taskWorkEntity.Id = newGuid;
            //taskWorkEntity.Url = SiteInfo.WebUrl + "/closure/Main#/closure/WOCheckList/" + entity.ProjectId;
            taskWorkEntity.Url = SiteInfo.GetProjectHandlerPageUrl(FlowCode.Closure_WOCheckList, entity.ProjectId);
            taskWorkEntity.ActivityName = "Start";
            taskWorkController.Create(taskWorkEntity);

            var closureHandler = new ClosureHandler();

            var noticerList = new List<string>();
            if (entity.NoticeUserList != null && entity.NoticeUserList.Count > 0)
            {
                noticerList = entity.NoticeUserList.Select(e => e.Account).ToList();
            }
            //noticerList.Add(entity.PMAccount);
            //closureHandler.SendEmail(newGuid, noticerList.ToArray(), "WOChecklist", entity.ProjectId, ClosureWOCheckList.TableName, entity.PMNameENUS);
            //noticerList.Remove(entity.PMAccount);


            taskWorkEntity = objectCopy.AutoCopy(taskWork);

            //创建Closure Tool任务
            taskWorkEntity.TypeCode = FlowCode.Closure_ClosureTool;
            taskWorkEntity.TypeNameENUS = "Closure Tool(Fin)";
            taskWorkEntity.TypeNameZHCN = "Closure Tool(Fin)";
            taskWorkEntity.ReceiverAccount = entity.FinanceAccount;
            taskWorkEntity.ReceiverNameZHCN = entity.FinanceNameZHCN;
            taskWorkEntity.ReceiverNameENUS = entity.FinanceNameENUS;
            taskWorkEntity.Id = newGuid;
            taskWorkEntity.Url = SiteInfo.GetProjectHandlerPageUrl(FlowCode.Closure_ClosureTool, entity.ProjectId);
            taskWorkEntity.ActivityName = "Start";
            taskWorkController.Create(taskWorkEntity);

            //noticerList.Add(entity.FinanceAccount);
            //closureHandler.SendEmail(newGuid, noticerList.ToArray(), "Closure Tool", entity.ProjectId, ClosureTool.TableName, entity.PMNameENUS);
            //noticerList.Remove(entity.FinanceAccount);


            taskWorkEntity = objectCopy.AutoCopy(taskWork);
            //创建LegalReview(Rep)任务
            taskWorkEntity.TypeCode = FlowCode.Closure_LegalReview;
            taskWorkEntity.TypeNameENUS = "LegalReview(Rep)";
            taskWorkEntity.TypeNameZHCN = "LegalReview(Rep)";
            taskWorkEntity.ReceiverAccount = entity.AssetActorAccount;
            taskWorkEntity.ReceiverNameZHCN = entity.AssetActorNameZHCN;
            taskWorkEntity.ReceiverNameENUS = entity.AssetActorNameENUS;
            taskWorkEntity.Id = newGuid;

            taskWorkEntity.Url = SiteInfo.GetProjectHandlerPageUrl(FlowCode.Closure_LegalReview, entity.ProjectId);
            taskWorkEntity.ActivityName = "Start";
            taskWorkController.Create(taskWorkEntity);

            //noticerList.Add(entity.AssetActorAccount);
            //closureHandler.SendEmail(newGuid, noticerList.ToArray(), "LegalReview", entity.ProjectId, ClosureLegalReview.TableName,entity.PMNameENUS);
            //noticerList.Remove(entity.AssetActorAccount);

        }

        [Route("api/Closure/GetByProjectId/{projectId}")]
        [HttpPost]
        [HttpGet]
        public ClosureInfo GetByProjectId(string projectId)
        {
            var entity = ClosureInfo.GetByProjectId(projectId);
            return entity;
        }

        /// <summary>
        /// 发送提醒和任务
        /// </summary>
        /// <param name="entity"></param>
        private void SendRemind(ClosureInfo entity)
        {
            var list = new List<RemindUserInfo>();
            var userInfo = new RemindUserInfo();
            userInfo.UserAccount = entity.AssetRepAccount;
            userInfo.UserNameZHCN = entity.AssetRepNameZHCN;
            userInfo.UserNameENUS = entity.AssetRepNameENUS;

            list.Add(userInfo);

            userInfo = new RemindUserInfo();
            userInfo.UserAccount = entity.AssetActorAccount;
            userInfo.UserNameZHCN = entity.AssetActorNameZHCN;
            userInfo.UserNameENUS = entity.AssetActorNameENUS;
            list.Add(userInfo);

            userInfo = new RemindUserInfo();
            userInfo.UserAccount = entity.FinanceAccount;
            userInfo.UserNameZHCN = entity.FinanceNameZHCN;
            userInfo.UserNameENUS = entity.FinanceNameENUS;
            list.Add(userInfo);

            userInfo = new RemindUserInfo();
            userInfo.UserAccount = entity.PMAccount;
            userInfo.UserNameZHCN = entity.PMNameZHCN;
            userInfo.UserNameENUS = entity.PMNameENUS;
            list.Add(userInfo);

            userInfo = new RemindUserInfo();
            userInfo.UserAccount = entity.CreateUserAccount;
            userInfo.UserNameZHCN = entity.CreateUserNameZHCN;
            userInfo.UserNameENUS = entity.CreateUserNameENUS;
            list.Add(userInfo);

            if (entity.NecessaryNoticeUserList != null && entity.NecessaryNoticeUserList.Count > 0)
            {
                foreach (var user in entity.NecessaryNoticeUserList)
                {
                    userInfo = new RemindUserInfo();
                    userInfo.UserAccount = user.Code;
                    userInfo.UserNameENUS = user.NameENUS;
                    userInfo.UserNameZHCN = user.NameZHCN;
                    list.Add(userInfo);
                }
            }

            if (entity.NoticeUserList != null && entity.NoticeUserList.Count > 0)
            {
                foreach (var user in entity.NoticeUserList)
                {
                    userInfo = new RemindUserInfo();
                    userInfo.UserAccount = user.Code;
                    userInfo.UserNameENUS = user.NameENUS;
                    userInfo.UserNameZHCN = user.NameZHCN;
                    list.Add(userInfo);
                }
            }


            var remind = new Remind();
            remind.SenderAccount = "Administrator";
            remind.SenderNameENUS = "Administrator";
            remind.SenderNameZHCN = "Administrator";
            remind.Title = string.Format("【{0}】Closure流程已创建", entity.ProjectId);
            remind.RegisterCode = "Closure";
            remind.IsReaded = false;


            var remindController = new RemindController();
            remindController.PostRemaindList(remind, list);


        }

        [Route("api/Closure/QueryUserPosition")]
        [HttpGet]
        public IHttpActionResult QueryUserPosition()
        {
            var user = Employee.GetSimpleEmployeeByCode(ClientCookie.UserCode);
            return Ok(user);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        [Route("api/Closure/BeginCreate/{projectCode}")]
        [HttpPost]
        public IHttpActionResult BeginCreate(string projectCode)
        {
            var bll = new ProjectIdsInfo();
            var entity = bll.GetByProjectCode(projectCode);


            if (entity == null)
            {
                return NotFound();
            }
            else
            {

                //entity.ProjectID++;
                entity.CurrentDate = DateTime.Now;
                //ProjectIdsInfo.Bll.Update(entity);
                //entity.LastUpdateTime = DateTime.Now;
                return Ok(entity);
            }
        }

        [Route("api/Closure/UpdateClosureInfo")]
        [HttpPost]
        public IHttpActionResult UpdateClosureInfo(ClosureInfo entity)
        {
            var closureTypeCode = Dictionary.FirstOrDefault(i => i.Code == entity.ClosureTypeCode);
            if (closureTypeCode != null)
            {
                entity.ClosureTypeNameENUS = closureTypeCode.NameENUS;
                entity.ClosureTypeNameZHCN = closureTypeCode.NameZHCN;
            }
            var riskStatus = Dictionary.FirstOrDefault(i => i.Code == entity.RiskStatusCode);
            if (riskStatus != null)
            {
                entity.RiskStatusNameENUS = riskStatus.NameENUS;
                entity.RiskStatusNameZHCN = riskStatus.NameZHCN;
            }
            var closureReason = Dictionary.FirstOrDefault(i => i.Code == entity.ClosureReasonCode);
            if (closureReason != null)
            {
                entity.ClosureReasonNameENUS = closureReason.NameENUS;
                entity.ClosureReasonNameZHCN = closureReason.NameZHCN;
                if (closureReason.Code != "Others")
                    entity.ClosureReasonRemark = "";
            }
            entity.Update();
            return Ok();
        }




        [Route("api/Closure/SummaryClosure")]
        [HttpPost]
        public IHttpActionResult SummaryClosure(JObject entity)
        {
            dynamic main = entity;
            JObject model = main.ClosureInfo;
            bool push = Convert.ToBoolean(main.push);
            var closure = model.ToObject<ClosureInfo>();

            using (TransactionScope tranScope = new TransactionScope())
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (closure.ClosureReasonCode != "Others")
                {
                    closure.ClosureReasonRemark = string.Empty;
                }

                if (closure.Id == new Guid())
                {
                    closure.Id = Guid.NewGuid();
                    closure.CreateDate = DateTime.Now;
                    closure.ProjectId = ProjectInfo.CreateSummaryProject(FlowCode.Closure, closure.USCode, closure.CreateUserAccount);
                    ClosureInfo.Add(closure);
                }
                else
                {
                    ClosureInfo.Update(closure);
                }

                try
                {
                    tranScope.Complete();
                }
                catch (DbEntityValidationException dbEx)
                {
                    tranScope.Dispose();
                }


                return Ok(entity);
            }
        }

        internal class SPosition
        {
            public string Code { get; set; }
            public string NameENUS { get; set; }
            public string NameZHCN { get; set; }
            public string PositionName { get; set; }
            public string PositionCode { get; set; }
        }
    }
}
