using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;
using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Enums;
using Mcdonalds.AM.Services.Common;
using System.Transactions;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers.Rebuild
{
    public class RebuildController : ApiController
    {
        [Route("api/Rebuild/BeginCreate")]
        [HttpPost]
        public IHttpActionResult BeginCreate(RebuildInfo rebuild)
        {
            try
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    rebuild.Id = Guid.NewGuid();
                    rebuild.ProjectId = ProjectInfo.CreateMainProject(FlowCode.Rebuild, rebuild.USCode, NodeCode.Start, rebuild.CreateUserAccount);
                    rebuild.CreateTime = DateTime.Now;
                    RebuildInfo.Add(rebuild);
                    rebuild.AddProjectUsers();
                    rebuild.SendRemind();
                    rebuild.SendWorkTask();
                    rebuild.CreateSubProject();
                    ProjectNode.GenerateOnCreate(FlowCode.Rebuild, rebuild.ProjectId);
                    tran.Complete();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Rebuild/GetRebuildInfo")]
        public IHttpActionResult GetRebuildInfo(string projectId)
        {
            var rebuildInfo = RebuildInfo.Search(e => e.ProjectId == projectId).FirstOrDefault();
            //if (rebuildInfo != null && rebuildInfo.ReopenDate.HasValue)
            //    rebuildInfo.ReopenedDays = (rebuildInfo.ReopenDate.Value - DateTime.Now).Days;
            return Ok(rebuildInfo);
        }

        [HttpGet]
        [Route("api/Rebuild/GetProjectId")]
        public IHttpActionResult GetProjectId(int procInstId)
        {
            var task = RebuildLegalReview.Search(e => e.ProcInstID.Value.Equals(procInstId)).FirstOrDefault();
            return Ok(task);
        }

        [Route("api/Rebuild/Update")]
        [HttpPost]
        public IHttpActionResult Update(RebuildInfo rebuild)
        {
            rebuild.UpdateFromProjectList();
            return Ok();
        }
        //[Route("api/Rebuild/GetNavTop")]
        //[HttpGet]
        //public IHttpActionResult GetNavTop(string projectId, string userCode)
        //{
        //    var pjUserBll = new ProjectUsers();
        //    var navInfo = new NavInfo();
        //    var Navs = new
        //    {
        //        NavInfos = navInfo.GetNavInfos(FlowCode.Rebuild, projectId, userCode).OrderBy(e => e.Disq),
        //        UserList = pjUserBll.GetProjectUserList(userCode, projectId).ToList(),
        //        FinishedTaskList = ProjectInfo.GetFinishedProject(projectId).ToList()
        //    };
        //    return Ok(Navs);
        //}

        #region Attachment

        [HttpPost]
        [Route("api/Rebuild/{typeCode}/UploadContract/{projectId}")]
        public IHttpActionResult UploadContract(string typeCode, string projectId)
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                HttpFileCollection fileCollect = request.Files;
                string internalName = string.Empty;
                string strFlowCode = "";
                string strNodeCode = "";
                if (fileCollect.Count > 0) //如果集合的数量大于0
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        string strEntityId = "";
                        string strRefTableName = "";
                        string strTypeCode = "";
                        if (typeCode == "LegalReview"
                            || typeCode == "LegalClearanceReport"
                            || typeCode == "Agreement"
                            || typeCode == "Others")
                        {
                            var entity = RebuildLegalReview.GetLegalReviewInfo(projectId, "");
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new RebuildLegalReview();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                RebuildLegalReview.Add(entity);
                            }
                            strNodeCode = typeCode == "Agreement" ? NodeCode.Rebuild_LegalReview_Confirm : NodeCode.Rebuild_LegalReview_Upload;
                            strFlowCode = FlowCode.Rebuild_LegalReview;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "RebuildLegalReview";
                            if (typeCode == "LegalReview")
                                strTypeCode = "Contract";
                            else if (typeCode == "LegalClearanceReport")
                                strTypeCode = "LegalClearanceReport";
                            else if (typeCode == "Agreement")
                                strTypeCode = "Agreement";
                            else if (typeCode == "Others")
                                strTypeCode = "Others";
                        }
                        else if (typeCode == "FinancAnalysisAttach"
                            || typeCode == "FinanceAnalysis")
                        {
                            var finance = new RebuildFinancAnalysis();
                            var entity = finance.GetFinanceInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new RebuildFinancAnalysis();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                RebuildFinancAnalysis.Add(entity);
                            }
                            strFlowCode = FlowCode.Rebuild_FinanceAnalysis;
                            strNodeCode = NodeCode.Rebuild_FinanceAnalysis_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "RebuildFinancAnalysis";
                            if (typeCode == "FinancAnalysisAttach")
                                strTypeCode = "Attachment";
                            else if (typeCode == "FinanceAnalysis")
                                strTypeCode = "FinAgreement";
                        }
                        else if (typeCode == "ConsInfoAttach"
                            || typeCode == "ConsInfo")
                        {
                            var consinfo = new RebuildConsInfo();
                            var entity = consinfo.GetConsInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new RebuildConsInfo();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                RebuildConsInfo.Add(entity);
                            }
                            strFlowCode = FlowCode.Rebuild_ConsInfo;
                            strNodeCode = NodeCode.Rebuild_ConsInfo_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "RebuildConsInfo";
                            if (typeCode == "ConsInfoAttach")
                            {
                                strTypeCode = "Attachment";
                            }
                            else if (typeCode == "ConsInfo")
                            {
                                strTypeCode = "ConsInfoAgreement";
                            }
                        }
                        else if (typeCode == "SignedApproval"
                            || typeCode == "SignedAgreement")
                        {
                            var entity = RebuildPackage.GetRebuildPackageInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new RebuildPackage();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                RebuildPackage.Add(entity);
                            }
                            strTypeCode = typeCode;
                            strFlowCode = FlowCode.Rebuild_Package;
                            strNodeCode = NodeCode.Rebuild_Package_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "RebuildPackage";
                        }


                        List<Attachment> listAtt = new List<Attachment>();
                        for (int i = 0; i < fileCollect.Count; i++)
                        {
                            //用key获取单个文件对象HttpPostedFile
                            HttpPostedFile fileSave = fileCollect[i];
                            string fileName = Path.GetFileName(fileSave.FileName);
                            string fileExtension = Path.GetExtension(fileSave.FileName);
                            var current = HttpContext.Current;

                            internalName = Guid.NewGuid() + fileExtension;
                            string absolutePath = current.Server.MapPath("~/") + "UploadFiles/" + internalName;
                            //通过此对象获取文件名

                            fileSave.SaveAs(absolutePath);
                            Attachment att = new Attachment();
                            att.InternalName = internalName;
                            att.RefTableName = strRefTableName;
                            att.RefTableID = strEntityId;
                            att.RelativePath = "//";
                            att.Name = fileName;
                            att.Extension = fileExtension;
                            att.Length = fileCollect[i].ContentLength;
                            att.CreateTime = DateTime.Now;
                            att.ID = Guid.NewGuid();
                            att.TypeCode = strTypeCode;
                            att.CreatorID = ClientCookie.UserCode;
                            att.CreatorNameENUS = ClientCookie.UserNameENUS;
                            att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                            listAtt.Add(att);
                        }
                        if (strTypeCode == "Attachment"
                            || strTypeCode == "Contract")
                            Attachment.AddList(listAtt);
                        else
                        {
                            if (listAtt.Count > 0)
                            {
                                Attachment.SaveSigleFile(listAtt[0]);
                                var currentNode = NodeInfo.GetCurrentNode(projectId, strFlowCode);
                                var newNode = NodeInfo.GetNodeInfo(strNodeCode);
                                if (newNode.Sequence > currentNode.Sequence)
                                {
                                    ProjectInfo.FinishNode(projectId, strFlowCode, strNodeCode);
                                }
                            }
                        }
                        scope.Complete();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Rebuild/DeleteAttachement")]
        public IHttpActionResult DeleteAttachement(string flowCode, string projectId, Guid attId)
        {
            if (flowCode == "LegalReview")
                ProjectInfo.UnFinishNode(projectId, FlowCode.Rebuild_LegalReview, NodeCode.Rebuild_LegalReview_Upload);
            else if (flowCode == "FinanceAnalysis")
                ProjectInfo.UnFinishNode(projectId, FlowCode.Rebuild_FinanceAnalysis, NodeCode.Rebuild_FinanceAnalysis_Upload);
            else if (flowCode == "Consinfo")
                ProjectInfo.UnFinishNode(projectId, FlowCode.Rebuild_ConsInfo, NodeCode.Rebuild_ConsInfo_Upload);
            Attachment.Delete(attId);
            return Ok();
        }

        #endregion

        #region LegalReview
        [HttpGet]
        [Route("api/Rebuild/LegalReview/GetLegalReview")]
        public IHttpActionResult GetLegalReview(string projectId, string entityId = "")
        {
            var entity = RebuildLegalReview.GetLegalReviewInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Rebuild;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "RebuildLegalReview";

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.ProjectComments = comments;

                    var saveComment =
                        comments.OrderByDescending(e => e.CreateTime)
                            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                    if (saveComment != null)
                    {
                        entity.Comments = saveComment.Content;
                    }
                }
                return Ok(entity);
            }
            else
                return Ok("NULL");
        }

        [HttpGet]
        [Route("api/Rebuild/LegalReview/GetLegalContractList")]
        public IHttpActionResult GetLegalContractList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("RebuildLegalReview", refTableId, typeCode);
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/SaveLegalReview")]
        public IHttpActionResult SaveLegalReview(RebuildLegalReview legalReview)
        {
            try
            {
                legalReview.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/SubmitLegalReview")]
        public IHttpActionResult SubmitLegalReview(RebuildLegalReview legalReview)
        {
            legalReview.Submit();
            return Ok();
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/RecallLegalReview")]
        public IHttpActionResult RecallLegalReview(RebuildLegalReview legalReview)
        {
            legalReview.Recall(legalReview.Comments);
            return Ok();
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/EditLegalReview")]
        public IHttpActionResult EditLegalReview(RebuildLegalReview legalReview)
        {
            var taskUrl = legalReview.Edit();

            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/ResubmitLegalReview")]
        public IHttpActionResult ResubmitLegalReview(RebuildLegalReview entity)
        {
            entity.ResubmitLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/ApproveLegalReview")]
        public IHttpActionResult ApproveLegalReview(RebuildLegalReview entity)
        {
            entity.ApproveLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/RejectLegalReview")]
        public IHttpActionResult RejectLegalReview(RebuildLegalReview entity)
        {
            entity.RejectLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/LegalReview/ReturnLegalReview")]
        public IHttpActionResult ReturnLegalReview(RebuildLegalReview entity)
        {
            entity.ReturnLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region FinanceAnalysis
        [HttpGet]
        [Route("api/Rebuild/FinanceAnalysis/GetFinanceAnalysis")]
        public IHttpActionResult GetFinanceAnalysis(string projectId, string entityId = "")
        {
            var finance = new RebuildFinancAnalysis();
            var entity = finance.GetFinanceInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Rebuild;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "RebuildFinancAnalysis";

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.ProjectComments = comments;

                    var saveComment =
                        comments.OrderByDescending(e => e.CreateTime)
                            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                    if (saveComment != null)
                    {
                        entity.Comments = saveComment.Content;
                    }
                }
                return Ok(entity);
            }
            else
                return Ok("NULL");
        }

        [HttpGet]
        [Route("api/Rebuild/FinanceAnalysis/GetFinanceAgreementList")]
        public IHttpActionResult GetFinanceAgreementList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("RebuildFinancAnalysis", refTableId, typeCode);
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/SaveFinanceAnalysis")]
        public IHttpActionResult SaveFinanceAnalysis(RebuildFinancAnalysis finance)
        {
            try
            {
                finance.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/SubmitFinanceAnalysis")]
        public IHttpActionResult SubmitFinanceAnalysis(RebuildFinancAnalysis finance)
        {
            try
            {
                finance.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/RecallFinanceAnalysis")]
        public IHttpActionResult RecallFinanceAnalysis(RebuildFinancAnalysis finance)
        {
            try
            {
                finance.Recall(finance.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/EditFinanceAnalysis")]
        public IHttpActionResult EditFinanceAnalysis(RebuildFinancAnalysis finance)
        {
            try
            {
                var taskUrl = finance.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/ResubmitFinanceAnalysis")]
        public IHttpActionResult ResubmitFinanceAnalysis(RebuildFinancAnalysis entity)
        {
            entity.ResubmitFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/ApproveFinanceAnalysis")]
        public IHttpActionResult ApproveFinanceAnalysis(RebuildFinancAnalysis entity)
        {
            entity.ApproveFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/RejectFinanceAnalysis")]
        public IHttpActionResult RejectFinanceAnalysis(RebuildFinancAnalysis entity)
        {
            entity.RejectFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/FinanceAnalysis/ReturnFinanceAnalysis")]
        public IHttpActionResult ReturnFinanceAnalysis(RebuildFinancAnalysis entity)
        {
            entity.ReturnFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region ConsInfo
        [HttpGet]
        [Route("api/Rebuild/ConsInfo/GetConsInfo")]
        public IHttpActionResult GetConsInfo(string projectId, string entityId = "")
        {
            var consinfo = new RebuildConsInfo();
            var entity = consinfo.GetConsInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.Rebuild;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "RebuildConsInfo";

                var comments = ProjectComment.SearchList(condition);
                if (comments != null && comments.Count > 0)
                {
                    entity.ProjectComments = comments;

                    var saveComment =
                        comments.OrderByDescending(e => e.CreateTime)
                            .FirstOrDefault(e => e.Status == ProjectCommentStatus.Save);
                    if (saveComment != null)
                    {
                        entity.Comments = saveComment.Content;
                    }
                }
            }

            return Ok(entity);

        }

        [HttpGet]
        [Route("api/Rebuild/ConsInfo/GetConsInfoAgreementList")]
        public IHttpActionResult GetConsInfoAgreementList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("RebuildConsInfo", refTableId, typeCode);
            foreach (var item in list)
            {
                if (item.InternalName.IndexOf(".") != -1)
                    item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
                else
                    item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName + item.Extension;
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Rebuild/ConsInfo/GetInvestCost")]
        public IHttpActionResult GetInvestCost(string refTableId)
        {
            ReinvestmentCost cost = null;
            if (!string.IsNullOrEmpty(refTableId))
                cost = ReinvestmentCost.GetByConsInfoId(new Guid(refTableId));
            return Ok(cost);
        }

        [HttpGet]
        [Route("api/Rebuild/ConsInfo/GetWriteOff")]
        public IHttpActionResult GetWriteOff(string refTableId)
        {
            WriteOffAmount writeoff = null;
            if (!string.IsNullOrEmpty(refTableId))
                writeoff = WriteOffAmount.GetByConsInfoId(new Guid(refTableId));
            return Ok(writeoff);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/SaveConsInfo")]
        public IHttpActionResult SaveConsInfo(RebuildConsInfo consinfo)
        {
            try
            {
                consinfo.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/SubmitConsInfo")]
        public IHttpActionResult SubmitConsInfo(RebuildConsInfo consinfo)
        {
            try
            {
                consinfo.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/RecallConsInfo")]
        public IHttpActionResult RecallConsInfo(RebuildConsInfo consinfo)
        {
            try
            {
                consinfo.Recall(consinfo.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/EditConsInfo")]
        public IHttpActionResult EditConsInfo(RebuildConsInfo consinfo)
        {
            try
            {
                var taskUrl = consinfo.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/ResubmitConsInfo")]
        public IHttpActionResult ResubmitConsInfo(RebuildConsInfo entity)
        {
            entity.ResubmitConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/ApproveConsInfo")]
        public IHttpActionResult ApproveConsInfo(RebuildConsInfo entity)
        {
            entity.ApproveConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/RejectConsInfo")]
        public IHttpActionResult RejectConsInfo(RebuildConsInfo entity)
        {
            entity.RejectConsInfo(ClientCookie.UserCode);


            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInfo/ReturnConsInfo")]
        public IHttpActionResult ReturnConsInfo(RebuildConsInfo entity)
        {
            entity.ReturnConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region RebuildPackage

        [HttpGet]
        [Route("api/Rebuild/RebuildPackage/GetPackageInfo")]
        public IHttpActionResult GetPackageInfo(string projectId, string entityId = "")
        {
            var package = RebuildPackage.GetRebuildPackageInfo(projectId, entityId);
            return Ok(package);
        }

        [HttpGet]
        [Route("api/Rebuild/RebuildPackage/GetPackageAgreementList")]
        public IHttpActionResult GetPackageAgreementList(string projectId, string refTableId = "")
        {
            var list = RebuildPackage.GetPackageAgreementList(projectId, refTableId, SiteFilePath.UploadFiles_URL);
            var cover = list.Where(e => e.TypeCode.ToLower() == ExcelDataInputType.RebuildCover.ToString().ToLower()).FirstOrDefault();
            if (cover != null)
                list.Remove(cover);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/SavePackageInfo")]
        public IHttpActionResult SavePackageInfo(RebuildPackage package)
        {
            try
            {
                package.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/SubmitPackageInfo")]
        public IHttpActionResult SubmitPackageInfo(RebuildPackage package)
        {
            try
            {
                package.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/ConfirmPackageInfo")]
        public IHttpActionResult ConfirmPackageInfo(RebuildPackage package)
        {
            try
            {
                package.Confrim();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/RecallPackageInfo")]
        public IHttpActionResult RecallPackageInfo(RebuildPackage package)
        {
            try
            {
                package.Recall(package.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/EditPackageInfo")]
        public IHttpActionResult EditPackageInfo(RebuildPackage package)
        {
            try
            {
                var taskUrl = package.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/ResubmitPackageInfo")]
        public IHttpActionResult ResubmitPackageInfo(RebuildPackage entity)
        {
            entity.ResubmitPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/ApprovePackageInfo")]
        public IHttpActionResult ApprovePackageInfo(RebuildPackage entity)
        {
            entity.ApprovePackage(ClientCookie.UserCode);
            var app = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            if (app != null && app.FinanceControllerCode == ClientCookie.UserCode)
            {
                try
                {
                    SendEmail(entity.ProjectId, entity.SerialNumber, entity.ProcInstID, app.VPGMCode);
                }
                catch (Exception e)
                {
                }
            }
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/RejectPackageInfo")]
        public IHttpActionResult RejectPackageInfo(RebuildPackage entity)
        {
            entity.RejectPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/ReturnPackageInfo")]
        public IHttpActionResult ReturnPackageInfo(RebuildPackage entity)
        {
            entity.ReturnPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/RebuildPackage/GenerateZipFile")]
        public IHttpActionResult GenerateZipFile(RebuildPackage package)
        {
            package.Save();
            var current = HttpContext.Current;
            string printPath = GeneratePDFOrImg(package, PrintFileType.Pdf);

            string coverTempPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.RebuildCove_Template;
            string coverTempFilePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + Guid.NewGuid() + ".xlsx";
            package.GenerateCoverEexcel(coverTempPath, coverTempFilePath);

            var listAtt = RebuildPackage.GetPackageAgreementList(package.ProjectId, package.Id.ToString(), SiteFilePath.UploadFiles_URL);

            var printFileName = Path.GetFileName(printPath);
            var printExtention = Path.GetExtension(printPath);

            listAtt.Add(new Attachment() { InternalName = printFileName, Name = "Print", Extension = printExtention });

            string packageFileUrl = ZipHandle.ExeFiles(listAtt);

            return Ok(new { fileName = Path.GetFileName(packageFileUrl) });
        }


        [HttpGet]
        [Route("api/Rebuild/RebuildPackage/DownloadPackage")]
        public IHttpActionResult DownloadPackage(string fileName)
        {
            var current = HttpContext.Current;
            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName;
            string tempFileName = DateTime.Now.ToString("yyMMdd");
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("Package" + tempFileName + ".zip", System.Text.Encoding.GetEncoding("utf-8")));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();
            return Ok();
        }
        public string GeneratePDFOrImg(RebuildPackage package, PrintFileType fileType)
        {
            var store = StoreBasicInfo.GetStorInfo(package.USCode);
            var storeInfo = StoreBasicInfo.GetStore(package.USCode);
            var rbdInfo = new RebuildInfo();
            rbdInfo = rbdInfo.GetRebuildInfo(package.ProjectId);

            //生成Print文件
            var printDic = package.GetPrintTemplateFields();
            //var printDic = new Dictionary<string, string>();
            //printDic.Add("WorkflowName", Constants.Rebuild_Package);
            //printDic.Add("ProjectID", package.ProjectId);
            //printDic.Add("USCode", package.USCode);
            //printDic.Add("StoreNameEN", store.NameENUS);
            //printDic.Add("Market", store.MarketENUS);
            //printDic.Add("Region", store.RegionENUS);
            //printDic.Add("StoreNameCN", store.NameZHCN);
            //printDic.Add("City", store.CityENUS);
            ////printDic.Add("StoreAge", (DateTime.Now.Year - store.OpenDate.Year).ToString());
            //printDic.Add("AddressCN", store.AddressZHCN);
            //printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            //if (storeInfo.StoreContractInfo.EndDate != null && storeInfo.StoreContractInfo.EndDate.HasValue)
            //    printDic.Add("CurrentLeaseENDYear",
            //        (storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd")));
            //else
            //    printDic.Add("CurrentLeaseENDYear", "");
            //printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            //printDic.Add("AssetsActor", storeInfo.StoreDevelop.AssetRepName);
            //printDic.Add("AssetsRep", rbdInfo.AssetActorNameENUS);
            //printDic.Add("ClosureDate", store.CloseDate.HasValue ? store.CloseDate.Value.ToString("yyyy-MM-dd") : "");
            //printDic.Add("WriteOff", package.WriteOff.HasValue ? package.WriteOff.Value.ToString() : "");
            //printDic.Add("CashCompensation", package.CashCompensation.HasValue ? package.CashCompensation.Value.ToString() : "");
            //printDic.Add("NetWriteOff", package.NetWriteOff.HasValue ? package.NetWriteOff.Value.ToString() : "");
            //printDic.Add("NewInvestment", package.NewInvestment.HasValue ? package.NewInvestment.Value.ToString() : "");
            //printDic.Add("CashFlowNVPCurrent", package.CashFlowNVPCurrent.HasValue ? package.CashFlowNVPCurrent.Value.ToString() : "");
            //printDic.Add("CashFlowNVPAfterChange", package.CashFlowNVPAfterChange.HasValue ? package.CashFlowNVPAfterChange.Value.ToString() : "");
            //printDic.Add("OtherCompensation", package.OtherCompensation.HasValue ? package.OtherCompensation.Value.ToString() : "");
            //printDic.Add("NetGain", package.NetGain.HasValue ? package.NetGain.Value.ToString() : "");
            //printDic.Add("ReasonDesc", string.IsNullOrEmpty(package.ReasonDesc) ? "&nbsp;" : "<pre>" + package.ReasonDesc + "</pre>");
            //printDic.Add("OtherCompensationDescription", string.IsNullOrEmpty(package.OtherCompenDesc) ? "&nbsp;" : "<pre>" + package.OtherCompenDesc + "</pre>");
            //printDic.Add("TempClosureDate", package.TempClosureDate.HasValue ? package.TempClosureDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
            //printDic.Add("ReopenDate", package.ReopenDate.HasValue ? package.ReopenDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");

            //printDic.Add("DecisionLogicRecomendation", string.IsNullOrEmpty(package.DecisionLogicRecomendation) ? "&nbsp;" : "<pre>" + package.DecisionLogicRecomendation + "</pre>");

            //if (rbdInfo != null)
            //{
            //    bool hasChangeDesc = false;
            //    string strTheChangeOfTheRental = "";
            //    string strOldRentalStructure = "&nbsp;";
            //    string strNewRentalStructure = "&nbsp;";
            //    if (package.ChangeRentalType.HasValue && package.ChangeRentalType.Value)
            //    {
            //        strTheChangeOfTheRental = "<input type=\"checkbox\" id=\"rental\" checked=\"true\" /> The Change of the rental";
            //        strOldRentalStructure = "<span>Old Rental Structure </span><br>" + package.OldRentalStructure;
            //        strNewRentalStructure = "<span>New Rental Structure </span><br>" + package.NewRentalStructure;
            //        hasChangeDesc = true;
            //        var r = new Regex(@"\n");
            //        strOldRentalStructure = r.Replace(strOldRentalStructure,"<br />");
            //        strNewRentalStructure = r.Replace(strNewRentalStructure, "<br />");
            //    }
            //    else
            //    {
            //        strTheChangeOfTheRental = "<input type=\"checkbox\" id=\"rental\" /> The Change of the rental";
            //    }

            //    string strTheChangeOfRedLine = "";
            //    string strOldChangeRedLineRedLineArea = "";
            //    string strNewChangeRedLineRedLineArea = "";
            //    if (package.ChangeRedLineType.HasValue && package.ChangeRedLineType.Value)
            //    {
            //        strTheChangeOfRedLine = "<input type=\"checkbox\" id=\"redline\" checked=\"true\" /> The Change of red line";
            //        //strOldChangeRedLineRedLineArea = "<input type=\"text\" id=\"oldRedlineArea\" value=\"" + package .OldChangeRedLineRedLineArea+ "\" /> ";
            //        //strNewChangeRedLineRedLineArea = "<input type=\"text\" id=\"newRredlineArea\" value=\"" + package.NewChangeRedLineRedLineArea + "\" /> ";
            //        strOldChangeRedLineRedLineArea = "Old Redline Area (sqm)  &nbsp;<br />" + package.OldChangeRedLineRedLineArea.ToString();
            //        strNewChangeRedLineRedLineArea = "New Redline Area (sqm)  &nbsp;<br />" + package.NewChangeRedLineRedLineArea.ToString();
            //        hasChangeDesc = true;
            //    }
            //    else
            //        strTheChangeOfRedLine = "<input type=\"checkbox\" id=\"redline\" /> The Change of red line";

            //    string strTheChangeOfLeaseTeam = "";
            //    string strOldChangeLeaseTermExpiraryDate = "";
            //    string strNewChangeLeaseTermExpiraryDate = "";
            //    if (package.ChangeLeaseTermType.HasValue && package.ChangeLeaseTermType.Value)
            //    {
            //        strTheChangeOfLeaseTeam = "<input type=\"checkbox\" id=\"leaseterm\" checked=\"true\" /> The Change of lease term";
            //        strOldChangeLeaseTermExpiraryDate =  package.OldChangeLeaseTermExpiraryDate.HasValue
            //            ? "Old Lease Change Expiry (Date) <br />" + package.OldChangeLeaseTermExpiraryDate.Value.ToString("yyyy/MM/dd")
            //            : "&nbsp;";
            //        strNewChangeLeaseTermExpiraryDate =  package.NewChangeLeaseTermExpiraryDate.HasValue
            //            ? "New Lease Change Expiry (Date) <br />" + package.NewChangeLeaseTermExpiraryDate.Value.ToString("yyyy/MM/dd")
            //            : "&nbsp;";
            //        hasChangeDesc = true;
            //    }
            //    else
            //        strTheChangeOfLeaseTeam = "<input type=\"checkbox\" id=\"leaseterm\" /> The Change of lease term";

            //    string strTheChangeOfLandlord = "";
            //    string strOldLandlord = "";
            //    string strNewLandlord = "";
            //    if (package.ChangeLandlordType.HasValue && package.ChangeLandlordType.Value)
            //    {
            //        strTheChangeOfLandlord = "<input type=\"checkbox\" id=\"landlord\" checked=\"true\" /> The change of Landlord";
            //        strOldLandlord = "Old Landlord &nbsp;<br />" + package.OldLandlord;
            //        strNewLandlord = "New Landlord &nbsp;<br />" + package.NewLandlord;
            //        hasChangeDesc = true;
            //    }
            //    else
            //        strTheChangeOfLandlord = "<input type=\"checkbox\" id=\"landlord\" /> The change of Landlord";

            //    string strOthers = "";
            //    string strOthersDESC = "&nbsp;";
            //    if (package.ChangeOtherType.HasValue && package.ChangeOtherType.Value)
            //    {
            //        strOthers = "<input type=\"checkbox\" id=\"others\" checked=\"true\" /> Others";
            //        strOthersDESC = package.Others;
            //        hasChangeDesc = true;
            //    }
            //    else
            //        strOthers = "<input type=\"checkbox\" id=\"others\" /> Others";

            //    string strLeaseChangeDescription = "&nbsp;";
            //    if (hasChangeDesc)
            //    {
            //        strLeaseChangeDescription = package.LeaseChangeDescription;
            //    }
            //    printDic.Add("TheChangeOfTheRental", strTheChangeOfTheRental);
            //    printDic.Add("TheChangeOfRedLine", strTheChangeOfRedLine);
            //    printDic.Add("TheChangeOfLeaseTeam", strTheChangeOfLeaseTeam);
            //    printDic.Add("TheChangeOfLandlord", strTheChangeOfLandlord);
            //    printDic.Add("Others", strOthers);

            //    printDic.Add("OldRentalStructure", strOldRentalStructure);
            //    printDic.Add("NewRentalStructure", strNewRentalStructure);

            //    printDic.Add("OldChangeRedLineRedLineArea", strOldChangeRedLineRedLineArea);
            //    printDic.Add("NewChangeRedLineRedLineArea", strNewChangeRedLineRedLineArea);

            //    printDic.Add("OldChangeLeaseTermExpiraryDate", strOldChangeLeaseTermExpiraryDate);
            //    printDic.Add("NewChangeLeaseTermExpiraryDate", strNewChangeLeaseTermExpiraryDate);

            //    printDic.Add("OldLandlord", strOldLandlord);
            //    printDic.Add("NewLandlord", strNewLandlord);

            //    printDic.Add("OthersDESC", strOthersDESC);
            //    printDic.Add("LeaseChangeDescription", strLeaseChangeDescription);
            //}

            //Submission and Approval Records Details — 所有意见
            var recordDetailList = new List<SubmissionApprovalRecord>();
            //Submission and Approval Records - 只显示通过意见
            var recordList = new List<SubmissionApprovalRecord>();

            var condition = new ProjectCommentCondition
            {
                RefTableName = RebuildPackage.TableName,
                RefTableId = package.Id
            };
            condition.Status = ProjectCommentStatus.Submit;
            var commentList = VProjectComment.SearchVListForPDF(condition);
            var commentDetailList = VProjectComment.SearchVList(condition);

            SubmissionApprovalRecord record = null;
            foreach (var item in commentList)
            {
                record = new SubmissionApprovalRecord();
                record.ActionName = item.ActionDesc;
                if (item.CreateTime != null)
                {
                    record.OperationDate = item.CreateTime.Value;
                }
                record.OperatorID = item.UserAccount;
                record.OperatorName = item.UserNameENUS;
                record.OperatorTitle = item.PositionName;
                record.Comments = item.Content;
                recordList.Add(record);
            }
            recordList = recordList.OrderBy(i => i.OperationDate).ToList();

            foreach (var item in commentDetailList)
            {
                record = new SubmissionApprovalRecord();
                record.ActionName = item.ActionDesc;
                if (item.CreateTime != null)
                {
                    record.OperationDate = item.CreateTime.Value;
                }
                record.OperatorID = item.UserAccount;
                record.OperatorName = item.UserNameENUS;
                record.OperatorTitle = item.PositionName;
                record.Comments = item.Content;
                recordDetailList.Add(record);
            }
            recordDetailList = recordDetailList.OrderBy(i => i.OperationDate).ToList();

            string result = "";
            if (fileType == PrintFileType.Pdf)
            {
                result = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.RebuildPackage, printDic, recordList, recordDetailList);
            }
            else
            {
                result = HtmlConversionUtility.ConvertToImage(HtmlTempalteType.RebuildPackage, printDic, recordList, recordDetailList);
            }
            return result;
        }
        #endregion

        #region ConsInvtChecking
        [HttpGet]
        [Route("api/Rebuild/ConsInvtChecking/GetConsInvtChecking")]
        public IHttpActionResult GetConsInvtChecking(string projectId, string entityId = "")
        {
            var checking = new RebuildConsInvtChecking();
            if (!string.IsNullOrEmpty(projectId))
                checking = checking.GetConsInvtChecking(projectId, entityId);
            return Ok(checking);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/SaveConsInvtChecking")]
        public IHttpActionResult SaveConsInvtChecking(RebuildConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/SubmitConsInvtChecking")]
        public IHttpActionResult SubmitConsInvtChecking(RebuildConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Submit();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/RecallConsInvtChecking")]
        public IHttpActionResult RecallConsInvtChecking(RebuildConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Recall(checkinfo.Comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/EditConsInvtChecking")]
        public IHttpActionResult EditConsInvtChecking(RebuildConsInvtChecking checkinfo)
        {
            try
            {
                var taskUrl = checkinfo.Edit();
                return Ok(new ProjectEditResult
                {
                    TaskUrl = taskUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/ResubmitConsInvtChecking")]
        public IHttpActionResult ResubmitConsInvtChecking(RebuildConsInvtChecking entity)
        {
            entity.ResubmitConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/ApproveConsInvtChecking")]
        public IHttpActionResult ApproveConsInvtChecking(RebuildConsInvtChecking entity)
        {
            entity.ApproveConsInvtChecking(ClientCookie.UserCode);
            var app = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            if (app != null && app.FinanceControllerCode == ClientCookie.UserCode)
            {
                try
                {
                    SendEmail(entity.ProjectId, entity.SerialNumber, entity.ProcInstID, app.VPGMCode);
                }
                catch (Exception e)
                {
                }
            }
            return Ok(entity);
        }

        private void SendEmail(string ProjectId, string SerialNumber, int? ProcInstID, string receiverUserCode)
        {
            var project = ProjectInfo.Get(ProjectId, FlowCode.Rebuild_ConsInvtChecking);

            var storeBasic = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
            using (EmailServiceClient emailClient = new EmailServiceClient())
            {
                EmailMessage email = new EmailMessage();
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApproverName", ClientCookie.UserNameENUS);
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("WorkflowName", "Rebuild");
                bodyValues.Add("StoreCode", storeBasic.StoreCode);
                bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
                bodyValues.Add("WorkflowName", Constants.Rebuild_ConsInvtChecking);////--流程名称
                bodyValues.Add("ProjectName", Constants.Rebuild);//项目名称
                var viewPage = string.Format("{0}/Rebuild/Main#/ConsInvtChecking/Process/Approval?projectId={1}&SN={2}&ProcInstID={3}",
                    HttpContext.Current.Request.Url.AbsolutePath, ProjectId, SerialNumber, ProcInstID);
                bodyValues.Add("FormUrl", viewPage);
                email.EmailBodyValues = bodyValues;

                List<string> emailAddresses = Employee.Search(e => e.Code == receiverUserCode).Select(e => e.Mail).ToList();
                emailAddresses.Add("Stephen.Wang@nttdata.com");
                emailAddresses.Add("Poyet.chen@nttdata.com");
                emailAddresses.Add("Cary.chen@nttdata.com");
                email.To = string.Join(";", emailAddresses);
                emailClient.SendNotificationEmail(email);
            }
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/RejectConsInvtChecking")]
        public IHttpActionResult RejectConsInvtChecking(RebuildConsInvtChecking entity)
        {
            entity.RejectConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/ConsInvtChecking/ReturnConsInvtChecking")]
        public IHttpActionResult ReturnConsInvtChecking(RebuildConsInvtChecking entity)
        {
            entity.ReturnConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        #endregion

        #region SiteInfo
        [HttpGet]
        [Route("api/Rebuild/SiteInfo/InitPage")]
        public IHttpActionResult InitPage(string projectId)
        {
            var rebuildInfo = RebuildInfo.Search(e => e.ProjectId == projectId).FirstOrDefault();
            var siteInfoProject = ProjectInfo.Get(projectId, FlowCode.Rebuild_SiteInfo);

            var estimatedVsActualConstruction =
                EstimatedVsActualConstruction.FirstOrDefault(e => e.RefId == siteInfoProject.Id);
            if (estimatedVsActualConstruction == null)
            {
                var consInfo = new RebuildConsInfo();
                consInfo = consInfo.GetConsInfo(projectId);
                var reinBasicInfo = consInfo.ReinBasicInfo;
                var gbMemo = GBMemo.GetGBMemo(projectId);
                var storeInfo = StoreSTLocation.FirstOrDefault(e => e.StoreCode == rebuildInfo.USCode);

                var days = "";
                if (rebuildInfo != null)
                {
                    if (rebuildInfo.ReopenDate.HasValue && rebuildInfo.TempClosureDate.HasValue)
                        days = (rebuildInfo.ReopenDate.Value - rebuildInfo.TempClosureDate.Value).Days.ToString();
                }

                estimatedVsActualConstruction = new EstimatedVsActualConstruction
                {
                    RefId = siteInfoProject.Id,
                    GBDate = gbMemo.GBDate,
                    CompletionDate = gbMemo.ConstCompletionDate,
                    ARDC = reinBasicInfo.NewDesignType,
                    OriginalOperationSize = storeInfo.TotalArea,
                    OriginalSeatNumber = storeInfo.TotalSeatsNo,
                    ClosureDays = days
                };
            }
            var result = new
            {
                Info = rebuildInfo,
                IsOriginator = ClientCookie.UserCode == rebuildInfo.PMAccount,
                EstimatedVsActualConstruction = estimatedVsActualConstruction,
                IsShowSave = ProjectInfo.IsFlowSavable(projectId, FlowCode.Rebuild_SiteInfo),
            };
            return Ok(result);
        }

        [Route("api/Rebuild/SiteInfo/SaveSiteInfo")]
        [HttpPost]
        public IHttpActionResult SaveSiteInfo(StoreSTLocation store)
        {
            using (var scope = new TransactionScope())
            {
                //store.Save();

                Mapper.CreateMap<StoreSTLocation, StoreSTLocationHistory>();
                var history = Mapper.Map<StoreSTLocationHistory>(store);
                var his = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == store.ProjectIdentifier);
                if (his == null)
                {
                    history.Id = Guid.NewGuid();
                    history.RefId = store.ProjectIdentifier;
                }
                else
                {
                    history.Id = his.Id;
                    history.RefId = store.ProjectIdentifier;
                }
                history.Save();

                if (store.EstimatedVsActualConstruction != null)
                {
                    store.EstimatedVsActualConstruction.Save();
                }

                scope.Complete();
            }
            return Ok();
        }

        [Route("api/Rebuild/SiteInfo/SubmitSiteInfo")]
        [HttpPost]
        public IHttpActionResult SubmitSiteInfo(StoreSTLocation store)
        {
            using (var scope = new TransactionScope())
            {
                //store.Save();

                Mapper.CreateMap<StoreSTLocation, StoreSTLocationHistory>();
                var history = Mapper.Map<StoreSTLocationHistory>(store);
                var his = StoreSTLocationHistory.FirstOrDefault(e => e.RefId == store.ProjectIdentifier);
                var project = ProjectInfo.Get(store.ProjectIdentifier);
                if (his == null)
                {
                    history.Id = Guid.NewGuid();
                    history.RefId = store.ProjectIdentifier;
                }
                else
                {
                    history.Id = his.Id;
                    history.RefId = store.ProjectIdentifier;
                }
                history.Save();

                if (store.EstimatedVsActualConstruction != null)
                {
                    store.EstimatedVsActualConstruction.Save();
                }
                TaskWork.Finish(t => t.ReceiverAccount == ClientCookie.UserCode && t.TypeCode == FlowCode.Rebuild_SiteInfo && t.RefID == project.ProjectId);
                ProjectInfo.FinishNode(project.ProjectId, FlowCode.Rebuild_SiteInfo, NodeCode.Finish);
                ProjectInfo.CompleteMainIfEnable(project.ProjectId);
                scope.Complete();
            }
            return Ok();
        }
        #endregion

        #region GBMemo
        [HttpGet]
        [Route("api/Rebuild/GBMemo/GetGBMemoInfo")]
        public IHttpActionResult GetGBMemoInfo(string projectId, string entityId = "")
        {
            var memo = GBMemo.GetGBMemo(projectId, entityId);
            return Ok(memo);
        }
        [HttpPost]
        [Route("api/Rebuild/GBMemo/SaveGBMemo")]
        public IHttpActionResult SaveGBMemo(GBMemo memo)
        {
            memo.Save();
            return Ok();
        }

        [HttpPost]
        [Route("api/Rebuild/GBMemo/SubmitGBMemo")]
        public IHttpActionResult SubmitGBMemo(GBMemo memo)
        {
            memo.Submit();
            return Ok();
        }
        [HttpPost]
        [Route("api/Rebuild/GBMemo/ResubmitGBMemo")]
        public IHttpActionResult ResubmitGBMemo(GBMemo entity)
        {
            entity.Resubmit(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/GBMemo/ApproveGBMemo")]
        public IHttpActionResult ApproveGBMemo(GBMemo entity)
        {
            entity.Approve(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Rebuild/GBMemo/RecallGBMemo")]
        public IHttpActionResult RecallGBMemo(GBMemo entity)
        {
            entity.Recall(entity.Comments);
            return Ok();
        }

        [HttpPost]
        [Route("api/Rebuild/GBMemo/EditGBMemo")]
        public IHttpActionResult EditGBMemo(GBMemo entity)
        {
            var taskUrl = entity.Edit();
            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }

        [HttpPost]
        [Route("api/Rebuild/GBMemo/ReturnGBMemo")]
        public IHttpActionResult ReturnGBMemo(GBMemo entity)
        {
            entity.Return(ClientCookie.UserCode);
            return Ok(entity);
        }

        [Route("api/Rebuild/GBMemo/NotifyGBMemo")]
        [HttpPost]
        public IHttpActionResult NotifyGBMemo(PostMemo<GBMemo> postData)
        {
            var actor = ProjectUsers.GetProjectUser(postData.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                pdfData.Add("WorkflowName", Constants.Rebuild);
                pdfData.Add("ProjectID", postData.Entity.ProjectId);
                pdfData.Add("RegionNameENUS", postData.Entity.Store.StoreBasicInfo.RegionENUS);
                pdfData.Add("RegionNameZHCN", postData.Entity.Store.StoreBasicInfo.RegionZHCN);
                pdfData.Add("MarketNameENUS", postData.Entity.Store.StoreBasicInfo.MarketENUS);
                pdfData.Add("MarketNameZHCN", postData.Entity.Store.StoreBasicInfo.MarketZHCN);
                pdfData.Add("ProvinceNameENUS", postData.Entity.Store.StoreBasicInfo.ProvinceENUS);
                pdfData.Add("ProvinceNameZHCN", postData.Entity.Store.StoreBasicInfo.ProvinceZHCN);
                pdfData.Add("CityNameENUS", postData.Entity.Store.StoreBasicInfo.CityENUS);
                pdfData.Add("CityNameZHCN", postData.Entity.Store.StoreBasicInfo.CityZHCN);
                pdfData.Add("StoreNameENUS", postData.Entity.Store.StoreBasicInfo.NameENUS);
                pdfData.Add("StoreNameZHCN", postData.Entity.Store.StoreBasicInfo.NameZHCN);
                pdfData.Add("USCode", postData.Entity.Store.StoreBasicInfo.StoreCode);

                pdfData.Add("IsClosed", postData.Entity.IsClosed ? "Y" : "N");
                pdfData.Add("IsInOperation", postData.Entity.IsInOperation ? "Y" : "N");
                pdfData.Add("IsMcCafe", postData.Entity.IsMcCafe ? "Y" : "N");
                pdfData.Add("IsKiosk", postData.Entity.IsKiosk ? "Y" : "N");
                pdfData.Add("IsMDS", postData.Entity.IsMDS ? "Y" : "N");
                pdfData.Add("Is24Hour", postData.Entity.Is24Hour ? "Y" : "N");

                pdfData.Add("GBDate", postData.Entity.GBDate.HasValue ? postData.Entity.GBDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ConstCompletionDate", postData.Entity.ConstCompletionDate.HasValue ? postData.Entity.ConstCompletionDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ReopenDate", postData.Entity.ReopenDate.HasValue ? postData.Entity.ReopenDate.Value.ToString("yyyy-MM-dd") : "");

                //Submission and Approval Records Details — 所有意见
                var recordDetailList = new List<SubmissionApprovalRecord>();

                var condition = new ProjectCommentCondition
                {
                    RefTableName = RebuildPackage.TableName,
                    RefTableId = postData.Entity.Id
                };
                condition.Status = ProjectCommentStatus.Submit;
                var commentDetailList = VProjectComment.SearchVList(condition);

                SubmissionApprovalRecord record = null;

                foreach (var item in commentDetailList)
                {
                    record = new SubmissionApprovalRecord();
                    record.ActionName = item.ActionDesc;
                    if (item.CreateTime != null)
                    {
                        record.OperationDate = item.CreateTime.Value;
                    }
                    record.OperatorID = item.UserAccount;
                    record.OperatorName = item.UserNameENUS;
                    record.OperatorTitle = item.PositionName;
                    record.Comments = item.Content;
                    recordDetailList.Add(record);
                }
                recordDetailList = recordDetailList.OrderBy(i => i.OperationDate).ToList();

                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.GBMemo, pdfData, recordDetailList);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.Store.StoreBasicInfo.StoreCode);
                bodyValues.Add("StoreName", postData.Entity.Store.StoreBasicInfo.NameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", Constants.Rebuild_GBMemo); ////--流程名称
                bodyValues.Add("ProjectName", Constants.Rebuild); //项目名称

                string viewPage = string.Format("{0}/Rebuild/Main#/GBMemo/Process/View?projectId={1}",
                        HttpContext.Current.Request.Url.Authority, postData.Entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    foreach (Employee emp in postData.Receivers)
                    {
                        if (sbTo.Length > 0)
                        {
                            sbTo.Append(";");
                        }
                        if (!string.IsNullOrEmpty(emp.Mail))
                        {
                            sbTo.Append(emp.Mail);
                        }
                    }
                    if (sbTo.Length > 0)
                    {
                        sbTo.Append(";");
                    }
                    message.EmailBodyValues = bodyValues;
                    string strTitle = FlowCode.Rebuild_GBMemo;
                    attachments.Add(pdfPath, strTitle + "_" + postData.Entity.ProjectId + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendGBMemoNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }
                postData.Entity.CompleteNotifyTask(postData.Entity.ProjectId);
                tranScope.Complete();
            }
            return Ok();
        }
        #endregion
    }
}