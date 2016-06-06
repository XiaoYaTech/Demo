using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Workflows;
using Mcdonalds.AM.Services.Workflows.Closure;
using Mcdonalds.AM.Services.Workflows.Enums;
using MyExcel.NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Common.Excel;

namespace Mcdonalds.AM.Services.Controllers.Closure
{
    public class ConsInvtCheckingController : ApiController
    {
        public string ConsInvtCheckingVersion = "CIC1.0.0";      //ConsInvtChecking模板版本号

        [Route("api/ClosureConsInvtChecking/GetById/{id}")]
        [HttpGet]
        public IHttpActionResult GetById(Guid id)
        {
            var entity = ClosureConsInvtChecking.FirstOrDefault(e => e.Id == id);

            return Ok(entity);
        }

        [Route("api/ClosureConsInvtChecking/GetByProjectId/{projectID}")]
        [HttpGet]
        public IHttpActionResult GetByProjectId(string projectId)
        {
            var entity = ClosureConsInvtChecking.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            if (entity != null)
            {
                var closureInfo = ClosureInfo.GetByProjectId(projectId);
                entity.USCode = closureInfo.USCode;
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

                if (string.IsNullOrEmpty(entity.PMSupervisor))
                {
                    var puser = ProjectUsers.FirstOrDefault(i => i.ProjectId == entity.ProjectId && i.RoleCode == ProjectUserRoleCode.CM);
                    if (puser != null)
                        entity.PMSupervisor = puser.UserAccount;
                }
            }

            return Ok(entity);
        }

        [Route("api/ClosureConsInvtChecking/DownLoadTemplate/{projectID}")]
        [HttpGet]
        public IHttpActionResult DownLoadTemplate(string projectId)
        {

            var current = System.Web.HttpContext.Current;
            //string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.FAWrite_offTool_Template;


            //Excel excel = new Excel();
            //string templatePath = path;

            //string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + Guid.NewGuid() + ".xls";

            //var closure = ClosureInfo.GetByProjectId(projectId);
            //excel.Open(path);
            //excel.Save(tempFilePath);

            //var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == closure.USCode);


            //Excel tempExcel = new Excel();
            //tempExcel.Open(tempFilePath);
            //var sheet = tempExcel.Sheets["PMT"];

            //sheet.Cells[1, 1].StrValue = store.RegionENUS;
            //sheet.Cells[2, 1].StrValue = store.MarketENUS;
            //sheet.Cells[3, 1].StrValue = store.NameZHCN;
            //sheet.Cells[4, 1].StrValue = closure.USCode;
            //sheet.Cells[5, 1].StrValue = store.StoreTypeName;
            //if (closure.ActualCloseDate != null)
            //{
            //    sheet.Cells[6, 1].StrValue = closure.ActualCloseDate.Value.ToString("yyyy-MM-dd");
            //}
            //sheet.Cells[7, 1].StrValue = "";//TODO::-Cary  GetPM

            //sheet.Cells[0, 7].StrValue = ConsInvtCheckingVersion;

            //tempExcel.Save(tempFilePath);


            //直接下载writeoff文件
            McdAMEntities _db = new McdAMEntities();
            var closureInfo = ClosureInfo.GetByProjectId(projectId);
            var wo = ClosureWOCheckList.Get(projectId);
            if (wo != null)
            {
                var att = _db.Attachment.FirstOrDefault(a => a.RefTableID == wo.Id.ToString() && a.TypeCode == "Template");

                string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + att.InternalName;
                string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + Guid.NewGuid() + Path.GetExtension(absolutePath);

                File.Copy(absolutePath, tempFilePath);
                ExcelHelper.UpdateExcelVersionNumber(tempFilePath, ConsInvtCheckingVersion);

                var currentNode = NodeInfo.GetCurrentNode(projectId, FlowCode.Closure_ConsInvtChecking);
                var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_ConsInvtChecking_DownLoadTemplate);
                if (newNode.Sequence > currentNode.Sequence)
                {
                    ProjectInfo.FinishNode(projectId, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_DownLoadTemplate);
                }

                current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + SiteFilePath.GetTemplateFileName(closureInfo.USCode, FlowCode.Closure_ConsInvtChecking, SiteFilePath.FAWrite_offTool_Template));
                current.Response.ContentType = "application/octet-stream";
                current.Response.WriteFile("" + tempFilePath + "");
                current.Response.End();
            }
            else
            {
                string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.FAWrite_offTool_Template;
                current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + SiteFilePath.GetTemplateFileName(closureInfo.USCode, FlowCode.Closure_ConsInvtChecking, SiteFilePath.FAWrite_offTool_Template));
                current.Response.ContentType = "application/octet-stream";
                current.Response.WriteFile("" + path + "");
                current.Response.End();
            }
            return Ok();


        }

        [Route("api/ClosureConsInvtChecking/GetTemplates/{id}")]
        [HttpGet]
        public IHttpActionResult GetTemplates(Guid id)
        {
            var list = Attachment.GetList(ClosureConsInvtChecking.TableName, id.ToString(), "Template");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName; 
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }

            return Ok(list);
        }

        [Route("api/ClosureConsInvtChecking/UploadTemplate/{projectID}")]
        [HttpPost]
        public IHttpActionResult UploadTemplate(string projectid)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            string resultStr = string.Empty;
            string internalName = string.Empty;

            ClosureConsInvtChecking woEntity = null;
            bool isNew = false;
            if (FileCollect.Count > 0) //如果集合的数量大于0
            {
                //用key获取单个文件对象HttpPostedFile
                HttpPostedFile fileSave = FileCollect[0];
                string fileName = Path.GetFileName(fileSave.FileName);
                string fileExtension = Path.GetExtension(fileSave.FileName);
                var current = System.Web.HttpContext.Current;

                internalName = Guid.NewGuid() + fileExtension;
                string filePath = current.Server.MapPath("~/") + "UploadFiles\\" + internalName;
                //通过此对象获取文件名

                fileSave.SaveAs(filePath);

                string column = "E";
                woEntity = ClosureConsInvtChecking.Get(projectid);

                var fileInfo = new FileInfo(filePath);
                //验证上传EXCEL版本  
                var vn = ExcelHelper.GetExcelVersionNumber(filePath);
                if (vn != ConsInvtCheckingVersion)
                    PluploadHandler.WriteErrorMsg("上传附件版本号不一致");

                var importDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.ClosureWOCheckList);

                if (woEntity == null)
                {
                    isNew = true;
                    woEntity = new ClosureConsInvtChecking();
                    woEntity.ProjectId = projectid;
                    woEntity.Id = Guid.NewGuid();
                    woEntity.CreateTime = DateTime.Now;
                    woEntity.CreateUserAccount = ClientCookie.UserCode;

                    woEntity.IsHistory = false;
                }

                woEntity.TotalOriginalBudget = GetDecimalValue(importDirector, 16, column);
                woEntity.TotalNBVBudget = GetDecimalValue(importDirector, 17, column);
                woEntity.TotalWriteoffBudget = GetDecimalValue(importDirector, 18, column);
                var woCheckList = ClosureWOCheckList.Get(projectid);
                if (woCheckList != null)
                {
                    if (!EqualValue(woCheckList.TotalCost_Original, woEntity.TotalOriginalBudget))
                    {
                        PluploadHandler.WriteErrorMsg("Total Budget 数据验证不通过，请检查后重试！");
                    }

                    if (!EqualValue(woCheckList.TotalCost_NBV, woEntity.TotalNBVBudget))
                    {
                        PluploadHandler.WriteErrorMsg("Total NBV(Closure Data) 数据验证不通过，请检查后重试！");
                    }

                    if (!EqualValue(woCheckList.TotalCost_WriteOFF, woEntity.TotalWriteoffBudget))
                    {
                        PluploadHandler.WriteErrorMsg("Total Write off 数据验证不通过，请检查后重试！");
                    }
                }
                woEntity.RECostBudget = GetDecimalValue(importDirector, 19, column);
                woEntity.LHIBudget = GetDecimalValue(importDirector, 20, column);
                woEntity.ESSDBudget = GetDecimalValue(importDirector, 21, column);
                woEntity.EquipmentBudget = GetDecimalValue(importDirector, 22, column);
                woEntity.SignageBudget = GetDecimalValue(importDirector, 23, column);
                woEntity.SeatingBudget = GetDecimalValue(importDirector, 24, column);
                woEntity.DecorationBudget = GetDecimalValue(importDirector, 25, column);
                woEntity.ClosingCostBudegt = GetDecimalValue(importDirector, 26, column);

                woEntity.ClosingCostActual = GetDecimalValue(importDirector, 27, column);
                woEntity.TotalActual = GetDecimalValue(importDirector, 28, column);

                woEntity.RECostActual = GetDecimalValue(importDirector, 29, column);
                woEntity.LHIActual = GetDecimalValue(importDirector, 30, column);
                woEntity.EquipmentActual = GetDecimalValue(importDirector, 31, column);
                woEntity.SignageActual = GetDecimalValue(importDirector, 32, column);
                woEntity.SeatingActual = GetDecimalValue(importDirector, 33, column);
                woEntity.DecorationActual = GetDecimalValue(importDirector, 34, column);
                woEntity.RECostVsBudget = GetDecimalValue(importDirector, 35, column);
                woEntity.LHIWOVsBudget = GetDecimalValue(importDirector, 36, column);

                woEntity.ESSDWOVsBudget = GetDecimalValue(importDirector, 37, column);
                woEntity.TTLWOVsBudget = GetDecimalValue(importDirector, 38, column);
                woEntity.REExplanation = GetStringValue(importDirector, 39, column);
                woEntity.LHIExplanation = GetStringValue(importDirector, 40, column);
                woEntity.ESSDExplanation = GetStringValue(importDirector, 41, column);
                woEntity.TotalExplanation = GetStringValue(importDirector, 42, column);


                Attachment att = Attachment.GetAttachment("ClosureConsInvtChecking", woEntity.Id.ToString(), "Template");
                if (att == null)
                {
                    att = new Attachment();
                    att.ID = Guid.NewGuid();
                    att.TypeCode = "Template";
                    att.RefTableName = ClosureConsInvtChecking.TableName;
                    att.RefTableID = woEntity.Id.ToString();
                    att.RelativePath = "//";
                }
                att.InternalName = internalName;
                att.Name = fileName;
                att.Extension = fileExtension;
                att.Length = FileCollect[0].ContentLength;
                att.CreateTime = DateTime.Now;
                att.CreatorID = ClientCookie.UserCode;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                Attachment.SaveSigleFile(att);

                var currentNode = NodeInfo.GetCurrentNode(projectid, FlowCode.Closure_ConsInvtChecking);
                var newNode = NodeInfo.GetNodeInfo(NodeCode.Closure_ConsInvtChecking_ResultUpload);
                if (newNode.Sequence > currentNode.Sequence)
                {
                    ProjectInfo.FinishNode(projectid, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_DownLoadTemplate);
                    ProjectInfo.FinishNode(projectid, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_WriteOffData);
                    ProjectInfo.FinishNode(projectid, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_ResultUpload);
                }

                if (isNew)
                {
                    ClosureConsInvtChecking.Add(woEntity);
                }
                else
                {
                    ClosureConsInvtChecking.Update(woEntity);
                }
            }
            return Ok(woEntity);
        }

        private bool EqualValue(decimal? val1, decimal? val2)
        {
            if (val1.HasValue && val2.HasValue)
            {
                return Math.Round(val1.Value, 2) == Math.Round(val2.Value, 2);
            }
            else if (!val1.HasValue && !val2.HasValue)
                return true;
            return false;
        }

        private string GetStringValue(ExcelDataImportDirector excel, int row, string column)
        {
            //历史版本从0开始，这里+1修正
            var str = excel.GetCellValue(row + 1, column);
            return str;
        }

        private decimal GetDecimalValue(ExcelDataImportDirector excel, int row, string column)
        {
            //历史版本从0开始，这里+1修正
            var str = excel.GetCellValue(row + 1, column);
            decimal result = 0;
            try
            {
                if (str != null)
                    result = decimal.Parse(str);
            }
            catch
            {
                PluploadHandler.WriteErrorMsg("Sheet(PMT)中第" + (row + 1) + "行数据格式有误，只能输入数字类型的数据！");
            }
            return result;
        }


        [Route("api/ClosureConsInvtChecking/ProcessClosureConsInvtChecking")]
        [HttpPost]
        public IHttpActionResult ProcessClosureConsInvtChecking(ClosureConsInvtChecking entity)
        {
            int procInstID = entity.ProcInstID.Value;

            string actionLower = entity.Action.ToLower();
            string account = ClientCookie.UserCode;
            //评论信息
            string comments = entity.Comments;


            // To-Do K2 action
            ProcessActionResult _action = BPMHelper.ConvertToProcAction(actionLower);

            List<ProcessDataField> _listDataFields = new List<ProcessDataField>();
            if (entity.Action == "ReSubmit")
            {
                var _diff = CalDiff(entity);
                _listDataFields.Add(new ProcessDataField("flag_DiffRangeType", _diff)); // 1, 2, 3

                //_listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                _listDataFields.Add(new ProcessDataField("dest_Creator", entity.CreateUserAccount)); // 发起人也变成工程PM，不是actor
                _listDataFields.Add(new ProcessDataField("dest_EngPM", entity.PMSupervisor));
                _listDataFields.Add(new ProcessDataField("dest_Fin", entity.FinControllerAccount));
                _listDataFields.Add(new ProcessDataField("dest_VPGM", entity.VPGMAccount));
                _listDataFields.Add(new ProcessDataField("dest_Group1", entity.FinanceAccount + ";" + entity.PMSupervisor));
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments, _listDataFields);
            }
            else
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments);

            if (actionLower.Equals(ProjectAction.Return, StringComparison.CurrentCultureIgnoreCase))
            {
                TaskWork.Finish(e => e.RefID == entity.ProjectId
                    && e.TypeCode == FlowCode.Closure_ConsInvtChecking
                    && e.Status == TaskWorkStatus.UnFinish
                    && e.K2SN != entity.SN);
            }

            if (actionLower == "resubmit")
            {
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ConsInvtChecking, NodeCode.Closure_ConsInvtChecking_WriteOffData);
            }
            else if (actionLower == "return")
            {
                //ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ConsInvtChecking);
            }

            SaveCommers(entity, _action.ToString(), ProjectCommentStatus.Submit);


            return Ok();
        }

        [Route("api/ClosureConsInvtChecking/Save")]
        [HttpPost]
        public IHttpActionResult Save(ClosureConsInvtChecking entity)
        {
            entity.Save();

            string action = string.IsNullOrEmpty(entity.Action) ?
                ProjectCommentAction.Submit : entity.Action;


            SaveCommers(entity, action, ProjectCommentStatus.Save);
            return Ok();
        }

        [Route("api/ClosureConsInvtChecking/PostClosureConsInvtChecking")]
        [HttpPost]
        public IHttpActionResult PostClosureConsInvtChecking(ClosureConsInvtChecking entity)
        {

            //ClosureInfo closure = new ClosureInfo();
            //closure = closure.GetByProjectId(entity.ProjectId.Value);

            var task = TaskWork.FirstOrDefault(
                e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure
                     && e.TypeCode == FlowCode.Closure_ConsInvtChecking && e.RefID == entity.ProjectId);
            task.Finish();
            // Start K2 Process
            string _procCode = WFConsInvtChecking.ProcessCode;

            List<ProcessDataField> _listDataFields = new List<ProcessDataField>();




            var _diff = CalDiff(entity); ;

            /*
             Total Variance <= +-5%   :  1
                +-5%  < Total Variance <= +- 10%   :   2
                Total Variance > +- 10% :  3
             */

            var closureEntity = ClosureInfo.GetByProjectId(entity.ProjectId);
            _listDataFields.Add(new ProcessDataField("flag_DiffRangeType", _diff)); //传偏差值范围 1, 2, 3

            //_listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
            _listDataFields.Add(new ProcessDataField("dest_Creator", entity.CreateUserAccount)); // 发起人也变成工程PM，不是actor
            _listDataFields.Add(new ProcessDataField("dest_EngPM", entity.PMSupervisor));
            _listDataFields.Add(new ProcessDataField("dest_Fin", entity.FinControllerAccount));
            _listDataFields.Add(new ProcessDataField("dest_VPGM", entity.VPGMAccount));
            _listDataFields.Add(new ProcessDataField("dest_Group1", entity.FinanceAccount + ";" + entity.PMSupervisor));
            _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));
            //_listDataFields.Add(new ProcessDataField("dest_Receiver", "")); 知会人

            //将 WFConsInvtChecking 的任务传给K2
            var taskJson = Newtonsoft.Json.JsonConvert.SerializeObject(task);
            _listDataFields.Add(new ProcessDataField("ProjectTaskInfo", taskJson));
            int _procInstID = K2FxContext.Current.StartProcess(_procCode, ClientCookie.UserCode, _listDataFields);


            if (_procInstID > 0)
            {
                // 更新 ConsInvtChecking 表的ProcInstId
                entity.ProcInstID = _procInstID;
                ClosureConsInvtChecking.Update(entity);

                SaveCommers(entity, ProjectAction.Submit, ProjectCommentStatus.Submit);
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ConsInvtChecking,
                    NodeCode.Closure_ConsInvtChecking_ResultUpload);
            }
            return Ok();
        }

        [Route("api/ClosureConsInvtChecking/GetDiff/{projectId}")]
        public IHttpActionResult GetDiff(string projectId)
        {
            var entity = ClosureConsInvtChecking.Get(projectId);
            var diff = CalDiff(entity);
            return Ok(new
            {
                Result = diff
            });
        }

        private string CalDiff(ClosureConsInvtChecking entity)
        {
            var _diff = string.Empty;
            decimal totalVariance = 0;
            if (entity.TotalOriginalBudget != 0)
                totalVariance = (entity.TotalActual.Value - entity.TotalWriteoffBudget.Value) / entity.TotalWriteoffBudget.Value * 100;
            if (totalVariance >= 0)
            {
                if (totalVariance >= 10)
                {
                    _diff = "3";
                }
                else if (totalVariance >= 5)
                {
                    _diff = "2";
                }
                else
                {
                    _diff = "1";
                }
            }
            else
            {
                if (totalVariance <= -10)
                {
                    _diff = "3";
                }
                else if (totalVariance <= -5)
                {
                    _diff = "2";
                }
                else
                {
                    _diff = "1";
                }
            }
            return _diff;
        }

        private void SaveCommers(ClosureConsInvtChecking entity, string action, ProjectCommentStatus status)
        {


            if (status == ProjectCommentStatus.Save)
            {

                var list = ProjectComment.Search(
                 c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Action == action
                      && c.RefTableName == ClosureConsInvtChecking.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    closureCommens.Content = entity.Comments.Trim();
                    ProjectComment.Update(closureCommens);
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }
            else
            {
                AddProjectComment(entity, action, status);
            }
        }

        private void AddProjectComment(ClosureConsInvtChecking entity, string action,
            ProjectCommentStatus status)
        {
            ProjectComment closureCommens = new ProjectComment();
            closureCommens.RefTableId = entity.Id;
            closureCommens.RefTableName = ClosureConsInvtChecking.TableName;

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


        [Route("api/ClosureConsInvtChecking/GetK2Status/{userAccount}/{sn}/{procInstID}")]
        [HttpGet]
        public IHttpActionResult GetK2Status(string userAccount, string sn, string procInstID)
        {
            // Load K2 Process
            bool result = false;
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, userAccount);
            result = !resultStr.Equals(WFConsInvtChecking.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

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

        [Route("api/ClosureConsInvtChecking/Edit")]
        [HttpPost]
        public IHttpActionResult Edit(ClosureConsInvtChecking entity)
        {
            //ModifyProject(entity, ProjectAction.Edit);
            var result = string.Empty;
            using (TransactionScope tranScope = new TransactionScope())
            {
                result = entity.Edit();
                tranScope.Complete();
            }
            return Ok(new
            {
                TaskUrl = result
            });
        }

        [Route("api/ClosureConsInvtChecking/Recall")]
        [HttpPost]
        public IHttpActionResult Recall(ClosureConsInvtChecking entity)
        {
            //
            bool _recallSuccess = false;
            if (entity.ProcInstID != null)
            {
                _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value, WFConsInvtChecking.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, entity.Comments);
                if (_recallSuccess)
                {


                    SaveCommers(entity, ProjectCommentAction.Recall, ProjectCommentStatus.Submit);
                }
            }

            if (!_recallSuccess)
            {
                throw new Exception("Recall失败");
            }

            //ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ConsInvtChecking, ProjectStatus.Recalled);
            ProjectInfo.UpdateProjectStatus(entity.ProjectId, FlowCode.Closure_ConsInvtChecking, ProjectStatus.Recalled);

            return Ok();
        }


        private void ModifyProject(ClosureConsInvtChecking entity, string action)
        {
            // 2014-08-05 victor.huang: Recall 或Edit 后不需要重新再生成Task，不然会多生成一条冗余记录
            //TaskWorkCondition condition = new TaskWorkCondition();
            //condition.ProjectId = entity.ProjectId;
            //condition.Url = "/closure/Main#/closure/WOCheckList/" + entity.ProjectId;
            //condition.UserAccount = ClientCookie.UserCode;
            //condition.UserNameENUS = entity.UserNameENUS;
            //condition.UserNameZHCN = entity.UserNameZHCN;

            //taskWorkBll.ReSendTaskWork(condition);
            if (action == ProjectAction.Edit)
            {
                entity.IsHistory = true;
                entity.LastUpdateTime = DateTime.Now;
                entity.Update();
                var objectCopy = new ObjectCopy();
                var newWo = objectCopy.AutoCopy(entity);
                newWo.Id = Guid.NewGuid();
                ClosureConsInvtChecking.Add(newWo);

                var projectEntity = ProjectInfo.Get(entity.ProjectId, FlowCode.Closure_ConsInvtChecking);
                projectEntity.Status = ProjectStatus.UnFinish;
                ProjectInfo.Update(projectEntity);
            }
            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ConsInvtChecking);
        }

    }
}
