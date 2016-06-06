using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using System.IO;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.DataAccess.Entities;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.Services.Workflows.Closure;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.Services.Workflows.Enums;
using Mcdonalds.AM.Services.Workflows;
using NTTMNC.BPM.Fx.K2;
using Newtonsoft.Json;
using Mcdonalds.AM.Services.Infrastructure;
using NTTMNC.BPM.Fx.Core;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers.Closure
{
    public class LegalReviewController : ApiController
    {
        private McdAMEntities _db = new McdAMEntities();

        [HttpGet]
        [Route("api/LegalReview/LoadFinagreement/{id}")]
        public IHttpActionResult LoadFinagreement(Guid id)
        {
            Attachment result = null;
            var list = Attachment.GetList(ClosureLegalReview.TableName, id.ToString(), "FinAgreement");
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
            }
            if (list != null && list.Count > 0)
            {
                result = list[0];
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("api/LegalReview/LoadCurrentLetter/{id}")]
        public IHttpActionResult LoadCurrentLetter(Guid id)
        {
            Attachment result = null;
            var list = Attachment.GetList(ClosureLegalReview.TableName, id.ToString(), "CurrentLetter");
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
            }
            if (list != null && list.Count > 0)
            {
                result = list[0];
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("api/LegalReview/GetContract/{id}")]
        public IHttpActionResult GetContract(Guid id)
        {
            var list = Attachment.GetList(ClosureLegalReview.TableName, id.ToString(), "Contract");
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
            }
            return Ok(list);
        }
        [HttpGet]
        [Route("api/LegalReview/GetByProjectId/{projectId}/{userAccount}")]
        public ClosureLegalReview GetByProjectId(string projectId, string userAccount)
        {
            ClosureLegalReview entity = ClosureLegalReview.Get(projectId);

            if (entity != null)
            {
                ProjectCommentCondition condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Closure;
                condition.UserAccount = userAccount;
                condition.RefTableId = entity.Id;
                condition.RefTableName = ClosureLegalReview.TableName;
                condition.Status = ProjectCommentStatus.Save;

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.Comments = comments[0].Content;
                }
            }


            return entity;

        }
        //根据ProcInstID获取数据
        [Route("api/LegalReview/GetByID/{id}")]
        [HttpGet]
        public IHttpActionResult GetById(Guid id)
        {
            var entity = ClosureLegalReview.GetById(id);
            return Ok(entity);
        }

        //根据ProcInstID获取数据
        [Route("api/LegalReview/GetByProcInstID/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetByProcInstID(int procInstID)
        {
            var entity = ClosureLegalReview.FirstOrDefault(e => e.ProcInstID == procInstID);
            return Ok(entity);
        }

        [Route("api/LegalReview/GetClosureCommers/{userAccount}/{refTableName}/{refTableId}")]
        [HttpGet]
        public IHttpActionResult GetClosureCommers(string userAccount, string refTableName, Guid refTableId)
        {
            ProjectCommentCondition condition = new ProjectCommentCondition();
            condition.SourceCode = FlowCode.Closure;
            condition.UserAccount = userAccount;
            condition.RefTableId = refTableId;
            ProjectComment.SearchList(condition);
            var list = _db.ProjectComment.Where(e => e.RefTableName == refTableName && e.RefTableId == refTableId
                && string.IsNullOrEmpty(e.Content) && e.SourceCode == "Closure").OrderByDescending(e => e.CreateTime);
            return Ok(list);
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [Route("api/LegalReview/ProcessLegalReview")]
        [HttpPost]
        public IHttpActionResult ProcessClosureLegalReview(ClosureLegalReview entity)
        {
            int procInstID = entity.ProcInstID.Value;


            string account = ClientCookie.UserCode;
            //评论信息
            string comments = entity.Comments;

            // To-Do K2 action
            ProcessActionResult _action = BPMHelper.ConvertToProcAction(entity.Action);


            string op = string.Empty;
            var _listDataFields = new List<ProcessDataField>();
            switch (entity.Action)
            {
                case "Submit":

                    break;
                case "Return":
                    op = ProjectCommentAction.Return;
                    break;
                case "ReSubmit":
                    op = ProjectCommentAction.ReSubmit;
                    var legalAccount = entity.LegalAccount;
                    _listDataFields.Add(new ProcessDataField("dest_Legal", legalAccount));//entity.   Legal
                    break;
            }

            ClosureLegalReview.Update(entity);
            var result = false;
            if (_listDataFields.Count > 0)
                result = K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments, _listDataFields);
            else
                result = K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments);

            if (result)
            {
                switch (entity.Action)
                {
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_LegalReview, ProjectStatus.Recalled);
                        break;
                    case ProjectAction.Return:
                        ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_LegalReview);
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(entity.ProjectId, FlowCode.Closure_LegalReview);
                        break;
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_LegalReview, NodeCode.Closure_LegalReview_Input);
                        break;
                    default:
                        ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_LegalReview, NodeCode.Closure_LegalReview_Input);
                        break;
                }
            }
            SaveCommers(entity, _action.ToString(), ProjectCommentStatus.Submit);
            return Ok();
        }

        [Route("api/LegalReview/SaveClosureLegalReview")]
        [HttpPost]
        public IHttpActionResult SaveClosureLegalReview(ClosureLegalReview entity)
        {

            var entityInfo = ClosureLegalReview.Get(entity.ProjectId);
            if (entityInfo == null)
            {

                //entity = new ClosureLegalReview();
                entity.CreateTime = DateTime.Now;
                entity.CreateUserAccount = ClientCookie.UserCode;
                entity.Id = Guid.NewGuid();

                entity.IsHistory = false;
                _db.ClosureLegalReview.Add(entity);
                //}
            }
            else
            {
                try
                {
                    entity.Id = entityInfo.Id;
                    entity.CreateTime = DateTime.Now;
                    entity.CreateUserAccount = ClientCookie.UserCode;
                    entity.IsHistory = false;
                    //entityInfo.LegalCommers = entity.LegalCommers;
                    _db.ClosureLegalReview.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            string action = string.IsNullOrEmpty(entity.Action) ?
            ProjectCommentAction.Submit : entity.Action;
            SaveCommers(entity, action, ProjectCommentStatus.Save);
            var result = _db.SaveChanges();
            return (Ok());
        }



        [Route("api/LegalReview/PostClosureLegalReview")]
        [HttpPost]
        public IHttpActionResult PostClosureLegalReview(ClosureLegalReview entity)
        {
            var task = _db.TaskWork.First(
              e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                   && e.TypeCode == FlowCode.Closure_LegalReview && e.RefID == entity.ProjectId
              );

            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_LegalReview, task.RefID);

            //var enableExecutiveSummary = handler.EnableExecutiveSummary(entity.ProjectId.Value);

            _db.TaskWork.Attach(task);
            _db.Entry(task).State = EntityState.Modified;

            var entityInfo = ClosureLegalReview.Get(entity.ProjectId);
            //GUID通过页面JS生成需要判断否存在


            if (entityInfo == null)
            {

                //entity = new ClosureLegalReview();
                entity.CreateTime = DateTime.Now;
                entity.CreateUserAccount = ClientCookie.UserCode;
                entity.Id = Guid.NewGuid();
                entity.IsHistory = false;
                _db.ClosureLegalReview.Add(entity);
                //}
            }
            else
            {
                try
                {
                    //entityInfo.LegalCommers = entity.Comments;
                    _db.ClosureLegalReview.Attach(entityInfo);
                    _db.Entry(entityInfo).State = EntityState.Modified;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            //bllActionLog.Add(new ActionLog
            //{
            //    Id = Guid.NewGuid(),
            //    ProjectId = entity.ProjectId,
            //    Action = ActionType.Submit,
            //    CreateTime = DateTime.Now,
            //    Operator = entity.CreateUserAccount,
            //    OperatorENUS = entity.UserNameENUS,
            //    OperatorZHCN = entity.UserNameZHCN,
            //    Remark = "提交LegalReview流程",
            //    OperatorTitle = "提交LegalReview流程"
            //});


            var result = _db.SaveChanges();

            if (result > 0)
            {
                // Start K2 Process
                string _procCode = WFClosureLegalReview.ProcessCode;
                List<ProcessDataField> _listDataFields = new List<ProcessDataField>();

                _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode)); // 发起人

                var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);


                //string legalAccount = userPositionHandler.GetAccounts(entity.ProjectId.Value,
                //   UserPositionHandler.LegalCounselCode );

                string legalAccount = entity.LegalAccount;

                _listDataFields.Add(new ProcessDataField("dest_Legal", legalAccount));//entity.   Legal

                _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));
                //_listDataFields.Add(new ProcessDataField("dest_Receiver", "")); 知会人

                if (_listDataFields.Exists(o => string.IsNullOrEmpty(o.DataFieldValue)))
                {
                    result = 0;
                }
                else
                {
                    //将TaskWork生成任务传给K2
                    var taskJson = TaskWork.ConvertToJson(task);
                    _listDataFields.Add(new ProcessDataField("ProjectTaskInfo", taskJson));

                    var _debugInfo = string.Format("[Ln 326] DataFields: {0}", JsonConvert.SerializeObject(_listDataFields));
                    Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureLegalReview");

                    int _procInstID = 0;

                    try
                    {
                        _procInstID = K2FxContext.Current.StartProcess(_procCode, ClientCookie.UserCode, _listDataFields);
                    }
                    catch (Exception ex)
                    {
                        _debugInfo = string.Format("[Ln 339] Result: {0}", ex.Message);
                        Log4netHelper.Log4netWriteErrorLog(_debugInfo, ex);
                        Log4netHelper.WriteErrorLog(_debugInfo);
                        Log4netHelper.WriteInfoLog(_debugInfo);
                        throw ex;
                    }

                    _debugInfo = string.Format("[Ln 345] Result: {0}", _procInstID);
                    Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureLegalReview");



                    if (_procInstID > 0)
                    {
                        if (entityInfo == null)
                        {

                            _db.ClosureLegalReview.Attach(entity);
                            _db.Entry(entity).State = EntityState.Modified;
                        }
                        else
                        {
                            entity.Id = entityInfo.Id;
                            entityInfo.ProcInstID = _procInstID;
                            _db.ClosureLegalReview.Attach(entityInfo);
                            _db.Entry(entityInfo).State = EntityState.Modified;
                        }

                        entity.ProcInstID = _procInstID;
                        SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Submit);
                        result = _db.SaveChanges();
                    }
                }
            }

            ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_LegalReview,
         NodeCode.Closure_LegalReview_Input);

            return Ok(result);
        }

        [Route("api/LegalReview/GetK2Status/{account}/{sn}/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetK2Status(string account, string sn, string procInstID)
        {
            // Load K2 Process
            bool result = false;
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, account);
            result = !resultStr.Equals(WFClosureLegalReview.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

            if (result)
            {
                // 非发起人节点
                resultStr = "Process";
            }
            else
            {
                resultStr = "Edit";
            }

            object resultObj = new
            {
                Status = resultStr
            };
            return Ok(resultObj);

        }
        [HttpPost]
        [Route("api/LegalReview/UploadFinAgreement/{procInstID}")]
        public IHttpActionResult UploadFinAgreement(int procInstID)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            string internalName = string.Empty;
            ClosureLegalReview legalReview = new ClosureLegalReview();

            if (FileCollect.Count > 0) //如果集合的数量大于0
            {

                var woEntity = legalReview.GetByProcInstID(procInstID);

                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = FileCollect[0];
                string fileName = "FinalTerminationAgreement";
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                Attachment att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosureLegalReview.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = FileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.TypeCode = "FinAgreement";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorID = ClientCookie.UserCode;

                Attachment.SaveSigleFile(att);


                ProjectInfo.FinishNode(woEntity.ProjectId, FlowCode.Closure_LegalReview,
                        NodeCode.Closure_LegalReview_UploadAgreement);


            }

            return Ok();
        }

        [HttpPost]
        [Route("api/LegalReview/UploadCurrentLetter/{procInstID}")]
        public IHttpActionResult UploadCurrentLetter(int procInstID)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            string internalName = string.Empty;
            ClosureLegalReview legalReview = new ClosureLegalReview();

            if (FileCollect.Count > 0) //如果集合的数量大于0
            {

                var woEntity = legalReview.GetByProcInstID(procInstID);

                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = FileCollect[0];
                string fileName = "往来函件";
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                Attachment att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosureLegalReview.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = FileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.TypeCode = "CurrentLetter";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorID = ClientCookie.UserCode;

                Attachment.SaveSigleFile(att);
            }

            return Ok();
        }


        [HttpPost]
        [Route("api/LegalReview/UploadContract/{projectId}")]
        public IHttpActionResult UploadContract(string projectId)
        {


            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            int result = 0;
            string resultStr = string.Empty;
            string internalName = string.Empty;
            if (FileCollect.Count > 0) //如果集合的数量大于0
            {
                var entity = ClosureLegalReview.Get(projectId);

                if (entity == null)
                {
                    entity = new ClosureLegalReview();
                    entity.ProjectId = projectId;
                    entity.Id = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreateUserAccount = ClientCookie.UserCode;

                    entity.IsHistory = false;

                    _db.ClosureLegalReview.Add(entity);
                    //}
                }


                for (int i = 0; i < FileCollect.Count; i++)
                {
                    //用key获取单个文件对象HttpPostedFile
                    HttpPostedFile fileSave = FileCollect[i];

                    string fileName = Path.GetFileName(fileSave.FileName);


                    string fileExtension = Path.GetExtension(fileSave.FileName);
                    var current = System.Web.HttpContext.Current;

                    internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;
                    //通过此对象获取文件名

                    fileSave.SaveAs(absolutePath);

                    Attachment att = new Attachment();
                    att.InternalName = internalName;
                    att.RefTableName = ClosureLegalReview.TableName;
                    att.RefTableID = entity.Id.ToString();
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = fileExtension;
                    att.Length = FileCollect[i].ContentLength;
                    att.CreateTime = DateTime.Now;
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "Contract";
                    att.CreatorID = ClientCookie.UserCode;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    _db.Attachment.Add(att);


                }
                result = _db.SaveChanges();
            }
            return Ok(result);

        }


        private void SaveCommers(ClosureLegalReview entity, string action, ProjectCommentStatus status)
        {


            //if (status == ProjectCommentStatus.Save)
            //{

            var list = _db.ProjectComment.Where(
             c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == ProjectCommentStatus.Save
                  && c.RefTableName == ClosureLegalReview.TableName && c.SourceCode == FlowCode.Closure).ToList();

            if (list.Count > 0)
            {
                ProjectComment closureCommens = list[0];
                closureCommens.Content = entity.Comments == null ? "" : entity.Comments.Trim();
                closureCommens.Status = status;
                if (entity.ProcInstID.HasValue && entity.ProcInstID.Value > 0)
                {
                    closureCommens.ProcInstID = entity.ProcInstID;
                }
                ProjectComment.Update(closureCommens);
            }
            else
            {
                AddProjectComment(entity, action, status);
            }
        }

        private void AddProjectComment(ClosureLegalReview entity, string action,
            ProjectCommentStatus status)
        {
            ProjectComment closureCommens = new ProjectComment();
            closureCommens.RefTableId = entity.Id;
            closureCommens.RefTableName = ClosureLegalReview.TableName;

            closureCommens.TitleNameENUS = ClientCookie.TitleENUS;
            closureCommens.TitleNameZHCN = ClientCookie.TitleENUS;
            closureCommens.TitleCode = ClientCookie.TitleENUS;

            closureCommens.CreateTime = DateTime.Now;
            closureCommens.CreateUserAccount = ClientCookie.UserCode;


            closureCommens.UserAccount = ClientCookie.UserCode;
            closureCommens.UserNameENUS = ClientCookie.UserNameENUS;
            closureCommens.UserNameZHCN = ClientCookie.UserNameZHCN;
            closureCommens.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            closureCommens.Id = Guid.NewGuid();
            if (!string.IsNullOrEmpty(entity.Comments))
            {
                closureCommens.Content = entity.Comments.Trim();
            }
            closureCommens.Action = action;
            closureCommens.Status = status;
            if (entity.ProcInstID > 0)
            {
                closureCommens.ProcInstID = entity.ProcInstID;
            }
            closureCommens.SourceCode = FlowCode.Closure;
            closureCommens.SourceNameENUS = FlowCode.Closure;
            closureCommens.SourceNameZHCN = "关店流程";
            closureCommens.Add();
        }

        [Route("api/LegalReview/Edit")]
        [HttpPost]
        public IHttpActionResult Edit(ClosureLegalReview entity)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var result = entity.Edit();
                tranScope.Complete();
                return Ok(new
                {
                    TaskUrl = result
                });
            }
        }

        private int ModifyProject(ClosureLegalReview entity, string action)
        {

            if (action == ProjectAction.Edit)
            {
                entity.IsHistory = true;
                entity.LastUpdateTime = DateTime.Now;
                entity.LastUpdateUserAccount = ClientCookie.UserCode;
                entity.LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
                entity.LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;

                _db.ClosureLegalReview.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
            }
            //  projectInfoBLL.UpdateProjectNode(entity.ProjectId, FlowCode.Closure_LegalReview,

            // NodeCode.Start);

            var result = _db.SaveChanges();

            return result;
        }

        [Route("api/LegalReview/Recall")]
        [HttpPost]
        public IHttpActionResult Recall(ClosureLegalReview entity)
        {
            bool _recallSuccess = false;
            if (entity.ProcInstID != null)
            {
                _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value, WFClosureLegalReview.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, entity.Comments);
                if (_recallSuccess)
                {

                    SaveCommers(entity, ProjectCommentAction.Recall, ProjectCommentStatus.Submit);
                }
            }
            if (!_recallSuccess)
            {
                throw new Exception("Recall失败");
            }
            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_LegalReview, ProjectStatus.Recalled);

            return Ok();

        }
    }
}
