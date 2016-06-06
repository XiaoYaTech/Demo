using System.IO;
using System.Text;
using System.Web;
using AutoMapper;
using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Enums;
using Mcdonalds.AM.Services.Common;
using System.Transactions;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.DataAccess.Common;


namespace Mcdonalds.AM.Services.Controllers.MajorLease
{
    public class MajorLeaseController : ApiController
    {

        [Route("api/MajorLease/BeginCreate")]
        [HttpPost]
        public IHttpActionResult BeginCreate(MajorLeaseInfo major)
        {
            try
            {
                using (var tran = new TransactionScope())
                {
                    major.Id = Guid.NewGuid();
                    major.ProjectId = ProjectInfo.CreateMainProject(FlowCode.MajorLease, major.USCode, NodeCode.Start, major.CreateUserAccount);
                    major.CreateTime = DateTime.Now;
                    MajorLeaseInfo.Add(major);
                    major.AddProjectUsers();
                    major.SendRemind();
                    major.SendWorkTask();
                    major.CreateSubProject();
                    major.CreateAttachmentsMemo();

                    //if (major.ProjectContractRevision != null)
                    //{
                    //    major.ProjectContractRevision.ProjectId = major.ProjectId;
                    //    major.ProjectContractRevision.Append();
                    //}
                    ProjectNode.GenerateOnCreate(FlowCode.MajorLease, major.ProjectId);
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
        [Route("api/MajorLease/GetMajorInfo")]
        public IHttpActionResult GetMajorInfo(string projectId)
        {
            var majorInfo = MajorLeaseInfo.Search(e => e.ProjectId == projectId).FirstOrDefault();
            if (majorInfo != null)
            {
                if (majorInfo.ReopenDate.HasValue)
                {
                    majorInfo.ReopenedDays = (majorInfo.ReopenDate.Value - DateTime.Now).Days;
                }
                majorInfo.ProjectContractRevision = ProjectContractRevision.Get(majorInfo.ProjectId);
                majorInfo.IsContractInfoSaveable = ProjectInfo.IsFlowSavable(projectId, FlowCode.MajorLease_ContractInfo);
                majorInfo.IsSiteInfoSaveable = ProjectInfo.IsFlowSavable(projectId, FlowCode.MajorLease_SiteInfo);
            }

            return Ok(majorInfo);
        }

        [HttpGet]
        [Route("api/MajorLease/GetProjectId")]
        public IHttpActionResult GetProjectId(int procInstId)
        {
            var task = MajorLeaseLegalReview.Search(e => e.ProcInstID.Value.Equals(procInstId)).FirstOrDefault();
            return Ok(task);
        }


        //[Route("api/MajorLease/GetNavTop")]
        //[HttpGet]
        //public IHttpActionResult GetNavTop(string projectId, string userCode)
        //{
        //    var pjUserBll = new ProjectUsers();
        //    //var taskWork = new TaskWork();
        //    var projectInfo = new ProjectInfo();
        //    var navInfo = new NavInfo();
        //    var Navs = new
        //    {
        //        NavInfos = navInfo.GetNavInfos(FlowCode.MajorLease, projectId, userCode).OrderBy(e => e.Disq),
        //        UserList = pjUserBll.GetProjectUserList(userCode, projectId).ToList(),
        //        //FinishedTaskList = taskWork.GetFinishedTasks(projectId).ToList()
        //        FinishedTaskList = ProjectInfo.GetFinishedProject(projectId).ToList()
        //    };
        //    return Ok(Navs);
        //}

        #region Attachment

        [HttpPost]
        [Route("api/MajorLease/{typeCode}/UploadContract/{projectId}")]
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
                            var entity = MajorLeaseLegalReview.GetLegalReviewInfo(projectId, "");
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new MajorLeaseLegalReview();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                MajorLeaseLegalReview.Add(entity);
                            }
                            strNodeCode = typeCode == "Agreement" ? NodeCode.MajorLease_LegalReview_Confirm : NodeCode.MajorLease_LegalReview_Upload;
                            strFlowCode = FlowCode.MajorLease_LegalReview;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "MajorLeaseLegalReview";
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
                            var finance = new MajorLeaseFinancAnalysis();
                            var entity = finance.GetFinanceInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new MajorLeaseFinancAnalysis();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                MajorLeaseFinancAnalysis.Add(entity);
                            }
                            strFlowCode = FlowCode.MajorLease_FinanceAnalysis;
                            strNodeCode = NodeCode.MajorLease_FinanceAnalysis_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "MajorLeaseFinancAnalysis";
                            if (typeCode == "FinancAnalysisAttach")
                                strTypeCode = "Attachment";
                            else if (typeCode == "FinanceAnalysis")
                                strTypeCode = "FinAgreement";
                        }
                        else if (typeCode == "ConsInfoAttach"
                            || typeCode == "ConsInfo")
                        {
                            var consinfo = new MajorLeaseConsInfo();
                            var entity = consinfo.GetConsInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new MajorLeaseConsInfo();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                MajorLeaseConsInfo.Add(entity);
                            }
                            strFlowCode = FlowCode.MajorLease_ConsInfo;
                            strNodeCode = NodeCode.MajorLease_ConsInfo_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "MajorLeaseConsInfo";
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
                            var entity = MajorLeaseChangePackage.GetMajorPackageInfo(projectId);
                            if (entity == null || entity.Id == Guid.Empty)
                            {
                                entity = new MajorLeaseChangePackage();
                                entity.ProjectId = projectId;
                                entity.Id = Guid.NewGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CreateUserAccount = ClientCookie.UserCode;
                                entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
                                entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                                entity.IsHistory = false;
                                MajorLeaseChangePackage.Add(entity);
                            }
                            strTypeCode = typeCode;
                            strFlowCode = FlowCode.MajorLease_Package;
                            strNodeCode = NodeCode.MajorLease_Package_Upload;
                            strEntityId = entity.Id.ToString();
                            strRefTableName = "MajorLeaseChangePackage";
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
        [Route("api/MajorLease/DeleteAttachement")]
        public IHttpActionResult DeleteAttachement(string flowCode, string projectId, Guid attId)
        {
            if (flowCode == "LegalReview")
                ProjectInfo.UnFinishNode(projectId, FlowCode.MajorLease_LegalReview, NodeCode.MajorLease_LegalReview_Upload);
            else if (flowCode == "FinanceAnalysis")
                ProjectInfo.UnFinishNode(projectId, FlowCode.MajorLease_FinanceAnalysis, NodeCode.MajorLease_FinanceAnalysis_Upload);
            else if (flowCode == "Consinfo")
                ProjectInfo.UnFinishNode(projectId, FlowCode.MajorLease_ConsInfo, NodeCode.MajorLease_ConsInfo_Upload);
            Attachment.Delete(attId);
            return Ok();
        }

        #endregion

        #region LegalReview
        [HttpGet]
        [Route("api/MajorLease/LegalReview/GetLegalReview")]
        public IHttpActionResult GetLegalReview(string projectId, string entityId = "")
        {
            var entity = MajorLeaseLegalReview.GetLegalReviewInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.MajorLease;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "MajorLeaseLegalReview";

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
        [Route("api/MajorLease/LegalReview/GetLegalContractList")]
        public IHttpActionResult GetLegalContractList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("MajorLeaseLegalReview", refTableId, typeCode);
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/MajorLease/LegalReview/SaveLegalReview")]
        public IHttpActionResult SaveLegalReview(MajorLeaseLegalReview legalReview)
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
        [Route("api/MajorLease/LegalReview/SubmitLegalReview")]
        public IHttpActionResult SubmitLegalReview(MajorLeaseLegalReview legalReview)
        {
            legalReview.Submit();
            return Ok();
        }


        [HttpPost]
        [Route("api/MajorLease/LegalReview/RecallLegalReview")]
        public IHttpActionResult RecallLegalReview(MajorLeaseLegalReview legalReview)
        {
            legalReview.Recall(legalReview.Comments);
            return Ok();
        }


        [HttpPost]
        [Route("api/MajorLease/LegalReview/EditLegalReview")]
        public IHttpActionResult EditLegalReview(MajorLeaseLegalReview legalReview)
        {
            var taskUrl = legalReview.Edit();

            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }


        [HttpPost]
        [Route("api/MajorLease/LegalReview/ResubmitLegalReview")]
        public IHttpActionResult ResubmitLegalReview(MajorLeaseLegalReview entity)
        {
            entity.ResubmitLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LegalReview/ApproveLegalReview")]
        public IHttpActionResult ApproveLegalReview(MajorLeaseLegalReview entity)
        {
            entity.ApproveLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LegalReview/RejectLegalReview")]
        public IHttpActionResult RejectLegalReview(MajorLeaseLegalReview entity)
        {
            entity.RejectLegalReview(ClientCookie.UserCode);


            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LegalReview/ReturnLegalReview")]
        public IHttpActionResult ReturnLegalReview(MajorLeaseLegalReview entity)
        {
            entity.ReturnLegalReview(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region FinanceAnalysis
        [HttpGet]
        [Route("api/MajorLease/FinanceAnalysis/GetFinanceAnalysis")]
        public IHttpActionResult GetFinanceAnalysis(string projectId, string entityId = "")
        {
            var finance = new MajorLeaseFinancAnalysis();
            var projectComment = new ProjectComment();
            var entity = finance.GetFinanceInfo(projectId, entityId);

            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.MajorLease;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "MajorLeaseFinancAnalysis";

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
        [Route("api/MajorLease/FinanceAnalysis/GetFinanceAgreementList")]
        public IHttpActionResult GetFinanceAgreementList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("MajorLeaseFinancAnalysis", refTableId, typeCode);
            foreach (var item in list)
            {
                item.FileURL = SiteFilePath.UploadFiles_URL + "/" + item.InternalName;
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/MajorLease/FinanceAnalysis/SaveFinanceAnalysis")]
        public IHttpActionResult SaveFinanceAnalysis(MajorLeaseFinancAnalysis finance)
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
        [Route("api/MajorLease/FinanceAnalysis/SubmitFinanceAnalysis")]
        public IHttpActionResult SubmitFinanceAnalysis(MajorLeaseFinancAnalysis finance)
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
        [Route("api/MajorLease/FinanceAnalysis/RecallFinanceAnalysis")]
        public IHttpActionResult RecallFinanceAnalysis(MajorLeaseFinancAnalysis finance)
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
        [Route("api/MajorLease/FinanceAnalysis/EditFinanceAnalysis")]
        public IHttpActionResult EditFinanceAnalysis(MajorLeaseFinancAnalysis finance)
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
        [Route("api/MajorLease/FinanceAnalysis/ResubmitFinanceAnalysis")]
        public IHttpActionResult ResubmitFinanceAnalysis(MajorLeaseFinancAnalysis entity)
        {
            entity.ResubmitFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/FinanceAnalysis/ApproveFinanceAnalysis")]
        public IHttpActionResult ApproveFinanceAnalysis(MajorLeaseFinancAnalysis entity)
        {
            entity.ApproveFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/FinanceAnalysis/RejectFinanceAnalysis")]
        public IHttpActionResult RejectFinanceAnalysis(MajorLeaseFinancAnalysis entity)
        {
            entity.RejectFinanceAnalysis(ClientCookie.UserCode);


            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/FinanceAnalysis/ReturnFinanceAnalysis")]
        public IHttpActionResult ReturnFinanceAnalysis(MajorLeaseFinancAnalysis entity)
        {
            entity.ReturnFinanceAnalysis(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region ConsInfo
        [HttpGet]
        [Route("api/MajorLease/ConsInfo/GetConsInfo")]
        public IHttpActionResult GetConsInfo(string projectId, string entityId = "")
        {
            var consinfo = new MajorLeaseConsInfo();
            var entity = consinfo.GetConsInfo(projectId, entityId);

            if (entity != null)
            {
                if (!entity.ReinvenstmentType.HasValue)
                    entity.ReinvenstmentType = 1;
                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.MajorLease;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = "MajorLeaseConsInfo";

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
        [Route("api/MajorLease/ConsInfo/GetReinvenstmentAmountType")]
        public IHttpActionResult GetReinvenstmentAmountType()
        {
            var amountType = new ReinvenstmentAmountType();
            var entitis = amountType.GetAmountTypeInfo();
            if (entitis != null)
            {
                return Ok(entitis);
            }
            else
                return Ok("NULL");
        }

        [HttpGet]
        [Route("api/MajorLease/ConsInfo/GetConsInfoAgreementList")]
        public IHttpActionResult GetConsInfoAgreementList(string refTableId, string typeCode)
        {
            var list = Attachment.GetList("MajorLeaseConsInfo", refTableId, typeCode);
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
        [Route("api/MajorLease/ConsInfo/GetInvestCost")]
        public IHttpActionResult GetInvestCost(string refTableId)
        {
            ReinvestmentCost cost = null;
            if (!string.IsNullOrEmpty(refTableId))
                cost = ReinvestmentCost.GetByConsInfoId(new Guid(refTableId));
            return Ok(cost);
        }

        [HttpGet]
        [Route("api/MajorLease/ConsInfo/GetWriteOff")]
        public IHttpActionResult GetWriteOff(string refTableId)
        {
            WriteOffAmount writeoff = null;
            if (!string.IsNullOrEmpty(refTableId))
                writeoff = WriteOffAmount.GetByConsInfoId(new Guid(refTableId));
            return Ok(writeoff);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInfo/SaveConsInfo")]
        public IHttpActionResult SaveConsInfo(MajorLeaseConsInfo consinfo)
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
        [Route("api/MajorLease/ConsInfo/SubmitConsInfo")]
        public IHttpActionResult SubmitConsInfo(MajorLeaseConsInfo consinfo)
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
        [Route("api/MajorLease/ConsInfo/RecallConsInfo")]
        public IHttpActionResult RecallConsInfo(MajorLeaseConsInfo consinfo)
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
        [Route("api/MajorLease/ConsInfo/EditConsInfo")]
        public IHttpActionResult EditConsInfo(MajorLeaseConsInfo consinfo)
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
        [Route("api/MajorLease/ConsInfo/ResubmitConsInfo")]
        public IHttpActionResult ResubmitConsInfo(MajorLeaseConsInfo entity)
        {
            entity.ResubmitConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInfo/ApproveConsInfo")]
        public IHttpActionResult ApproveConsInfo(MajorLeaseConsInfo entity)
        {
            entity.ApproveConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInfo/RejectConsInfo")]
        public IHttpActionResult RejectConsInfo(MajorLeaseConsInfo entity)
        {
            entity.RejectConsInfo(ClientCookie.UserCode);


            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInfo/ReturnConsInfo")]
        public IHttpActionResult ReturnConsInfo(MajorLeaseConsInfo entity)
        {
            entity.ReturnConsInfo(ClientCookie.UserCode);

            return Ok(entity);
        }
        #endregion

        #region LeaseChangePackage

        [HttpGet]
        [Route("api/MajorLease/LeaseChangePackage/GetPackageInfo")]
        public IHttpActionResult GetPackageInfo(string projectId, string entityId = "")
        {
            var package = MajorLeaseChangePackage.GetMajorPackageInfo(projectId, entityId);
            return Ok(package);
        }

        [HttpGet]
        [Route("api/MajorLease/LeaseChangePackage/GetPackageAgreementList")]
        public IHttpActionResult GetPackageAgreementList(string projectId, string refTableId)
        {
            var list = MajorLeaseChangePackage.GetPackageAgreementList(projectId, refTableId, SiteFilePath.UploadFiles_URL);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/MajorLease/LeaseChangePackage/SavePackageInfo")]
        public IHttpActionResult SavePackageInfo(MajorLeaseChangePackage package)
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
        [Route("api/MajorLease/LeaseChangePackage/SubmitPackageInfo")]
        public IHttpActionResult SubmitPackageInfo(MajorLeaseChangePackage package)
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
        [Route("api/MajorLease/LeaseChangePackage/ConfirmPackageInfo")]
        public IHttpActionResult ConfirmPackageInfo(MajorLeaseChangePackage package)
        {
            try
            {
                package.Confirm(ClientCookie.UserCode);
                //package.ApprovePackage(ClientCookie.UserCode);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/MajorLease/LeaseChangePackage/RecallPackageInfo")]
        public IHttpActionResult RecallPackageInfo(MajorLeaseChangePackage package)
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
        [Route("api/MajorLease/LeaseChangePackage/EditPackageInfo")]
        public IHttpActionResult EditPackageInfo(MajorLeaseChangePackage package)
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
        [Route("api/MajorLease/LeaseChangePackage/ResubmitPackageInfo")]
        public IHttpActionResult ResubmitPackageInfo(MajorLeaseChangePackage entity)
        {
            entity.ResubmitPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LeaseChangePackage/ApprovePackageInfo")]
        public IHttpActionResult ApprovePackageInfo(MajorLeaseChangePackage entity)
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
        [Route("api/MajorLease/LeaseChangePackage/RejectPackageInfo")]
        public IHttpActionResult RejectPackageInfo(MajorLeaseChangePackage entity)
        {
            entity.RejectPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LeaseChangePackage/ReturnPackageInfo")]
        public IHttpActionResult ReturnPackageInfo(MajorLeaseChangePackage entity)
        {
            entity.ReturnPackage(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/LeaseChangePackage/GenerateZipFile")]
        public IHttpActionResult GenerateZipFile(MajorLeaseChangePackage package)
        {
            package.Save();
            var current = HttpContext.Current;
            string printPath = GeneratePDFOrImg(package, PrintFileType.Pdf);

            string coverTempPath = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.MajorLeaseChangeCove_Template;
            string coverTempFilePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + Guid.NewGuid() + ".xlsx";
            package.GenerateCoverEexcel(coverTempPath, coverTempFilePath);

            var listAtt = MajorLeaseChangePackage.GetPackageAgreementList(package.ProjectId, package.Id.ToString(), SiteFilePath.UploadFiles_URL);

            var printFileName = Path.GetFileName(printPath);
            var printExtention = Path.GetExtension(printPath);

            listAtt.Add(new Attachment() { InternalName = printFileName, Name = "Print", Extension = printExtention });

            string packageFileUrl = ZipHandle.ExeFiles(listAtt);

            return Ok(new { fileName = Path.GetFileName(packageFileUrl) });
        }


        [HttpGet]
        [Route("api/MajorLease/LeaseChangePackage/DownloadPackage")]
        public IHttpActionResult DownloadPackage(string fileName)
        {
            var current = HttpContext.Current;
            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName;
            string tempFileName = DateTime.Now.ToString("yyMMdd");
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode("Package" + tempFileName + ".zip", System.Text.Encoding.GetEncoding("utf-8")));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + tempFilePath + "");
            current.Response.End();
            return Ok();
        }
        public string GeneratePDFOrImg(MajorLeaseChangePackage package, PrintFileType fileType)
        {
            var store = StoreBasicInfo.GetStorInfo(package.USCode);
            var storeInfo = StoreBasicInfo.GetStore(package.USCode);
            var marjorInfo = new MajorLeaseInfo();
            marjorInfo = marjorInfo.GetMajorLeaseInfo(package.ProjectId);

            //生成Print文件
            var printDic = new Dictionary<string, string>();
            printDic.Add("WorkflowName", FlowCode.MajorLease_Package);
            printDic.Add("ProjectID", package.ProjectId);
            printDic.Add("USCode", package.USCode);
            printDic.Add("StoreNameEN", store.NameENUS);
            printDic.Add("Market", store.MarketENUS);
            printDic.Add("Region", store.RegionENUS);
            printDic.Add("StoreNameCN", store.NameZHCN);
            printDic.Add("City", store.CityENUS);
            printDic.Add("Address", store.AddressZHCN);
            printDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            printDic.Add("CurrentLeaseENDYear", (storeInfo.CurrentYear - int.Parse(storeInfo.StoreContractInfo.EndYear)).ToString());
            printDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            printDic.Add("AssetsActor", storeInfo.StoreDevelop.AssetRepName);
            printDic.Add("AssetsRep", marjorInfo.AssetActorNameENUS);//store.AssetRepName  TODO::Cary

            if (marjorInfo != null)
            {
                string strTheChangeOfTheRental = "";
                if (marjorInfo.ChangeRentalType.HasValue && marjorInfo.ChangeRentalType.Value)
                    strTheChangeOfTheRental = "<input type=\"checkbox\" id=\"rental\" checked=\"true\" /> The Change of the rental";
                else
                    strTheChangeOfTheRental = "<input type=\"checkbox\" id=\"rental\" /> The Change of the rental";

                string strTheChangeOfRedLine = "";
                if (marjorInfo.ChangeRedLineType.HasValue && marjorInfo.ChangeRedLineType.Value)
                    strTheChangeOfRedLine = "<input type=\"checkbox\" id=\"redline\" checked=\"true\" /> The Change of red line";
                else
                    strTheChangeOfRedLine = "<input type=\"checkbox\" id=\"redline\" /> The Change of red line";

                string strTheChangeOfLeaseTeam = "";
                if (marjorInfo.ChangeLeaseTermType.HasValue && marjorInfo.ChangeLeaseTermType.Value)
                    strTheChangeOfLeaseTeam = "<input type=\"checkbox\" id=\"leaseterm\" checked=\"true\" /> The Change of lease term";
                else
                    strTheChangeOfLeaseTeam = "<input type=\"checkbox\" id=\"leaseterm\" /> The Change of lease term";

                printDic.Add("TheChangeOfTheRental", strTheChangeOfTheRental);
                printDic.Add("TheChangeOfRedLine", strTheChangeOfRedLine);
                printDic.Add("TheChangeOfLeaseTeam", strTheChangeOfLeaseTeam);

                printDic.Add("ChangeRentalExpiraryDate", package.ChangeRentalExpiraryDate.HasValue ? package.ChangeRentalExpiraryDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                printDic.Add("ChangeRentalRedLineArea", package.ChangeRentalRedLineArea.HasValue ? package.ChangeRentalRedLineArea.Value.ToString() : "&nbsp;");
                printDic.Add("ChangeRentalTypeDESC", string.IsNullOrEmpty(package.ChangeRentalTypeDESC) ? "&nbsp;" : package.ChangeRentalTypeDESC);

                printDic.Add("ChangeRedLineExpiraryDate", package.ChangeRedLineExpiraryDate.HasValue ? package.ChangeRedLineExpiraryDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                printDic.Add("ChangeRedLineRedLineArea", package.ChangeRedLineRedLineArea.HasValue ? package.ChangeRedLineRedLineArea.Value.ToString() : "&nbsp;");
                printDic.Add("ChangeRedLineTypeDESC", string.IsNullOrEmpty(package.ChangeRedLineTypeDESC) ? "&nbsp;" : package.ChangeRedLineTypeDESC);

                printDic.Add("ChangeLeaseTermExpiraryDate", package.ChangeLeaseTermExpiraryDate.HasValue ? package.ChangeLeaseTermExpiraryDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                printDic.Add("ChangeLeaseTermRedLineArea", package.ChangeLeaseTermRedLineArea.HasValue ? package.ChangeLeaseTermRedLineArea.Value.ToString() : "&nbsp;");
                printDic.Add("ChangeLeaseTermDESC", string.IsNullOrEmpty(package.ChangeLeaseTermDESC) ? "&nbsp;" : package.ChangeLeaseTermDESC);
            }

            printDic.Add("TotalWriteOff", package.WriteOff.HasValue ? package.WriteOff.Value.ToString() : "");
            printDic.Add("Compensation", package.CashCompensation.HasValue ? package.CashCompensation.Value.ToString() : "");
            printDic.Add("NetWriteOff", package.NetWriteOff.HasValue ? package.NetWriteOff.Value.ToString() : "");
            printDic.Add("NetInvestmentCoast", package.NewInvestment.HasValue ? package.NewInvestment.Value.ToString() : "");
            printDic.Add("CashFlowNPVCurrent", package.CashFlowNVPCurrent.HasValue ? package.CashFlowNVPCurrent.Value.ToString() : "");
            printDic.Add("CashFlowNPVAfterkRebuild", package.CashFlowNVPAfterChange.HasValue ? package.CashFlowNVPAfterChange.Value.ToString() : "");
            printDic.Add("OtherCompensation", package.OtherCompensation.HasValue ? package.OtherCompensation.Value.ToString() : "");
            printDic.Add("NetGain", package.NetGain.HasValue ? package.NetGain.Value.ToString() : "");
            printDic.Add("ReasonDescription", package.ReasonDesc);
            printDic.Add("OtherCompensationDescription", package.OtherCompenDesc);

            var recordList = new List<SubmissionApprovalRecord>();
            var projectCommentBll = new ProjectComment();
            var condition = new ProjectCommentCondition
            {
                RefTableName = "MajorLeaseChangePackage",
                RefTableId = package.Id,
                SourceCode = FlowCode.MajorLease_Package
            };

            var commentList = ProjectComment.SearchList(condition);
            foreach (var item in commentList)
            {
                var record = new SubmissionApprovalRecord { ActionName = item.Action };
                if (item.CreateTime != null)
                {
                    record.OperationDate = item.CreateTime.Value;
                }
                record.OperatorID = item.UserAccount;
                record.OperatorName = item.UserNameZHCN;
                record.OperatorTitle = item.TitleNameZHCN;
                recordList.Add(record);
            }

            string result = "";
            if (fileType == PrintFileType.Pdf)
            {
                result = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.MajorLease, printDic, recordList);
            }
            else
            {
                result = HtmlConversionUtility.ConvertToImage(HtmlTempalteType.MajorLease, printDic, recordList);
            }
            return result;
        }
        #endregion

        #region ConsInvtChecking
        [HttpGet]
        [Route("api/MajorLease/ConsInvtChecking/GetConsInvtChecking")]
        public IHttpActionResult GetConsInvtChecking(string projectId, string entityId = "")
        {
            var checking = new MajorLeaseConsInvtChecking();
            if (!string.IsNullOrEmpty(projectId))
                checking = checking.GetConsInvtChecking(projectId, entityId);
            return Ok(checking);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInvtChecking/SaveConsInvtChecking")]
        public IHttpActionResult SaveConsInvtChecking(MajorLeaseConsInvtChecking checkinfo)
        {
            try
            {
                checkinfo.Save();
                return Ok(checkinfo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInvtChecking/SubmitConsInvtChecking")]
        public IHttpActionResult SubmitConsInvtChecking(MajorLeaseConsInvtChecking checkinfo)
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
        [Route("api/MajorLease/ConsInvtChecking/RecallConsInvtChecking")]
        public IHttpActionResult RecallConsInvtChecking(MajorLeaseConsInvtChecking checkinfo)
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
        [Route("api/MajorLease/ConsInvtChecking/EditConsInvtChecking")]
        public IHttpActionResult EditConsInvtChecking(MajorLeaseConsInvtChecking checkinfo)
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
        [Route("api/MajorLease/ConsInvtChecking/ResubmitConsInvtChecking")]
        public IHttpActionResult ResubmitConsInvtChecking(MajorLeaseConsInvtChecking entity)
        {
            entity.ResubmitConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInvtChecking/ApproveConsInvtChecking")]
        public IHttpActionResult ApproveConsInvtChecking(MajorLeaseConsInvtChecking entity)
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
            var project = ProjectInfo.Get(ProjectId, FlowCode.MajorLease_ConsInvtChecking);

            var storeBasic = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
            var bllEmployee = new Employee();
            using (EmailServiceClient emailClient = new EmailServiceClient())
            {
                EmailMessage email = new EmailMessage();
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApproverName", ClientCookie.UserNameENUS);
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("WorkflowName", "MajorLease");
                bodyValues.Add("StoreCode", storeBasic.StoreCode);
                bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
                bodyValues.Add("WorkflowName", Constants.MajorLease_ConsInvtChecking);////--流程名称
                bodyValues.Add("ProjectName", Constants.MajorLease);//项目名称
                var viewPage = string.Format("{0}/MajorLease/Main#/ConsInvtChecking/Process/Approval?projectId={1}&SN={2}&ProcInstID={3}",
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
        [Route("api/MajorLease/ConsInvtChecking/RejectConsInvtChecking")]
        public IHttpActionResult RejectConsInvtChecking(MajorLeaseConsInvtChecking entity)
        {
            entity.RejectConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/ConsInvtChecking/ReturnConsInvtChecking")]
        public IHttpActionResult ReturnConsInvtChecking(MajorLeaseConsInvtChecking entity)
        {
            entity.ReturnConsInvtChecking(ClientCookie.UserCode);

            return Ok(entity);
        }

        #endregion

        #region GBMemo
        [HttpGet]
        [Route("api/MajorLease/GBMemo/GetGBMemoInfo")]
        public IHttpActionResult GetGBMemoInfo(string projectId, string entityId = "")
        {
            var memo = MajorLeaseGBMemo.GetGBMemo(projectId, entityId);
            return Ok(memo);
        }
        [HttpPost]
        [Route("api/MajorLease/GBMemo/SaveGBMemo")]
        public IHttpActionResult SaveGBMemo(MajorLeaseGBMemo memo)
        {
            memo.Save();
            return Ok();
        }

        [HttpPost]
        [Route("api/MajorLease/GBMemo/SubmitGBMemo")]
        public IHttpActionResult SubmitGBMemo(MajorLeaseGBMemo memo)
        {
            memo.Submit();
            return Ok();
        }
        [HttpPost]
        [Route("api/MajorLease/GBMemo/ResubmitGBMemo")]
        public IHttpActionResult ResubmitGBMemo(MajorLeaseGBMemo entity)
        {
            entity.Resubmit(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/GBMemo/ApproveGBMemo")]
        public IHttpActionResult ApproveGBMemo(MajorLeaseGBMemo entity)
        {
            entity.Approve(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/MajorLease/GBMemo/RecallGBMemo")]
        public IHttpActionResult RecallGBMemo(MajorLeaseGBMemo entity)
        {
            entity.Recall(entity.Comments);
            return Ok();
        }

        [HttpPost]
        [Route("api/MajorLease/GBMemo/EditGBMemo")]
        public IHttpActionResult EditGBMemo(MajorLeaseGBMemo entity)
        {
            var taskUrl = entity.Edit();
            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }

        [HttpPost]
        [Route("api/MajorLease/GBMemo/ReturnGBMemo")]
        public IHttpActionResult ReturnGBMemo(MajorLeaseGBMemo entity)
        {
            entity.Return(ClientCookie.UserCode);
            return Ok(entity);
        }

        [Route("api/MajorLease/GBMemo/NotifyGBMemo")]
        [HttpPost]
        public IHttpActionResult NotifyGBMemo(PostMemo<MajorLeaseGBMemo> postData)
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

                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.GBMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.Store.StoreBasicInfo.StoreCode);
                bodyValues.Add("StoreName", postData.Entity.Store.StoreBasicInfo.NameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", FlowCode.MajorLease_GBMemo); ////--流程名称
                bodyValues.Add("ProjectName", Constants.MajorLease); //项目名称

                string viewPage = string.Format("{0}/MajorLease/Main#/GBMemo/Process/View?projectId={1}",
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
                    string strTitle = FlowCode.MajorLease_GBMemo;
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
                ProjectInfo.FinishNode(postData.Entity.ProjectId, FlowCode.MajorLease_GBMemo, NodeCode.Finish);
                AttachmentsMemoProcessInfo.UpdateNotifyDate(postData.Entity.ProjectId, FlowCode.GBMemo);
                tranScope.Complete();
            }
            return Ok();
        }
        #endregion
    }
}