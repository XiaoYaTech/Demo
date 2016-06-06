using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;
using Mcdonalds.AM.Services.Common;

namespace Mcdonalds.AM.DataAccess
{
    public partial class MajorLeaseChangePackage : BaseWFEntity<MajorLeaseChangePackage>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_Package; }
        }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<bool> ChangeRentalType { get; set; }
        public string ChangeRentalTypeDESC { get; set; }
        public Nullable<bool> ChangeRedLineType { get; set; }
        public string ChangeRedLineTypeDESC { get; set; }
        public Nullable<bool> ChangeLeaseTermType { get; set; }
        public string ChangeLeaseTermDESC { get; set; }
        public string USCode { get; set; }
        public MajorLeaseInfo MajorInfo { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public bool IsShowSave { get; set; }

        public bool IsShowReject { get; set; }

        public bool EnableAssetMgrUpload { get; set; }

        public ProjectContractRevision ProjectContractRevision { get; set; }

        public static string AssetMgrCode
        {
            get
            {
                return ConfigurationManager.AppSettings["AssetMgrCode"];
            }
        }

        public override string TableName
        {
            get { return "MajorLeaseChangePackage"; }
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetMajorPackageInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public static MajorLeaseChangePackage GetMajorPackageInfo(string strProjectId, string entityId = "")
        {
            var entity = string.IsNullOrEmpty(entityId) ?
                Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault()
                : Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == strProjectId);


            if (entity == null)
            {
                entity = new MajorLeaseChangePackage();
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
                entity.ProjectId = strProjectId;
                entity.MajorInfo = majorLeaseInfo;

                entity.ProjectContractRevision = StoreContractInfo.MappingProjectContractRevision(majorLeaseInfo.USCode);

                if (entity.ProjectContractRevision != null)
                {
                    entity.ProjectContractRevision.ProjectId = strProjectId;
                    entity.MajorInfo.OldChangeLeaseTermExpiraryDate = entity.ProjectContractRevision.LeaseChangeExpiryOld;
                    if (!string.IsNullOrEmpty(entity.ProjectContractRevision.RedlineAreaOld))
                    {
                        entity.MajorInfo.OldChangeRedLineRedLineArea = Convert.ToDecimal(entity.ProjectContractRevision.RedlineAreaOld);
                    }
                    entity.MajorInfo.OldLandlord = entity.ProjectContractRevision.LandlordOld;
                    entity.MajorInfo.OldRentalStructure = entity.ProjectContractRevision.RentStructureOld;
                }
            }
            else
            {
                entity.MajorInfo = majorLeaseInfo;
                entity.ProjectContractRevision = ProjectContractRevision.Get(majorLeaseInfo.ProjectId);

                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == strProjectId
                                                               && e.FlowCode == FlowCode.MajorLease_Package);

                if (projectInfo != null)
                {

                    entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.MajorLease_Package);
                    entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.MajorLease_Package);
                    var includeRejctActivityList = new List<string>()
                        {
                            "VPGM",
                            "Dev VP",
                            "CDO",
                            "CFO",
                            "Managing Director"
                        };

                    entity.IsShowReject = TaskWork.Any(
                            e => e.RefID == strProjectId
                                && e.TypeCode == FlowCode.MajorLease_Package
                                && e.K2SN != null
                                && includeRejctActivityList.Contains(e.ActivityName)
                                && e.Status == TaskWorkStatus.UnFinish);

                }



                var condition = new ProjectCommentCondition();
                condition.SourceCode = FlowCode.MajorLease;
                condition.UserAccount = ClientCookie.UserCode;

                condition.RefTableId = entity.Id;
                condition.RefTableName = WFMajorLeasePackage.TableName;

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



            entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            entity.USCode = majorLeaseInfo.USCode;

            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.MajorLease_Package);
            entity.EnableAssetMgrUpload = SetEnableAssetMgrUpload();
            PopulateInitialValues(entity);

            PopulateAppUsers(entity);
            return entity;
        }

        private static bool SetEnableAssetMgrUpload()
        {
            var enable = false;
            if (ClientCookie.UserCode == AssetMgrCode)
            {
                enable = true;
            }
            return enable;
        }


        public override ApproveDialogUser GetApproveDialogUser()
        {
            return ApproveDialogUser.FirstOrDefault(e => e.RefTableID == Id.ToString());
        }

        private Boolean CheckIfNeedAssetMgrUpload()
        {
            if (NetWriteOff.HasValue)
            {
                if (NetWriteOff.Value >= Convert.ToDecimal(152.75 * 10000))
                    return true;
                return false;
            }
            return false;
        }

        private static void PopulateInitialValues(MajorLeaseChangePackage entity)
        {
            var consInfo = GetWorkflowEntity(entity.ProjectId, FlowCode.MajorLease_ConsInfo) as MajorLeaseConsInfo;
            if (consInfo != null)
            {
                var writeOff = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == consInfo.Id);
                if (writeOff != null
                    && !entity.WriteOff.HasValue)
                {
                    decimal totalWriteOff;
                    Decimal.TryParse(writeOff.TotalWriteOff, out totalWriteOff);
                    entity.WriteOff = totalWriteOff;
                }
                if (consInfo.ReinvenstmentType == 3
                     && !entity.NewInvestment.HasValue)
                {
                    var reinvestmentCost = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == consInfo.Id);
                    if (reinvestmentCost != null)
                    {
                        decimal totalReinvestmentCost;
                        Decimal.TryParse(reinvestmentCost.TotalReinvestmentBudget, out totalReinvestmentCost);
                        entity.NewInvestment = totalReinvestmentCost;
                    }
                }


            }

            if (entity.WriteOff == null)
            {
                entity.WriteOff = 0;
            }
            if (entity.NewInvestment == null)
            {
                entity.NewInvestment = 0;
            }
        }

        private static void PopulateAppUsers(MajorLeaseChangePackage entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.MarketMgrCode
                };
                entity.AppUsers.MarketMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RegionalMgrCode
                };
                entity.AppUsers.RegionalMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.DDCode
                };
                entity.AppUsers.DD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.GMCode
                };
                entity.AppUsers.GM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.FCCode
                };
                entity.AppUsers.FC = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.RDDCode
                };
                entity.AppUsers.RDD = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.VPGMCode
                };
                entity.AppUsers.VPGM = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CDOCode
                };
                entity.AppUsers.CDO = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.CFOCode
                };
                entity.AppUsers.CFO = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetMgrCode
                };
                entity.AppUsers.MCCLAssetMgr = simp;

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLAssetDtrCode
                };
                entity.AppUsers.MCCLAssetDtr = simp;

                if (!string.IsNullOrEmpty(approvedUsers.NecessaryNoticeUsers))
                {
                    var temp = approvedUsers.NecessaryNoticeUsers.Split(';');
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
                else
                {
                    entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.MajorLease_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
                }

                if (!string.IsNullOrEmpty(approvedUsers.NoticeUsers))
                {
                    var temp = approvedUsers.NoticeUsers.Split(';');
                    entity.AppUsers.NoticeUsers = Employee.GetSimpleEmployeeByCodes(temp);
                }
            }
            else
            {
                entity.AppUsers.NecessaryNoticeUsers = Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.MajorLease_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray());
            }
        }

        public void Submit()
        {
            Save();
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, FlowCode.MajorLease_Package);
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {

                Save("Submit");
                task.Finish();
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_Package, NodeCode.MajorLease_Package_Input);
                scope.Complete();
            }
        }
        private int? StartProcess(TaskWork task)
        {
            //var processDataFields = new List<ProcessDataField>()
            //{
            //    new ProcessDataField("dest_Creator", CreateUserAccount),
            //    new ProcessDataField("dest_MarketMgr", dest_MarketMgr),
            //    new ProcessDataField("dest_GMApprovers",dest_GMApprovers),
            //    new ProcessDataField("dest_VPGM",string.Join(";",AppUsers.VPGM.Code)),
            //    new ProcessDataField("dest_DevVP",string.Join(";",AppUsers.MCCLAssetDtr.Code)),
            //    new ProcessDataField("dest_CDO",string.Join(";",AppUsers.CDO.Code)),
            //    new ProcessDataField("dest_CFO",string.Join(";",AppUsers.CFO.Code)),
            //    new ProcessDataField("dest_MngDirector",string.Join(";",AppUsers.MCCLAssetDtr.Code)),
            //    new ProcessDataField("ProcessCode", WFMajorLeasePackage.ProcessCode),
            //    new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task))
            //};
            CreateUserAccount = ClientCookie.UserCode;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFMajorLeasePackage.ProcessCode, CreateUserAccount,
                processDataFields);
        }

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            if (actionName == ProjectAction.Return)
            {
                TaskWork.Finish(e => e.RefID == ProjectId
                    && e.TypeCode == WorkflowCode
                    && e.Status == TaskWorkStatus.UnFinish);
            }

            K2FxContext.Current.ApprovalProcess(sn, employeeCode, ParseActionName(actionName), comment, dataField);

            using (var scope = new TransactionScope())
            {
                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_Package_Input, GetProjectStatus(actionName));
                        break;
                }

                scope.Complete();
            }

        }

        private string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                case ProjectAction.ReSubmit:
                    nodeCode = NodeCode.MajorLease_Package_Examine;
                    break;
                default:
                    nodeCode = NodeCode.MajorLease_Package_Upload;
                    break;
            }
            return nodeCode;
        }

        public void ApprovePackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            string dest_MarketMgr = "";

            if (AppUsers.MarketMgr != null)
                dest_MarketMgr += AppUsers.MarketMgr.Code + ";";

            if (AppUsers.RegionalMgr != null)
                dest_MarketMgr += AppUsers.RegionalMgr.Code + ";";

            dest_MarketMgr = dest_MarketMgr.TrimEnd(';');

            string dest_GMApprovers = "";

            if (AppUsers.DD != null)
                dest_GMApprovers += AppUsers.DD.Code + ";";

            if (AppUsers.GM != null)
                dest_GMApprovers += AppUsers.GM.Code + ";";

            if (AppUsers.FC != null)
                dest_GMApprovers += AppUsers.FC.Code + ";";

            if (AppUsers.RDD != null)
                dest_GMApprovers += AppUsers.RDD.Code + ";";

            if (dest_GMApprovers.Length > 1)
                dest_GMApprovers = dest_GMApprovers.Substring(0, dest_GMApprovers.Length - 1);

            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MarketMgr", dest_MarketMgr),
                new ProcessDataField("dest_GMApprovers",dest_GMApprovers),
                new ProcessDataField("dest_VPGM",string.Join(";",AppUsers.VPGM.Code)),
                new ProcessDataField("dest_DevVP",string.Join(";",AppUsers.MCCLAssetDtr.Code)),
                new ProcessDataField("dest_CDO",string.Join(";",AppUsers.CDO.Code)),
                new ProcessDataField("dest_CFO",string.Join(";",AppUsers.CFO.Code)),
                new ProcessDataField("dest_MngDirector",string.Join(";",AppUsers.ManagingDirector.Code)),
                new ProcessDataField("dest_AssetMgr",string.Join(";",AssetMgrCode)),
                new ProcessDataField("ProcessCode", WFMajorLeasePackage.ProcessCode),
                new ProcessDataField("IsNeedCDOApproval",CheckIfNeedCDOApproval().ToString(),"BOOLEAN"),
                new ProcessDataField("IsNeedAssetMgrUpload",CheckIfNeedAssetMgrUpload().ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                if (majorLeaseInfo != null)
                {
                    if (majorLeaseInfo.ChangeRedLineType.HasValue && majorLeaseInfo.ChangeRedLineType.Value)
                    { //红线变更而且consinfo中没有layout文件时，需要生成consinfo任务给PM
                        var ci = MajorLeaseConsInfo.FirstOrDefault(e => e.ProjectId == this.ProjectId);
                        var storeLayoutId = new Guid("4E87C7EA-C4FD-4030-B73A-591D3459C3F6");
                        var cid = ci.Id.ToString();
                        if (Attachment.Count(at => at.RefTableID == cid && at.RequirementId == storeLayoutId) == 0)
                        {
                            var strURI = "/MajorLease/Main#/ConsInfo?projectId=" + ProjectId;
                            TaskWork.Add(MajorInfo.GenerateTaskWork(FlowCode.MajorLease_ConsInfo, "ConsInfo", "Cons Info", strURI));
                        }
                    }
                }


                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        private bool CheckIfNeedCDOApproval()
        {
            var isNeedCDOApproval = false;
            if (NetWriteOff.HasValue)
            {
                if (NetWriteOff.Value >= Convert.ToDecimal(61.1 * 10000))
                //&& NetWriteOff.Value <= Convert.ToDecimal(152.75 * 10000))
                { isNeedCDOApproval = true; }

            }
            return isNeedCDOApproval;
        }

        public void RejectPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Decline, Comments);
        }

        public void ReturnPackage(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }

        public void ResubmitPackage(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    CreateTime = DateTime.Now;
                    LastUpdateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;
                    CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    CreateUserNameENUS = ClientCookie.UserNameENUS;
                    IsHistory = false;
                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }

                SaveApproveUsers(action);
                if (!action.Equals("Edit"))
                {
                    SaveComments(action);
                }
                SaveProjectCtractRevision();
                scope.Complete();
            }
        }

        private void SaveProjectCtractRevision()
        {
            if (MajorInfo != null)
            {
                //ProjectContractRevision = MajorInfo.MappingProjectContractRevision(ProjectContractRevision);
                //if (ProjectContractRevision != null)
                //{
                //    ProjectContractRevision.Append();
                //}
                MajorInfo.Update();
            }
        }

        private void SaveApproveUsers(string actionName)
        {
            switch (actionName)
            {
                case ProjectAction.Submit:
                case ProjectAction.ReSubmit:
                    var approveUser = ApproveDialogUser.GetApproveDialogUser(Id.ToString());
                    if (approveUser == null)
                    {
                        approveUser = new ApproveDialogUser();
                        approveUser.Id = Guid.Empty;
                        approveUser.RefTableID = Id.ToString();
                        approveUser.ProjectId = ProjectId;
                        approveUser.FlowCode = FlowCode.MajorLease_Package;
                    }
                    approveUser.MarketMgrCode = AppUsers.MarketMgr.Code;

                    if (AppUsers.RegionalMgr != null)
                    {
                        approveUser.RegionalMgrCode = AppUsers.RegionalMgr.Code;
                    }
                    approveUser.DDCode = AppUsers.DD.Code;
                    approveUser.GMCode = AppUsers.GM.Code;
                    approveUser.FCCode = AppUsers.FC.Code;

                    if (AppUsers.RDD != null)
                    {
                        approveUser.RDDCode = AppUsers.RDD.Code;
                    }

                    approveUser.VPGMCode = AppUsers.VPGM.Code;

                    if (AppUsers.CDO != null)
                    {
                        approveUser.CDOCode = AppUsers.CDO.Code;
                    }

                    if (AppUsers.CFO != null)
                    {
                        approveUser.CFOCode = AppUsers.CFO.Code;
                    }

                    if (AppUsers.MCCLAssetMgr != null)
                    {
                        approveUser.MCCLAssetMgrCode = AppUsers.MCCLAssetMgr.Code;
                    }
                    if (AppUsers.MCCLAssetDtr != null)
                    {
                        approveUser.MCCLAssetDtrCode = AppUsers.MCCLAssetDtr.Code;
                    }
                    if (AppUsers.NecessaryNoticeUsers != null)
                        approveUser.NecessaryNoticeUsers = string.Join(";", AppUsers.NecessaryNoticeUsers.Select(u => u.Code).ToArray());
                    if (AppUsers.NoticeUsers != null)
                        approveUser.NoticeUsers = string.Join(";", AppUsers.NoticeUsers.Select(u => u.Code).ToArray());

                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }

        }

        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "MajorLeaseChangePackage", ClientCookie.UserCode);
            if (comment != null)
            {
                comment.Content = Comments;
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.Update();
            }
            else
            {
                comment = new ProjectComment();
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.ActivityName = "";
                comment.Content = Comments;
                comment.CreateTime = DateTime.Now;
                comment.CreateUserAccount = ClientCookie.UserCode;
                comment.CreateUserNameENUS = ClientCookie.UserNameENUS;
                comment.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                comment.UserAccount = ClientCookie.UserCode;
                comment.UserNameENUS = ClientCookie.UserNameENUS;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "MajorLeaseChangePackage";
                comment.SourceCode = FlowCode.MajorLease;
                comment.SourceNameZHCN = FlowCode.MajorLease;
                comment.SourceNameENUS = FlowCode.MajorLease;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstID;
                comment.Add();
            }
        }

        public static List<Attachment> GetPackageAgreementList(string projectId, string packageId, string uploadFilesURL)
        {
            string typeCode = "";
            string refTableId = "";
            var listReturn = new List<Attachment>();

            var legal = MajorLeaseLegalReview.GetLegalReviewInfo(projectId, "");
            if (legal != null)
            {
                refTableId = legal.Id.ToString();
                var listLegal = new List<Attachment>();
                var listTemp = Attachment.GetList("MajorLeaseLegalReview", refTableId, typeCode);
                foreach (var item in listTemp)
                {
                    if (item.TypeCode != "Contract")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        if (item.TypeCode == "LegalClearanceReport")
                            item.Name = "Legal clearance report";
                        else if (item.TypeCode == "Agreement")
                        {
                            item.Name = "Lease agreement";
                        }
                        else if (item.TypeCode == "Others")
                        {
                            item.Name = "Others（如往来函件，业主证明)";
                        }
                        listLegal.Add(item);
                    }
                }
                listReturn.AddRange(listLegal);
            }

            var finance = new MajorLeaseFinancAnalysis();
            finance = finance.GetFinanceInfo(projectId, "");
            if (finance != null)
            {
                refTableId = finance.Id.ToString();
                var listFinance = new List<Attachment>();
                var listTemp = Attachment.GetList("MajorLeaseFinancAnalysis", refTableId, "FinAgreement");
                foreach (var item in listTemp)
                {
                    if (item.TypeCode != "Attachment")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        item.Name = "Finance Analysts";
                        listFinance.Add(item);
                    }
                }
                listReturn.AddRange(listFinance);
            }

            var consinfo = new MajorLeaseConsInfo();
            consinfo = consinfo.GetConsInfo(projectId);
            if (consinfo != null)
            {
                var listConsInfo = new List<Attachment>();
                refTableId = consinfo.Id.ToString();
                var listTemp = Attachment.GetList("MajorLeaseConsInfo", refTableId, typeCode);
                foreach (var item in listTemp)
                {
                    if (item.TypeCode != "Attachment")
                    {
                        item.FileURL = uploadFilesURL + "/" + item.InternalName;
                        if (item.TypeCode == "ConsInfoAgreement")
                            item.Name = "Store Layout";
                        else if (item.TypeCode == "ReinCost")
                        {
                            item.Name = "Additional investment list";
                        }
                        else if (item.TypeCode == "WriteOff")
                        {
                            item.Name = "Write-off details";
                        }
                        listConsInfo.Add(item);
                    }
                }
                listReturn.AddRange(listConsInfo);
            }

            var listPackage = new List<Attachment>();
            var listTempPackage = Attachment.GetList("MajorLeaseChangePackage", packageId, typeCode);
            if (listTempPackage != null)
            {
                foreach (var item in listTempPackage)
                {
                    item.FileURL = uploadFilesURL + "/" + item.InternalName;
                    if (item.TypeCode == "SignedApproval")
                    {
                        item.Name = "Signed Approval";
                    }
                    else if (item.TypeCode == "SignedAgreement")
                    {
                        item.Name = "Signed Agreement";
                    }
                    else if (item.TypeCode == ExcelDataInputType.MajorLeaseChangeCover.ToString())
                    {
                        item.Name = "Cover";
                    }
                    listPackage.Add(item);
                }
                listReturn.AddRange(listPackage);
            }

            listReturn.Add(new Attachment()
            {
                Name = "Right sizing",
                TypeCode = "RightSizing",
                FileURL = "#",
            });
            listReturn.Add(new Attachment()
            {
                Name = "Facade, signage rendering",
                TypeCode = "FacadeSignageRendering",
                FileURL = "#",
            });
            if (!listReturn.Where(e => e.TypeCode == ExcelDataInputType.MajorLeaseChangeCover.ToString()).Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Cover",
                    TypeCode = "Cover",
                    FileURL = "#",
                });
            }
            if (!listReturn.Where(e => e.TypeCode == "SignedApproval").Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Signed Approval",
                    TypeCode = "SignedApproval",
                    FileURL = "#",
                });
            }
            if (!listReturn.Where(e => e.TypeCode == "SignedAgreement").Any())
            {
                listReturn.Add(new Attachment()
                {
                    Name = "Signed Agreement",
                    TypeCode = "SignedAgreement",
                    FileURL = "#",
                });
            }
            return listReturn;
        }
        public void GenerateCoverEexcel(string templatePath, string tempFilePath)
        {
            //生成Cover Excecl 后，往Attachment表里插入一条记录。
            var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (projectInfo == null)
            {
                throw new Exception("Cannot find the project info!");
            }

            File.Copy(templatePath, tempFilePath);
            var fileInfo = new FileInfo(tempFilePath);
            var excelOutputDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.MajorLeaseChangeCover);

            var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == projectInfo.USCode);
            if (store == null)
            {
                throw new Exception("Cannot find Store info!");
            }

            var inputInfo = new ExcelInputDTO
            {
                Region = store.RegionENUS,
                Market = store.MarketENUS,
                Province = store.ProvinceENUS,
                StoreName = store.NameZHCN,
                City = store.CityENUS,
                USCode = store.StoreCode,
                StoreType = store.StoreType,
                OpenDate = store.OpenDate
            };

            excelOutputDirector.Input(inputInfo);

            var att = new Attachment
            {
                InternalName = fileInfo.Name,
                RefTableName = WFMajorLeasePackage.TableName,
                RefTableID = Id.ToString(),
                RelativePath = "//",
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                Length = (int)fileInfo.Length,
                CreateTime = DateTime.Now,
                ID = Guid.NewGuid(),
                TypeCode = "Cover",
                CreatorID = ClientCookie.UserCode,
                CreatorNameENUS = ClientCookie.UserNameENUS,
                CreatorNameZHCN = ClientCookie.UserNameZHCN,
                UpdateTime = DateTime.Now,
                IsDelete = 0,
                RequirementId = AttachmentRequirement.FirstOrDefault(e => e.FlowCode == FlowCode.MajorLease_Package && e.NameENUS == "Cover").Id
            };
            Attachment.SaveSigleFile(att);

        }

        public bool CheckIfUnfreezePackageWorkflow(string projectId)
        {
            var isUnfreeze = false;
            var flowCodeList = new List<string>()
            {
                FlowCode.MajorLease_FinanceAnalysis,
                FlowCode.MajorLease_LegalReview
            };

            var majorleaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == projectId);
            if (majorleaseInfo != null
                && majorleaseInfo.ChangeRedLineType.HasValue && majorleaseInfo.ChangeRedLineType.Value)
            {
                flowCodeList.Add(FlowCode.MajorLease_ConsInfo);
            }


            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId
                                                        && flowCodeList.Contains(e.FlowCode)).AsNoTracking().ToList();

            if (projectInfos.Count == flowCodeList.Count
                && projectInfos.All(e => e.Status == ProjectStatus.Finished))
            {
                isUnfreeze = true;
            }

            return isUnfreeze;
        }

        public void GeneratePackageTask(string projectId)
        {
            if (CheckIfUnfreezePackageWorkflow(projectId))
            {
                if (!TaskWork.Any(e => e.RefID == projectId
                        && e.TypeCode == WorkflowCode
                        && e.Status == TaskWorkStatus.UnFinish))
                {
                    var taskWork = new TaskWork();
                    taskWork.SourceCode = FlowCode.MajorLease;
                    taskWork.SourceNameENUS = FlowCode.MajorLease;
                    taskWork.SourceNameZHCN = FlowCode.MajorLease;
                    taskWork.Status = TaskWorkStatus.UnFinish;
                    taskWork.StatusNameZHCN = "任务";
                    taskWork.StatusNameENUS = "任务";
                    taskWork.RefID = projectId;
                    taskWork.Id = Guid.NewGuid();
                    taskWork.CreateTime = DateTime.Now;
                    var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == projectId);

                    if (majorLeaseInfo == null)
                    {
                        throw new Exception("Cannot find the relative major lease info!");
                    }

                    taskWork.Title = TaskWork.BuildTitle(projectId, majorLeaseInfo.StoreNameZHCN, majorLeaseInfo.StoreNameENUS);
                    taskWork.TypeCode = WorkflowCode;
                    taskWork.TypeNameENUS = WorkflowCode;
                    taskWork.TypeNameZHCN = WorkflowCode;
                    taskWork.ReceiverAccount = majorLeaseInfo.AssetActorAccount;
                    taskWork.ReceiverNameENUS = majorLeaseInfo.AssetActorNameENUS;
                    taskWork.ReceiverNameZHCN = majorLeaseInfo.AssetActorNameZHCN;
                    taskWork.Url = string.Format(@"/MajorLease/Main#/Package?projectId={0}", projectId);
                    taskWork.StoreCode = majorLeaseInfo.USCode;
                    taskWork.ActivityName = NodeCode.Start;
                    taskWork.ActionName = SetTaskActionName(projectId);
                    TaskWork.Add(taskWork);
                }
            }

        }

        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }

            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
                WFMajorLeasePackage.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/MajorLease/Main#/Package?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (majorLeaseInfo == null)
                {
                    throw new Exception("Could not find the Major Lease Info, please check it!");
                }
                var task = majorLeaseInfo.GenerateTaskWork(FlowCode.MajorLease_Package,
                     "LeaseChangePackage",
                     "LeaseChangePackage",
                     taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);

                IsHistory = true;
                Update(this);
                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == WFMajorLeasePackage.TableName).ToList();

                ProjectInfo.Reset(ProjectId, FlowCode.MajorLease_Package);
                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.MajorLease_Package, ProjectStatus.UnFinish);

                Mapper.CreateMap<MajorLeaseChangePackage, MajorLeaseChangePackage>();
                var form = Mapper.Map<MajorLeaseChangePackage>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

                var newAttachmentList = new List<Attachment>();
                Mapper.CreateMap<Attachment, Attachment>();
                foreach (var attachment in attachments)
                {
                    var newAttachment = Mapper.Map<Attachment>(attachment);
                    newAttachment.RefTableID = form.Id.ToString();
                    newAttachment.ID = Guid.NewGuid();
                    newAttachmentList.Add(newAttachment);
                }
                Attachment.Add(newAttachmentList.ToArray());
                scope.Complete();
            }

            return taskUrl;
        }

        public override void PrepareTask(TaskWork taskWork)
        {
            switch (taskWork.ActivityName)
            {
                case "Actor Upload":
                    ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_Package_Examine);
                    break;
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessDeclined:
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.MajorLease, ProjectStatus.Rejected);
                        break;
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_Package_Examine);
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_Package_Upload, ProjectStatus.Finished);

                        var majorLeaseConsInvtChecking = new MajorLeaseConsInvtChecking();
                        majorLeaseConsInvtChecking.ProjectId = task.RefID;
                        majorLeaseConsInvtChecking.GenerateConsInvtCheckingTask(true);

                        GenerateConstractInfoTask();
                        GenerateSiteInfoTask();

                        //GenerateActorUploadTask();
                        break;
                }

                scope.Complete();
            }
        }

        private void GenerateSiteInfoTask()
        {
            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (majorLeaseInfo == null)
            {
                throw new Exception("Cannot find the relative major lease info!");
            }
            if (!TaskWork.Any(e => e.RefID == ProjectId
                                   && e.TypeCode == FlowCode.MajorLease_SiteInfo
                                   && e.ReceiverAccount == majorLeaseInfo.PMAccount
                                   && e.Status == TaskWorkStatus.UnFinish))
            {
                var taskWork = new TaskWork();
                taskWork.SourceCode = FlowCode.MajorLease;
                taskWork.SourceNameENUS = FlowCode.MajorLease;
                taskWork.SourceNameZHCN = FlowCode.MajorLease;
                taskWork.Status = TaskWorkStatus.UnFinish;
                taskWork.StatusNameZHCN = "任务";
                taskWork.StatusNameENUS = "任务";
                taskWork.RefID = ProjectId;
                taskWork.Id = Guid.NewGuid();
                taskWork.CreateTime = DateTime.Now;

                taskWork.Title = TaskWork.BuildTitle(ProjectId, majorLeaseInfo.StoreNameZHCN,
                    majorLeaseInfo.StoreNameENUS);
                taskWork.TypeCode = FlowCode.MajorLease_SiteInfo;
                taskWork.TypeNameENUS = FlowCode.MajorLease_SiteInfo;
                taskWork.TypeNameZHCN = FlowCode.MajorLease_SiteInfo;
                taskWork.ReceiverAccount = majorLeaseInfo.PMAccount;
                taskWork.ReceiverNameENUS = majorLeaseInfo.PMNameENUS;
                taskWork.ReceiverNameZHCN = majorLeaseInfo.PMNameZHCN;
                taskWork.Url = string.Format(@"/MajorLease/Main#/SiteInfo/Process/Approval?projectId={0}&action={1}",
                    ProjectId, "Confirm");
                taskWork.StoreCode = majorLeaseInfo.USCode;
                taskWork.ActivityName = NodeCode.Start;
                taskWork.ActionName = SetTaskActionName(ProjectId);
                //TaskWork.Add(taskWork);
                //到达GB Date时间 发出任务
                if (majorLeaseInfo.GBDate.HasValue)
                    ScheduleLog.GenerateTaskSchedule(majorLeaseInfo.GBDate.Value, taskWork, ClientCookie.UserCode, ProjectId, FlowCode.MajorLease_SiteInfo, majorLeaseInfo.USCode);
            }
        }

        private void GenerateActorUploadTask()
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.MajorLease;
            taskWork.SourceNameENUS = FlowCode.MajorLease;
            taskWork.SourceNameZHCN = FlowCode.MajorLease;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = ProjectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;
            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);

            if (majorLeaseInfo == null)
            {
                throw new Exception("Cannot find the relative major lease info!");
            }

            taskWork.Title = TaskWork.BuildTitle(ProjectId, majorLeaseInfo.StoreNameZHCN, majorLeaseInfo.StoreNameENUS);
            taskWork.TypeCode = FlowCode.MajorLease_Package;
            taskWork.TypeNameENUS = FlowCode.MajorLease_Package;
            taskWork.TypeNameZHCN = FlowCode.MajorLease_Package;
            taskWork.ReceiverAccount = CreateUserAccount;
            taskWork.ReceiverNameENUS = CreateUserNameENUS;
            taskWork.ReceiverNameZHCN = CreateUserNameZHCN;
            taskWork.Url = string.Format(@"/MajorLease/Main#/Package/Process/Upload?projectId={0}", ProjectId);
            taskWork.StoreCode = majorLeaseInfo.USCode;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            TaskWork.Add(taskWork);
        }

        private void GenerateConstractInfoTask()
        {
            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);

            if (majorLeaseInfo == null)
            {
                throw new Exception("Cannot find the relative major lease info!");
            }
            if (!TaskWork.Any(e => e.RefID == ProjectId
                                   && e.TypeCode == FlowCode.MajorLease_ContractInfo
                                   && e.ReceiverAccount == majorLeaseInfo.AssetActorAccount
                                   && e.Status == TaskWorkStatus.UnFinish))
            {
                var taskWork = new TaskWork();
                taskWork.SourceCode = FlowCode.MajorLease;
                taskWork.SourceNameENUS = FlowCode.MajorLease;
                taskWork.SourceNameZHCN = FlowCode.MajorLease;
                taskWork.Status = TaskWorkStatus.UnFinish;
                taskWork.StatusNameZHCN = "任务";
                taskWork.StatusNameENUS = "任务";
                taskWork.RefID = ProjectId;
                taskWork.Id = Guid.NewGuid();
                taskWork.CreateTime = DateTime.Now;


                taskWork.Title = TaskWork.BuildTitle(ProjectId, majorLeaseInfo.StoreNameZHCN, majorLeaseInfo.StoreNameENUS);
                taskWork.TypeCode = FlowCode.MajorLease_ContractInfo;
                taskWork.TypeNameENUS = FlowCode.MajorLease_ContractInfo;
                taskWork.TypeNameZHCN = FlowCode.MajorLease_ContractInfo;
                taskWork.ReceiverAccount = majorLeaseInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = majorLeaseInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = majorLeaseInfo.AssetActorNameZHCN;
                taskWork.Url = string.Format(@"/MajorLease/Main#/ContractInfo/Process/Approval?projectId={0}&action={1}", ProjectId, "Confirm");
                taskWork.StoreCode = majorLeaseInfo.USCode;
                taskWork.ActivityName = NodeCode.Start;
                taskWork.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(taskWork);
            }

        }

        public void Confirm(string userCode)
        {
            //var task = TaskWork.FirstOrDefault(e => e.RefID == ProjectId
            //                                     && e.TypeCode == WorkflowCode
            //                                     && e.ReceiverAccount == ClientCookie.UserCode);
            //if (task != null)
            //{
            //    task.Status = TaskWorkStatus.Finished;
            //}
            //TaskWork.Update(task);

            ExecuteProcess(userCode, SerialNumber, "Confirm", Comments);
        }

        public void CompleteActorPackageTask(string actor)
        {
            var task = TaskWork.FirstOrDefault(e => e.RefID == ProjectId
                                                    && e.TypeCode == WorkflowCode
                                                    && e.ReceiverAccount == actor
                                                    && e.Status == TaskWorkStatus.UnFinish);
            if (task != null)
            {
                task.Status = TaskWorkStatus.Finished;
                TaskWork.Update(task);
            }
        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var project = ProjectInfo.Get(this.ProjectId, FlowCode.MajorLease_Package);
            var storeBasic = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
            var storeContract = StoreContractInfo.Search(c => c.StoreCode == project.USCode).OrderByDescending(c => c.CreatedTime).FirstOrDefault();
            var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            var majorLeasePackage = MajorLeaseChangePackage.GetMajorPackageInfo(ProjectId);
            var majorLease = MajorLeaseInfo.FirstOrDefault(tc => tc.ProjectId == ProjectId);
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            templateFileds.Add("WorkflowName", SystemCode.Instance.GetCodeName(FlowCode.MajorLease, ClientCookie.Language));
            templateFileds.Add("ProjectID", ProjectId);
            templateFileds.Add("USCode", storeBasic.StoreCode);
            templateFileds.Add("Region", storeBasic.Region);
            templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
            templateFileds.Add("Market", storeBasic.Market);
            templateFileds.Add("City", storeBasic.CityZHCN);
            templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
            //templateFileds.Add("StoreAge", Math.Floor((DateTime.Now - storeBasic.OpenDate).TotalDays / 365D).ToString());
            templateFileds.Add("Address", storeBasic.AddressZHCN);
            templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
            var storeInfo = StoreBasicInfo.GetStore(project.USCode);
            templateFileds.Add("CurrentLeaseENDYear", storeInfo.StoreContractInfo.EndDate.HasValue ? storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd") : "");
            templateFileds.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            templateFileds.Add("AssetsActor", assetActor.UserNameENUS);
            templateFileds.Add("AssetsRep", storeInfo.StoreDevelop.AssetRepName);
            templateFileds.Add("ClosureDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");

            bool changeFlag = false;
            if (majorLease.ChangeRentalType.HasValue && majorLease.ChangeRentalType.Value)
            {
                templateFileds.Add("TheChangeOfTheRental", "The Change of the rental");
                templateFileds.Add("OldRentalStructure", "Old Rental Structure:" + majorLease.OldRentalStructure);
                templateFileds.Add("NewRentalStructure", "New Rental Structure:" + majorLease.NewRentalStructure);
                changeFlag = true;
            }
            else
            {
                templateFileds.Add("TheChangeOfTheRental", "");
                templateFileds.Add("OldRentalStructure", "");
                templateFileds.Add("NewRentalStructure", "");
            }

            if (majorLease.ChangeRedLineType.HasValue && majorLease.ChangeRedLineType.Value)
            {
                templateFileds.Add("TheChangeOfRedLine", "The Change of red line");
                templateFileds.Add("OldChangeRedLineRedLineArea", majorLease.OldChangeRedLineRedLineArea.HasValue ? "Old Redline Area (sqm):" + majorLease.OldChangeRedLineRedLineArea.Value : "");
                templateFileds.Add("NewChangeRedLineRedLineArea", majorLease.NewChangeRedLineRedLineArea.HasValue ? "New Redline Area (sqm):" + majorLease.NewChangeRedLineRedLineArea.Value : "");
                changeFlag = true;
            }
            else
            {
                templateFileds.Add("TheChangeOfRedLine", "");
                templateFileds.Add("OldChangeRedLineRedLineArea", "");
                templateFileds.Add("NewChangeRedLineRedLineArea", "");
            }

            if (majorLease.ChangeLeaseTermType.HasValue && majorLease.ChangeLeaseTermType.Value)
            {
                templateFileds.Add("TheChangeOfLeaseTeam", "The Change of lease term");
                templateFileds.Add("OldChangeLeaseTermExpiraryDate", majorLease.OldChangeLeaseTermExpiraryDate.HasValue ? "Old Lease Change Expiry (Date):" + majorLease.OldChangeLeaseTermExpiraryDate.Value.ToString("yyyy-MM-dd") : "");
                templateFileds.Add("NewChangeLeaseTermExpiraryDate", majorLease.NewChangeLeaseTermExpiraryDate.HasValue ? "New Lease Change Expiry (Date):" + majorLease.NewChangeLeaseTermExpiraryDate.Value.ToString("yyyy-MM-dd") : "");
                changeFlag = true;
            }
            else
            {
                templateFileds.Add("TheChangeOfLeaseTeam", "");
                templateFileds.Add("OldChangeLeaseTermExpiraryDate", "");
                templateFileds.Add("NewChangeLeaseTermExpiraryDate", "");
            }

            if (majorLease.ChangeLandlordType.HasValue && majorLease.ChangeLandlordType.Value)
            {
                templateFileds.Add("TheChangeOfLandlord", "The change of Landlord");
                templateFileds.Add("OldLandlord", "Old Landlord:" + majorLease.OldLandlord);
                templateFileds.Add("NewLandlord", "New Landlord:" + majorLease.NewLandlord);
                changeFlag = true;
            }
            else
            {
                templateFileds.Add("TheChangeOfLandlord", "");
                templateFileds.Add("OldLandlord", "");
                templateFileds.Add("NewLandlord", "");
            }

            if (majorLease.ChangeOtherType.HasValue && majorLease.ChangeOtherType.Value)
            {
                templateFileds.Add("TheChangeOfOthers", "Others");
                templateFileds.Add("Others", majorLease.Others);
                changeFlag = true;
            }
            else
            {
                templateFileds.Add("TheChangeOfOthers", "");
                templateFileds.Add("Others", "");
            }

            if (changeFlag)
            {
                templateFileds.Add("LeaseChangeDescription", "Description:" + majorLease.LeaseChangeDescription);
            }
            else
            {
                templateFileds.Add("LeaseChangeDescription", "");
            }
            if (majorLeasePackage.WriteOff.HasValue)
                templateFileds.Add("WriteOff", majorLeasePackage.WriteOff.HasValue ? DataConverter.ToMoney(majorLeasePackage.WriteOff.Value) : " ");
            else
                templateFileds.Add("WriteOff", "");

            if (majorLeasePackage.CashCompensation.HasValue)
                templateFileds.Add("CashCompensation", majorLeasePackage.CashCompensation.HasValue ? DataConverter.ToMoney(majorLeasePackage.CashCompensation.Value) : " ");
            else
                templateFileds.Add("CashCompensation", "");

            if (majorLeasePackage.NetWriteOff.HasValue)
                templateFileds.Add("NetWriteOff", majorLeasePackage.NetWriteOff.HasValue ? DataConverter.ToMoney(majorLeasePackage.NetWriteOff.Value) : " ");
            else
                templateFileds.Add("NetWriteOff", "");

            if (majorLeasePackage.NewInvestment.HasValue)
                templateFileds.Add("NewInvestment", majorLeasePackage.NewInvestment.HasValue ? DataConverter.ToMoney(majorLeasePackage.NewInvestment.Value) : " ");
            else
                templateFileds.Add("NewInvestment", "");

            if (majorLeasePackage.CashFlowNVPCurrent.HasValue)
                templateFileds.Add("CashFlowNVPCurrent", majorLeasePackage.CashFlowNVPCurrent.HasValue ? DataConverter.ToMoney(majorLeasePackage.CashFlowNVPCurrent.Value) : " ");
            else
                templateFileds.Add("CashFlowNVPCurrent", "");

            if (majorLeasePackage.CashFlowNVPAfterChange.HasValue)
                templateFileds.Add("CashFlowNVPAfterChange", majorLeasePackage.CashFlowNVPAfterChange.HasValue ? DataConverter.ToMoney(majorLeasePackage.CashFlowNVPAfterChange.Value) : " ");
            else
                templateFileds.Add("CashFlowNVPAfterChange", "");

            if (majorLeasePackage.OtherCompensation.HasValue)
                templateFileds.Add("OtherCompensation", majorLeasePackage.OtherCompensation.HasValue ? DataConverter.ToMoney(majorLeasePackage.OtherCompensation.Value) : " ");
            else
                templateFileds.Add("OtherCompensation", "");

            if (majorLeasePackage.NetGain.HasValue)
                templateFileds.Add("NetGain", majorLeasePackage.NetGain.HasValue ? DataConverter.ToMoney(majorLeasePackage.NetGain.Value) : " ");
            else
                templateFileds.Add("NetGain", "");

            templateFileds.Add("ReasonDesc", majorLeasePackage.ReasonDesc);
            templateFileds.Add("OtherCompensationDescription", majorLeasePackage.OtherCompenDesc);
            templateFileds.Add("DecisionLogicRecomendation", majorLeasePackage.DecisionLogicRecomendation);
            return templateFileds;
        }

        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            var packageApprovers =
                                prevApprovers.FirstOrDefault(e => e.FlowCode == WorkflowCode
                                                                && e.RefTableID == Id.ToString());


            foreach (var taskWork in taskWorks.Where(e => e.TypeCode == WorkflowCode).ToList())
            {
                List<ProcessDataField> dataFields;
                AppUsers = currApprover;
                dataFields = SetWorkflowDataFields(null);

                if (packageApprovers != null)
                {
                    SimpleEmployee receiver = null;
                    if (taskWork.ReceiverAccount == packageApprovers.MarketMgrCode
                        && currApprover.MarketMgr != null
                        && packageApprovers.MarketMgrCode != currApprover.MarketMgr.Code)
                    {
                        receiver = currApprover.MarketMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RegionalMgrCode
                        && currApprover.RegionalMgr != null
                        && packageApprovers.RegionalMgrCode != currApprover.RegionalMgr.Code)
                    {
                        receiver = currApprover.RegionalMgr;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.MDDCode
                        && currApprover.MDD != null
                         && packageApprovers.MDDCode != currApprover.MDD.Code)
                    {
                        receiver = currApprover.MDD;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.GMCode
                        && currApprover.GM != null
                         && packageApprovers.GMCode != currApprover.GM.Code)
                    {
                        receiver = currApprover.GM;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.FCCode
                        && currApprover.FC != null
                         && packageApprovers.FCCode != currApprover.FC.Code)
                    {
                        receiver = currApprover.FC;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.RDDCode
                        && currApprover.RDD != null
                         && packageApprovers.RDDCode != currApprover.RDD.Code)
                    {
                        receiver = currApprover.RDD;
                    } if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
                        && currApprover.VPGM != null
                         && packageApprovers.VPGMCode != currApprover.VPGM.Code)
                    {
                        receiver = currApprover.VPGM;

                    } if (taskWork.ReceiverAccount == packageApprovers.MCCLAssetDtrCode
                        && currApprover.MCCLAssetDtr != null
                         && packageApprovers.MCCLAssetDtrCode != currApprover.MCCLAssetDtr.Code)
                    {
                        receiver = currApprover.MCCLAssetDtr;
                    }

                    if (taskWork.ReceiverAccount == packageApprovers.CDOCode
                        && currApprover.CDO != null
                         && packageApprovers.CDOCode != currApprover.CDO.Code)
                    {
                        receiver = currApprover.CDO;
                    }
                    if (taskWork.ReceiverAccount == packageApprovers.CFOCode
                        && currApprover.CFO != null
                         && packageApprovers.CFOCode != currApprover.CFO.Code)
                    {
                        receiver = currApprover.CFO;
                    }
                    else
                    {
                        if (taskWork.ProcInstID.HasValue)
                        {
                            UpdateWorkflowDataField(taskWork.ProcInstID.Value, dataFields);
                        }
                    }

                    if (receiver != null)
                    {
                        try
                        {
                            RedirectWorkflowTask(taskWork.K2SN, taskWork.ReceiverAccount, receiver.Code, dataFields);
                        }
                        catch (Exception ex)
                        {
                            Log4netHelper.WriteError(string.Format("【Redirect Workflow Task Error】：{0}",
                                JsonConvert.SerializeObject(taskWork)));
                        }
                        finally
                        {
                            taskWork.ReceiverAccount = receiver.Code;
                            taskWork.ReceiverNameENUS = receiver.NameENUS;
                            taskWork.ReceiverNameZHCN = receiver.NameZHCN;
                        }

                    }


                }
            }

            if (packageApprovers != null)
            {
                if (currApprover.MarketMgr != null)
                {
                    packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                }
                if (currApprover.RegionalMgr != null)
                {
                    packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                }
                if (currApprover.DD != null)
                {
                    packageApprovers.DDCode = currApprover.DD.Code;
                }

                if (currApprover.GM != null)
                {
                    packageApprovers.GMCode = currApprover.GM.Code;
                }

                if (currApprover.FC != null)
                {
                    packageApprovers.FCCode = currApprover.FC.Code;
                }

                if (currApprover.VPGM != null)
                {
                    packageApprovers.VPGMCode = currApprover.VPGM.Code;
                }

                if (currApprover.RDD != null)
                {
                    packageApprovers.RDDCode = currApprover.RDD.Code;
                }

                if (currApprover.MCCLAssetDtr != null)
                {
                    packageApprovers.MCCLAssetDtrCode = currApprover.MCCLAssetDtr.Code;
                }

                if (currApprover.CDO != null)
                {
                    packageApprovers.CDOCode = currApprover.CDO.Code;
                }

                if (currApprover.CFO != null)
                {
                    packageApprovers.CFOCode = currApprover.CFO.Code;
                }

            }

        }
    }

    public class WFMajorLeasePackage
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\Package";
        public const string ProcessCode = @"MCD_AM_MLC_P";
        public const string TableName = "MajorLeaseChangePackage";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名

        #endregion
    }
}
