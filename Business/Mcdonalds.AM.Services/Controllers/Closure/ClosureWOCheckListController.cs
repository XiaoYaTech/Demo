using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Entities.Closure.Enum;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using MyExcel.NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data.Entity;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.Core;
using Mcdonalds.AM.Services.Workflows;
using Mcdonalds.AM.Services.Workflows.Enums;
using Mcdonalds.AM.Services.Workflows.Closure;
using Newtonsoft.Json;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ClosureWOCheckListController : ApiController
    {
        [Route("api/ClosureWOCheckList/GetByProjectId/{projectID}")]
        [HttpGet]
        public IHttpActionResult GetByProjectId(string projectId)
        {
            var entity = ClosureWOCheckList.Get(projectId);
            if (entity != null)
            {
                ProjectCommentCondition condition = new ProjectCommentCondition();
                condition.RefTableId = entity.Id;
                condition.RefTableName = ClosureWOCheckList.TableName;
                condition.UserAccount = ClientCookie.UserCode;
                condition.Status = ProjectCommentStatus.Save;
                var commentList = ProjectComment.SearchList(condition);
                if (commentList != null && commentList.Count > 0)
                {
                    entity.Comments = commentList[0].Content;
                }
                var closureEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
                entity.USCode = closureEntity.USCode;
                entity.UserAccount = ClientCookie.UserCode;

                if (string.IsNullOrEmpty(entity.PMSupervisorAccount))
                {
                    var puser = ProjectUsers.FirstOrDefault(i => i.ProjectId == entity.ProjectId && i.RoleCode == ProjectUserRoleCode.CM);
                    if (puser != null)
                        entity.PMSupervisorAccount = puser.UserAccount;
                }
                // entity.Comments = GetCommers(entity);
                //string pmAccount = closureEntity.PMAccount;
                //entity.PMSupervisorAccount = userPositionHandler.GetReportToAccounts(pmAccount);
                //string[] accounts = {UserPositionHandler.IT,UserPositionHandler.MCCL_Construction_Mgr};



                //var positionlist = userPositionHandler.SearchUserPosition(closureEntity.USCode,accounts);
                //string mcclApprover = userPositionHandler.GetAccounts(positionlist);
                //entity.MCCLApproverAccount = mcclApprover;
            }
            return Ok(entity);
        }

        [Route("api/ClosureWOCheckList/GetById/{Id}")]
        [HttpGet]
        public IHttpActionResult GetById(string Id)
        {
            Guid _Id;
            if (!Guid.TryParse(Id, out _Id))
                return Ok();
            var entity = ClosureWOCheckList.Get(_Id);
            if (entity != null)
            {
                ProjectCommentCondition condition = new ProjectCommentCondition();
                condition.RefTableId = entity.Id;
                condition.RefTableName = ClosureWOCheckList.TableName;
                condition.UserAccount = ClientCookie.UserCode;
                condition.Status = ProjectCommentStatus.Save;
                var commentList = ProjectComment.SearchList(condition);
                if (commentList != null && commentList.Count > 0)
                {
                    entity.Comments = commentList[0].Content;
                }
                var closureEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
                entity.USCode = closureEntity.USCode;
                entity.UserAccount = ClientCookie.UserCode;
            }
            return Ok(entity);
        }

        [Route("api/ClosureWOCheckList/GetTemplates/{projectId}")]
        [HttpGet]
        public IHttpActionResult GetTemplates(string projectId)
        {
            Attachment att = new Attachment();

            var entity = ClosureWOCheckList.Get(projectId);

            List<Attachment> list = null;
            if (entity != null)
            {
                list = Attachment.GetList(ClosureWOCheckList.TableName, entity.Id.ToString(), "Template");
                foreach (var item in list)
                {
                    //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                    item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
                }
            }
            return Ok(list);
        }

        [Route("api/ClosureWOCheckList/GetHistoryTemplates/{entityId}")]
        [HttpGet]
        public IHttpActionResult GetHistoryTemplates(Guid entityId)
        {
            Attachment att = new Attachment();

            var entity = ClosureWOCheckList.Get(entityId);

            List<Attachment> list = null;
            if (entity != null)
            {
                list = Attachment.GetList(ClosureWOCheckList.TableName, entity.Id.ToString(), "Template");
                foreach (var item in list)
                {
                    //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                    item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
                }
            }
            return Ok(list);
        }




        [HttpGet]
        [Route("api/ClosureWOCheckList/GetAttachements/{id}")]
        public IHttpActionResult GetAttachements(Guid id)
        {
            var list = Attachment.GetList(ClosureWOCheckList.TableName, id.ToString(), "Attachment");
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
            }
            return Ok(list);
        }

        private McdAMEntities _db = new McdAMEntities();
        [Route("api/ClosureWOCheckList/DownLoadTemplate/{projectID}")]
        [HttpGet]
        public IHttpActionResult DownLoadTemplate(string projectID)
        {
            var current = System.Web.HttpContext.Current;
            string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.FAWrite_offTool_Template;


            Excel excel = new Excel();
            string templatePath = path;

            string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + Guid.NewGuid() + ".xlsx";

            var closure = ClosureInfo.GetByProjectId(projectID);
            excel.Open(path);
            excel.Save(tempFilePath);

            var store = _db.StoreBasicInfo.First(e => e.StoreCode == closure.USCode);


            Excel tempExcel = new Excel();
            tempExcel.Open(tempFilePath);
            var sheet = tempExcel.Sheets["PMT"];

            sheet.Cells[1, 1].StrValue = store.RegionENUS;
            sheet.Cells[2, 1].StrValue = store.MarketENUS;
            sheet.Cells[3, 1].StrValue = store.NameENUS;
            sheet.Cells[4, 1].StrValue = store.StoreCode;
            sheet.Cells[5, 1].StrValue = store.StoreTypeName;
            if (closure.ActualCloseDate != null)
                sheet.Cells[6, 1].StrValue = closure.ActualCloseDate.Value.ToString("yyyy-MM-dd");
            sheet.Cells[7, 1].StrValue = closure.PMNameZHCN;

            tempExcel.Save(tempFilePath);

            var currentNode = NodeInfo.GetCurrentNode(projectID, FlowCode.Closure_WOCheckList);
            var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_WOCheckList_DownLoadTemplate);
            if (newNode.Sequence > currentNode.Sequence)
            {
                ProjectInfo.FinishNode(projectID, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_DownLoadTemplate);
            }


            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + SiteFilePath.GetTemplateFileName(closure.USCode, FlowCode.Closure_WOCheckList, SiteFilePath.FAWrite_offTool_Template));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();
            return Ok();
        }


        [HttpGet]
        [Route("api/ClosureWOCheckList/GetClosingCost/{id}")]
        public IHttpActionResult GetClosingCost(Guid id)
        {
            Attachment result = null;
            var list = Attachment.GetList(ClosureWOCheckList.TableName, id.ToString(), "ClosingCost");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            if (list != null && list.Count > 0)
            {
                result = list[0];
            }
            return Ok(result);
        }


        [Route("api/ClosureWOCheckList/UploadClosingCost/{projectid}")]
        [HttpPost]
        public IHttpActionResult UploadClosingCost(string projectid)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            int result = 0;
            string resultStr = string.Empty;
            string internalName = string.Empty;


            if (FileCollect.Count > 0) //如果集合的数量大于0
            {

                var woEntity = ClosureWOCheckList.Get(projectid);

                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = FileCollect[0];
                string fileExtension = Path.GetExtension(fileSave.FileName);
                string fileName = "ClosingCost" + fileExtension;
                var current = System.Web.HttpContext.Current;

                internalName = Guid.NewGuid() + fileExtension;
                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                fileSave.SaveAs(absolutePath);

                Attachment att = new Attachment();
                att.InternalName = internalName;
                att.RefTableName = ClosureWOCheckList.TableName;
                att.RefTableID = woEntity.Id.ToString();
                att.RelativePath = "//";
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = FileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.TypeCode = "ClosingCost";
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorID = ClientCookie.UserCode;

                Attachment.SaveSigleFile(att);

                var currentNode = NodeInfo.GetCurrentNode(projectid, FlowCode.Closure_WOCheckList);
                var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_WOCheckList_ClosingCost);
                if (newNode.Sequence > currentNode.Sequence)
                {
                    ProjectInfo.FinishNode(projectid, FlowCode.Closure_WOCheckList,
                        NodeCode.Closure_WOCheckList_ClosingCost);
                }

            }

            return Ok(NodeCode.Closure_WOCheckList_Approve);
        }

        [Route("api/ClosureWOCheckList/UploadAttachement/{id}")]
        [HttpPost]
        public IHttpActionResult UploadAttachement(string id)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            int result = 0;
            string resultStr = string.Empty;
            string internalName = string.Empty;
            if (FileCollect.Count > 0) //如果集合的数量大于0
            {
                var woEntity = ClosureWOCheckList.Get(id);


                for (int i = 0; i < FileCollect.Count; i++)
                {
                    var fileSave = FileCollect[i];
                    //用key获取单个文件对象HttpPostedFile

                    string fileName = Path.GetFileName(fileSave.FileName);
                    string fileExtension = Path.GetExtension(fileSave.FileName);
                    var current = System.Web.HttpContext.Current;
                    internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;

                    fileSave.SaveAs(absolutePath);

                    Attachment att = new Attachment();
                    att.InternalName = internalName;
                    att.RefTableName = ClosureWOCheckList.TableName;
                    att.RefTableID = woEntity.Id.ToString();
                    att.RelativePath = "//";
                    att.Name = fileName;
                    att.Extension = fileExtension;
                    att.Length = FileCollect[0].ContentLength;
                    att.CreateTime = DateTime.Now;
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "Attachment";
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.CreatorID = ClientCookie.UserCode;
                    _db.Attachment.Add(att);
                    result = _db.SaveChanges();
                }

                resultStr = SiteInfo.WebUrl + "UploadFiles/" + internalName;


            }
            return Ok(resultStr);

        }


        [Route("api/ClosureWOCheckList/Edit")]
        [HttpPost]
        public IHttpActionResult Edit(ClosureWOCheckList entity)
        {



            return Ok(new
            {
                TaskUrl = entity.Edit()
            });
        }

        private int ModifyProject(ClosureWOCheckList entity, string action)
        {
            // 2014-08-05 victor.huang: Recall 或Edit 后不需要重新再生成Task，不然会多生成一条冗余记录
            //TaskWorkCondition condition = new TaskWorkCondition();
            //condition.ProjectId = entity.ProjectId;
            //condition.Url = "/closure/Main#/closure/WOCheckList/" + entity.ProjectId;
            //condition.UserAccount = ClientCookie.UserCode;
            //condition.UserNameENUS = entity.UserNameENUS;
            //condition.UserNameZHCN = entity.UserNameZHCN;

            //taskWorkBll.ReSendTaskWork(condition);

            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_WOCheckList);

            var result = 0;

            result = _db.SaveChanges();

            return result;
        }

        [Route("api/ClosureWOCheckList/Recall")]
        [HttpPost]
        public IHttpActionResult Recall(ClosureWOCheckList entity)
        {
            //
            bool _recallSuccess = false;
            if (entity.ProcInstID != null)
            {

                _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value, WFClosureWOCheckList.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, entity.Comments);
                if (_recallSuccess)
                {

                    SaveCommers(entity, ProjectCommentAction.Recall, ProjectCommentStatus.Submit);
                }

            }
            if (!_recallSuccess)
            {
                throw new Exception("Recall失败");
            }
            //ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_WOCheckList, ProjectStatus.Recalled);
            ProjectInfo.UpdateProjectStatus(entity.ProjectId, FlowCode.Closure_WOCheckList, ProjectStatus.Recalled);

            return Ok();
        }

        public IHttpActionResult Finnish(ClosureWOCheckList entity)
        {
            entity.ProgressRate = 100;
            entity.Status = (int)WOCheckListStatus.Finish;
            var task = _db.TaskWork.First(
                e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                     && e.TypeCode == FlowCode.Closure_WOCheckList && e.RefID == entity.ProjectId
                );

            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_WOCheckList, task.RefID);

            _db.TaskWork.Attach(task);
            _db.ClosureWOCheckList.Attach(entity);
            var result = _db.SaveChanges();
            return Ok(result);
        }

        //public IHttpActionResult Reject(ClosureWOCheckList entity)
        //{
        //    entity.ProgressRate = 60;
        //    entity.Status = (int)WOCheckListStatus.ClosingCost;
        //    var task = _db.TaskWork.First(
        //        e => e.ReceiverAccount == ClosureInfo.UserAccount && e.Status == 0 && e.SourceCode == "Closure"
        //             && e.TypeCode == "WOChecklist" && e.RefID == entity.ProjectId.ToString()
        //        );

        //    task.Status = 0;
        //    task.FinishTime = null;

        //    _db.TaskWork.Attach(task);
        //    _db.ClosureWOCheckList.Attach(entity);
        //    var result = _db.SaveChanges();
        //    return Ok(result);
        //}

        [Route("api/ClosureWOCheckList/GetClosureCommers/{refTableName}/{refTableId}")]
        [HttpGet]
        public IHttpActionResult GetClosureCommers(string refTableName, Guid refTableId)
        {
            var list = _db.ProjectComment.Where(e => e.RefTableName == refTableName && e.RefTableId == refTableId
               && string.IsNullOrEmpty(e.Content) && e.SourceCode == "Closure").OrderByDescending(e => e.CreateTime);
            return Ok(list);
        }

        [Route("api/ClosureWOCheckList/GetHistoryList/{projectId}")]
        [HttpGet]
        public IHttpActionResult GetHistoryList(string projectId)
        {
            // var list = _db.ClosureWOCheckList.Where(e => e.ProjectId == projectId && e.IsAvailable == false).OrderByDescending(e => e.CreateTime);
            // woCheckListHandler.GetHistoryList(projectId);
            return Ok();
        }
        [Route("api/ClosureWOCheckList/DeleteClosingCost/{id}/{attId}")]
        [HttpGet]
        public IHttpActionResult DeleteClosingCost(Guid id, Guid attId)
        {
            var entity = _db.ClosureWOCheckList.Find(id);
            ProjectInfo.UnFinishNode(entity.ProjectId, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_ClosingCost);

            Attachment.Delete(attId);
            return Ok(NodeCode.Closure_WOCheckList_ClosingCost);
        }

        [Route("api/ClosureWOCheckList/ProcessClosureWOCheckList")]
        [HttpPost]
        public IHttpActionResult ProcessClosureWOCheckList(ClosureWOCheckList entity)
        {
            int procInstID = entity.ProcInstID.Value;

            string actionLower = entity.Action.ToLower();




            string account = ClientCookie.UserCode;
            //评论信息
            string comments = entity.Comments;

            // To-Do K2 action
            ProcessActionResult _action = BPMHelper.ConvertToProcAction(actionLower);

            if (actionLower.Equals(ProjectAction.Return, StringComparison.CurrentCultureIgnoreCase))
            {
                TaskWork.Finish(e => e.RefID == entity.ProjectId
                    && e.TypeCode == FlowCode.Closure_WOCheckList
                    && e.Status == TaskWorkStatus.UnFinish
                    && e.K2SN != entity.SN);
            }

            if (actionLower == "resubmit")
            {
                List<ProcessDataField> _listDataFields = new List<ProcessDataField>();

                _listDataFields.Add(new ProcessDataField("dest_PMSupervisor", entity.PMSupervisorAccount));// 工程PM直属上级
                _listDataFields.Add(new ProcessDataField("dest_MCCLApprovers", string.Join(";", entity.MCCLApproverAccount, entity.MCCLITApproverAccount, entity.MCCLMCCLEqApproverAccount)));// MCCL Approvers
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments, _listDataFields);

                entity.Save();
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments);




            if (actionLower == "submit")
            {


            }
            else if (actionLower == "resubmit")
            {
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_WOCheckList,
                   NodeCode.Closure_WOCheckList_ClosingCost);

            }

            //ProjectInfo.UpdateProjectStatus(entity.ProjectId, FlowCode.Closure_WOCheckList, entity.GetProjectStatus(entity.Action));
            SaveCommers(entity, _action.ToString(), ProjectCommentStatus.Submit);

            return Ok(entity.Status);
        }

        [Route("api/ClosureWOCheckList/SaveClosureWOCheckList")]
        [HttpPost]
        public IHttpActionResult SaveClosureWOCheckList(ClosureWOCheckList entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                entity.CreateTime = DateTime.Now;
                entity.CreateUserAccount = ClientCookie.UserCode;
            }
            entity.Save();
            string action = string.IsNullOrEmpty(entity.Action) ?
                ProjectCommentAction.Submit : entity.Action;

            SaveCommers(entity, action, ProjectCommentStatus.Save);

            int result = _db.SaveChanges();
            return Ok(result);
        }

        private string GetCommers(ClosureWOCheckList entity)
        {
            string commers = string.Empty;
            if (entity.Id != new Guid() && !string.IsNullOrEmpty(ClientCookie.UserCode))
            {
                var list = _db.ProjectComment.Where(
                    c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Action == "Submit"
                         && c.RefTableName == ClosureWOCheckList.TableName && c.TitleCode == ClientCookie.TitleENUS && c.SourceCode == "Closure").ToList();

                if (list.Count > 0)
                {
                    commers = list[0].Content;
                }
            }
            return commers;
        }

        private void SaveCommers(ClosureWOCheckList entity, string action, ProjectCommentStatus status)
        {


            if (status == ProjectCommentStatus.Save)
            {

                var list = _db.ProjectComment.Where(c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == ProjectCommentStatus.Save && c.RefTableName == ClosureWOCheckList.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    if (!string.IsNullOrEmpty(entity.Comments))
                    {
                        closureCommens.Content = entity.Comments.Trim();
                    }
                    _db.ProjectComment.Attach(closureCommens);
                    _db.Entry(closureCommens).State = EntityState.Modified;
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }
            else
            {
                var list = _db.ProjectComment.Where(c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == ProjectCommentStatus.Save && c.RefTableName == ClosureWOCheckList.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    if (!string.IsNullOrEmpty(entity.Comments))
                    {
                        closureCommens.Content = entity.Comments.Trim();
                    }
                    closureCommens.Status = ProjectCommentStatus.Submit;
                    _db.ProjectComment.Attach(closureCommens);
                    _db.Entry(closureCommens).State = EntityState.Modified;
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }
            _db.SaveChanges();

        }

        private void AddProjectComment(ClosureWOCheckList entity, string action,
            ProjectCommentStatus status)
        {

            ProjectComment closureCommens = new ProjectComment();
            closureCommens.RefTableId = entity.Id;
            closureCommens.RefTableName = ClosureWOCheckList.TableName;

            closureCommens.TitleNameENUS = ClientCookie.TitleENUS;
            closureCommens.TitleNameZHCN = ClientCookie.TitleENUS;
            closureCommens.TitleCode = ClientCookie.TitleENUS;

            closureCommens.CreateTime = DateTime.Now;
            closureCommens.CreateUserAccount = ClientCookie.UserCode;

            closureCommens.UserAccount = ClientCookie.UserCode;
            closureCommens.UserNameENUS = ClientCookie.UserNameENUS;
            closureCommens.UserNameZHCN = ClientCookie.UserNameZHCN;
            closureCommens.CreateUserNameZHCN = entity.UserNameZHCN;
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

        [Route("api/ClosureWOCheckList/PostClosureWOCheckList")]
        [HttpPost]
        public IHttpActionResult PostClosureWOCheckList(ClosureWOCheckList entity)
        {
            string _debugInfo = string.Format("[Ln 571] Start Run PostClosureWOCheckList - Entity: {0}", JsonConvert.SerializeObject(entity));
            Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureWOCheckList");

            //ClosureInfo closure = new ClosureInfo();
            //closure = closure.GetByProjectId(entity.ProjectId.Value);

            var task = _db.TaskWork.First(
                e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                     && e.TypeCode == FlowCode.Closure_WOCheckList && e.RefID == entity.ProjectId
                );

            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_WOCheckList, task.RefID);
            task.RefTableName = ClosureWOCheckList.TableName;
            task.RefTableId = entity.Id;

            _db.TaskWork.Attach(task);
            _db.Entry(task).State = EntityState.Modified;

            int result = _db.SaveChanges();
            _debugInfo = string.Format("[Ln 611] Task:{0}", TaskWork.ConvertToJson(task));
            //DateTime.Parse()
            Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureWOCheckList");

            if (result > 0)
            {
                // Start K2 Process
                var _procCode = WFClosureWOCheckList.ProcessCode;
                var _listDataFields = new List<ProcessDataField>();

                _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                _listDataFields.Add(new ProcessDataField("dest_PMSupervisor", entity.PMSupervisorAccount));// 工程PM直属上级
                _listDataFields.Add(new ProcessDataField("dest_MCCLApprovers", string.Join(";", entity.MCCLApproverAccount, entity.MCCLITApproverAccount, entity.MCCLMCCLEqApproverAccount)));// MCCL Approvers
                _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));
                //_listDataFields.Add(new ProcessDataField("dest_Receiver", "")); 知会人

                //将WOCheckList的任务传给K2
                var taskJson = Newtonsoft.Json.JsonConvert.SerializeObject(task);
                _listDataFields.Add(new ProcessDataField("ProjectTaskInfo", taskJson));

                _debugInfo = string.Format("[Ln 630] DataFields: {0}", JsonConvert.SerializeObject(_listDataFields));
                Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureWOCheckList");

                var _procInstID = K2FxContext.Current.StartProcess(_procCode, ClientCookie.UserCode, _listDataFields);


                _debugInfo = string.Format("[Ln 637] ProcInstID: {0}", _procInstID);
                Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureWOCheckList");

                if (_procInstID > 0)
                {
                    // 更新 WOCheckList表的ProcInstId
                    entity.ProcInstID = _procInstID;
                    if (entity.Id == Guid.Empty)
                    {
                        entity.Id = Guid.NewGuid();
                        entity.CreateTime = DateTime.Now;
                        entity.CreateUserAccount = ClientCookie.UserCode;
                    }
                    entity.Save();

                    _db.Entry(entity).State = EntityState.Modified;
                    SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Submit);

                    result = _db.SaveChanges();
                }
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_WOCheckList,
                   NodeCode.Closure_WOCheckList_ClosingCost);
            }

            return Ok(result);
        }

        private void SetStatus(ClosureWOCheckList entity, WOCheckListStatus status)
        {
            int statusVal = (int)status;
            if (entity.Status < statusVal)
            {
                entity.Status = statusVal;
            }
        }

        //[Route("api/ClosureWOCheckList/UploadTemplate/{projectID}")]
        //[HttpPost]
        //public IHttpActionResult UploadTemplate(string projectid)
        //{


        //    HttpRequest request = System.Web.HttpContext.Current.Request;
        //    HttpFileCollection FileCollect = request.Files;
        //    int result = 0;
        //    string resultStr = string.Empty;
        //    string internalName = string.Empty;
        //    var woEntity = ClosureWOCheckList.Get(projectid);
        //    if (FileCollect.Count > 0) //如果集合的数量大于0
        //    {
        //        //用key获取单个文件对象HttpPostedFile
        //        HttpPostedFile fileSave = FileCollect[0];
        //        string fileName = Path.GetFileName(fileSave.FileName);
        //        string fileExtension = Path.GetExtension(fileSave.FileName);
        //        var current = System.Web.HttpContext.Current;

        //        internalName = Guid.NewGuid() + fileExtension;
        //        string filePath = current.Server.MapPath("~/") + "UploadFiles\\" + internalName;
        //        //通过此对象获取文件名
        //        fileSave.SaveAs(filePath);

        //        int column = 4;

        //        Attachment att = null;


        //        var wb = new HSSFWorkbook(new FileStream(filePath, FileMode.Open));
        //        var sheet = wb.GetSheet("PMT");

        //        var newRECost_WriteOFF = GetDecimalValue(sheet, 9, column);
        //        var newLHI_WriteOFF = GetDecimalValue(sheet, 10, column);
        //        var newESSD_Write_off = GetDecimalValue(sheet, 11, column);

        //        bool needCheckClosureTool = false;
        //        if (woEntity == null)
        //        {
        //            woEntity = new ClosureWOCheckList();
        //            woEntity.Id = Guid.NewGuid();
        //            woEntity.ProjectId = projectid;
        //            woEntity.IsHistory = false;
        //        }
        //        else
        //        {
        //            needCheckClosureTool = true;
        //        }



        //        woEntity.RE_Original = GetDecimalValue(sheet, 1, column);
        //        woEntity.LHI_Original = GetDecimalValue(sheet, 2, column);
        //        woEntity.ESSD_Original = GetDecimalValue(sheet, 3, column);
        //        woEntity.Equipment_Original = GetDecimalValue(sheet, 4, column);
        //        woEntity.Signage_Original = GetDecimalValue(sheet, 5, column);
        //        woEntity.Seating_Original = GetDecimalValue(sheet, 6, column);
        //        woEntity.Decoration_Original = GetDecimalValue(sheet, 7, column);
        //        woEntity.RE_NBV = GetDecimalValue(sheet, 8, column);
        //        woEntity.LHI_NBV = GetDecimalValue(sheet, 9, column);
        //        woEntity.ESSD_NBV = GetDecimalValue(sheet, 10, column);

        //        woEntity.Equipment_NBV = GetDecimalValue(sheet, 11, column);
        //        woEntity.Signage_NBV = GetDecimalValue(sheet, 12, column);
        //        woEntity.Seating_NBV = GetDecimalValue(sheet, 13, column);
        //        woEntity.Decoration_NBV = GetDecimalValue(sheet, 14, column);
        //        woEntity.EquipmentTransfer = GetDecimalValue(sheet, 15, column);

        //        woEntity.TotalCost_Original = GetDecimalValue(sheet, 16, column);
        //        woEntity.TotalCost_NBV = GetDecimalValue(sheet, 17, column);

        //        woEntity.TotalCost_WriteOFF = GetDecimalValue(sheet, 18, column);
        //        woEntity.RECost_WriteOFF = GetDecimalValue(sheet, 19, column);
        //        woEntity.LHI_WriteOFF = GetDecimalValue(sheet, 20, column);
        //        woEntity.ESSD_WriteOFF = GetDecimalValue(sheet, 21, column);

        //        woEntity.Equipment_WriteOFF = GetDecimalValue(sheet, 22, column);
        //        woEntity.Signage_WriteOFF = GetDecimalValue(sheet, 23, column);

        //        woEntity.Seating_WriteOFF = GetDecimalValue(sheet, 24, column);
        //        woEntity.Decoration_WriteOFF = GetDecimalValue(sheet, 25, column);
        //        woEntity.ClosingCost = GetDecimalValue(sheet, 26, column);

        //        woEntity.Save();

        //        if (needCheckClosureTool)
        //        {
        //            var closureToolHandler = new ClosureTool();

        //            var oldRECost_WriteOFF = woEntity.RECost_WriteOFF;
        //            var oldLHI_Write_off = woEntity.LHI_WriteOFF;
        //            var oldESSD_Write_off = woEntity.ESSD_WriteOFF;

        //            if ((newRECost_WriteOFF != oldRECost_WriteOFF) || (newLHI_WriteOFF != oldLHI_Write_off) || (newESSD_Write_off != oldESSD_Write_off))
        //            {
        //                var closureToolController = new ClosureToolController();
        //                var toolEntity = ClosureTool.Get(projectid);
        //                if (toolEntity != null)
        //                {
        //                    //判断是否满足生成closureTools的条件
        //                    var errMsg = toolEntity.EnableGenClosureTool(toolEntity.Id);
        //                    if (!string.IsNullOrEmpty(errMsg))
        //                    {

        //                        closureToolController.GenClosureTool(toolEntity.Id, toolEntity.UserAccount, toolEntity.UserNameZHCN, toolEntity.UserNameENUS);

        //                        //通知Finance Specialist和Asset Actor
        //                    }
        //                }
        //            }
        //        }

        //        att = new Attachment();
        //        att.RefTableName = ClosureWOCheckList.TableName;
        //        att.RefTableID = woEntity.Id.ToString();
        //        att.CreateTime = DateTime.Now;
        //        att.InternalName = internalName;
        //        att.Name = fileName;
        //        att.Extension = fileExtension;
        //        att.Length = FileCollect[0].ContentLength;
        //        att.TypeCode = "Template";
        //        att.CreatorID = ClientCookie.UserCode;
        //        att.CreatorNameENUS = ClientCookie.UserNameENUS;
        //        att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
        //        att.RelativePath = "//";

        //        Attachment.SaveSigleFile(att);


        //        var currentNode = NodeInfo.GetCurrentNode(projectid, FlowCode.Closure_WOCheckList);
        //        var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_WOCheckList_ClosingCost);
        //        if (newNode.Sequence > currentNode.Sequence)
        //        {
        //            ProjectInfo.UpdateProjectNode(projectid, FlowCode.Closure_WOCheckList, NodeCode.Closure_WOCheckList_ClosingCost);
        //        }


        //        result = _db.SaveChanges();

        //        resultStr = SiteInfo.WebUrl + "UploadFiles/" + internalName;
        //    }
        //    return Ok(woEntity.Status);
        //}

        //根据ProcInstID获取数据
        [Route("api/ClosureWOCheckList/GetByProcInstID/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetByProcInstID(int procInstID)
        {
            var entity = _db.ClosureWOCheckList.First(e => e.ProcInstID == procInstID);
            return Ok(entity);
        }

        private decimal GetDecimalValue(ISheet sheet, int row, int column)
        {
            var cell = sheet.GetRow(row).GetCell(column);
            string str = string.Empty;
            try
            {
                str = cell.StringCellValue;
            }
            catch (Exception e)
            {
                str = cell.NumericCellValue.ToString();
            }
            decimal result = 0;
            if (!string.IsNullOrEmpty(str))
            {
                result = decimal.Parse(str);
            }
            return result;
        }



        [Route("api/ClosureWOCheckList/GetK2Status/{userAccount}/{sn}/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetK2Status(string userAccount, string sn, string procInstID)
        {
            // Load K2 Process
            bool result = false;
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, userAccount);
            result = !resultStr.Equals(Workflows.Closure.WFClosureWOCheckList.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

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

    }
}
