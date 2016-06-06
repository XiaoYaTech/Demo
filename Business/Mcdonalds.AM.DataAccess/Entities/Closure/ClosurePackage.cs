using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.Services.Common;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosurePackage : BaseWFEntity<ClosurePackage>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.Closure_ClosurePackage; }
        }
        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Closure_Package"; }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string MarketMgr { get; set; }
        public string MarketMgrCode { get; set; }
        public string RegionalMgr { get; set; }
        public string RegionalMgrCode { get; set; }
        public string DD_GM_FC_RDD { get; set; }
        public string DD { get; set; }
        public string GM { get; set; }
        public string FC { get; set; }
        public string RDD { get; set; }
        public string VPGM { get; set; }
        public string Dev_VP { get; set; }
        public string CDO { get; set; }
        public string CFO { get; set; }
        public string MngDirector { get; set; }
        public string MDD { get; set; }
        public string Director { get; set; }

        public string MCCLAssetMgr { get; set; }
        public string MCCLAssetDtr { get; set; }
        public List<SimpleEmployee> NoticeUsers { get; set; }
        public List<SimpleEmployee> NecessaryNoticeUsers { get; set; }
        public ApproveUsers AppUsers { get; set; }

        public string UserAccount { get; set; }
        public string UserName { get; set; }
        public string UserNameENUS { get; set; }
        public string UserNameZHCN { get; set; }

        public const string TableName = "ClosurePackage";

        public static ClosurePackage Get(string projectId, string id = "")
        {
            ClosurePackage entity = null;
            if (!string.IsNullOrEmpty(id))
            {
                entity = FirstOrDefault(e => e.Id == new Guid(id));
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                entity = FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            }
            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            }
            return entity;
        }

        public override void PrepareTask(TaskWork taskWork)
        {
            if (taskWork.ActivityName == "Asset Rep Upload")
            {
                ProjectInfo.FinishNode(ProjectId, FlowCode.Closure_ClosurePackage, NodeCode.Closure_ClosurePackage_OnlineApprove);
            }
        }

        public static ClosurePackage GetHistory(string projectId)
        {
            ClosurePackage entity = null;
            var packageList = Search(e => e.ProjectId == projectId && e.IsHistory == true).OrderByDescending(i => i.CreateTime);
            if (packageList.Count() > 0)
            {
                entity = packageList.First();
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            }
            return entity;
        }

        public static ClosurePackage GetByProcInstID(int ProcInstID)
        {
            return FirstOrDefault(e => e.ProcInstID == ProcInstID);
        }

        public static ClosurePackage GetByProcInstID(int ProcInstID, bool isHistory)
        {
            return FirstOrDefault(e => e.ProcInstID == ProcInstID && e.IsHistory == isHistory);
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = Get(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public static List<Attachment> QueryAttList(string projectId)
        {
            string sql = string.Format(@"
                    SELECT * FROM [Attachment]
                                    WHERE TypeCode in 
				                    ('Template','Cover','FinAgreement','SignedTerminationAgreement' ,'SignedPackage','ClosureTool','Others')
				  AND RefTableID IN
                 (
			        SELECT id FROM dbo.ClosureWOCheckList
                    WHERE ProjectId = '{0}'
                    UNION 
                    SELECT id FROM dbo.ClosureTool
                    WHERE ProjectId = '{0}'
                    UNION
                    SELECT id FROM dbo.ClosureExecutiveSummary
                    WHERE ProjectId = '{0}'
                    UNION
                    SELECT id FROM dbo.ClosurePackage
                    WHERE ProjectId =  '{0}'
                    UNION
                    SELECT id FROM ClosureLegalReview
                    WHERE ProjectId = '{0}'
				)", projectId);
            var list = SqlQuery<Attachment>(sql, null).ToList();
            return list;
        }

        private int Save()
        {
            var result = 0;

            if (!Any(p => p.Id == this.Id))
            {
                this.IsHistory = false;
                this.CreateUserAccount = ClientCookie.UserCode;
                this.CreateTime = DateTime.Now;
                result = Add(this);
            }
            else
            {
                this.LastUpdateTime = DateTime.Now;
                this.LastUpdateUserAccount = ClientCookie.UserCode;
                result = Update(this);
            }
            return result;
        }

        public void SaveApproveUsers(string actionName)
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
                        approveUser.FlowCode = FlowCode.Closure_ClosurePackage;
                    }
                    if (!string.IsNullOrEmpty(MarketMgrCode))
                    {
                        approveUser.MarketMgrCode = MarketMgrCode;
                    }
                    else
                    {
                        approveUser.MarketMgrCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(RegionalMgrCode))
                    {
                        approveUser.RegionalMgrCode = RegionalMgrCode;
                    }
                    else
                    {
                        approveUser.RegionalMgrCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(MDD))
                    {
                        approveUser.MDDCode = MDD;
                    }
                    else
                    {
                        approveUser.MDDCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(GM))
                    {
                        approveUser.GMCode = GM;
                    }
                    else
                    {
                        approveUser.GMCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(FC))
                    {
                        approveUser.FCCode = FC;
                    }
                    else
                    {
                        approveUser.FCCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(RDD))
                    {
                        approveUser.RDDCode = RDD;
                    }
                    else
                    {
                        approveUser.RDDCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(VPGM))
                    {
                        approveUser.VPGMCode = VPGM;
                    }
                    else
                    {
                        approveUser.VPGMCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(CDO))
                    {
                        approveUser.CDOCode = CDO;
                    }
                    else
                    {
                        approveUser.CDOCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(CFO))
                    {
                        approveUser.CFOCode = CFO;
                    }
                    else
                    {
                        approveUser.CFOCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(MngDirector))
                    {
                        approveUser.MngDirectorCode = MngDirector;
                    }
                    else
                    {
                        approveUser.MngDirectorCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(MCCLAssetMgr))
                    {
                        approveUser.MCCLAssetMgrCode = MCCLAssetMgr;
                    }
                    else
                    {
                        approveUser.MCCLAssetMgrCode = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(MCCLAssetDtr))
                    {
                        approveUser.MCCLAssetDtrCode = MCCLAssetDtr;
                    }
                    else
                    {
                        approveUser.MCCLAssetDtrCode = string.Empty;
                    }
                    if (NoticeUsers != null && NoticeUsers.Count > 0)
                    {
                        approveUser.NoticeUsers = string.Join(";", NoticeUsers.Select(e => e.Code).ToArray());
                    }
                    else
                    {
                        approveUser.NoticeUsers = string.Empty;
                    }
                    if (NecessaryNoticeUsers != null && NecessaryNoticeUsers.Count > 0)
                    {
                        approveUser.NecessaryNoticeUsers = string.Join(";", NecessaryNoticeUsers.Select(e => e.Code).ToArray());
                    }
                    else
                    {
                        approveUser.NecessaryNoticeUsers = string.Empty;
                    }
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }
        public string Action
        {
            get;
            set;
        }

        public string Comments
        {
            get;
            set;
        }

        public string SN
        {
            get;
            set;
        }

        public bool showButton
        {
            get;
            set;
        }


        public override string Edit()
        {
            //取消未处理完成的任务（主要是Package最后一个环节办理完后，不走K2流程生成的1条回到Actor的任务）
            var oldTask = TaskWork.Search(i => i.SourceCode == FlowCode.Closure && i.TypeCode == FlowCode.Closure_ClosurePackage && i.RefID == this.ProjectId && i.Status == TaskWorkStatus.UnFinish).ToArray();
            foreach (var taskItem in oldTask)
            {
                taskItem.Status = TaskWorkStatus.Cancel;
            }
            TaskWork.Update(oldTask);

            //var taskWork = TaskWork.FirstOrDefault(e => e.ReceiverAccount == ClientCookie.UserCode
            //            && e.SourceCode == FlowCode.Closure
            //            && e.TypeCode == FlowCode.Closure_ClosurePackage && e.RefID == this.ProjectId);


            //taskWork.Status = TaskWorkStatus.UnFinish;
            //taskWork.StatusNameZHCN = "任务";
            //taskWork.StatusNameENUS = "任务";

            //var closureEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            //taskWork.ReceiverAccount = closureEntity.AssetActorAccount;
            //taskWork.ReceiverNameENUS = closureEntity.AssetActorNameENUS;
            //taskWork.ReceiverNameZHCN = closureEntity.AssetActorNameZHCN;
            //taskWork.Id = Guid.NewGuid();
            //taskWork.CreateTime = DateTime.Now;
            //taskWork.Url = TaskWork.BuildUrl(FlowCode.Closure_ClosurePackage, this.ProjectId, "");
            //taskWork.ActivityName = NodeCode.Start;
            //taskWork.ActionName = SetTaskActionName(ProjectId);
            //taskWork.ProcInstID = null;
            //taskWork.FinishTime = null;
            //TaskWork.Add(taskWork);
            var url = GeneratePackageTask(ProjectId);

            this.IsHistory = true;

            this.Save();

            var objectCopy = new ObjectCopy();
            var newEntity = objectCopy.AutoCopy(this);
            newEntity.Id = Guid.NewGuid();
            newEntity.ProcInstID = 0;
            newEntity.IsHistory = false;
            newEntity.Add();

            var projectEntity = ProjectInfo.Get(this.ProjectId, FlowCode.Closure_ClosurePackage);
            projectEntity.Status = ProjectStatus.UnFinish;

            ProjectInfo.Update(projectEntity);
            ProjectInfo.Reset(ProjectId, FlowCode.Closure_ClosurePackage);
            ProjectInfo.Reset(ProjectId, FlowCode.Closure);
            var attList = Attachment.Search(e => e.RefTableID == this.Id.ToString()
                                   && e.RefTableName == ClosurePackage.TableName).AsNoTracking().ToList();

            var newList = new List<Attachment>();
            foreach (var att in attList)
            {
                var newAtt = objectCopy.AutoCopy(att);
                newAtt.RefTableID = newEntity.Id.ToString();
                newAtt.ID = Guid.NewGuid();
                newList.Add(newAtt);
            }
            Attachment.Add(newList.ToArray());
            return url;
        }

        public void GeneratePackage(string projectId)
        {
            if (!Any(p => p.ProjectId == projectId && p.IsHistory == false))
            {
                var closurePackage = new ClosurePackage();
                closurePackage.Id = Guid.NewGuid();
                closurePackage.IsHistory = false;
                closurePackage.ProjectId = projectId;
                Add(closurePackage);
            }
        }

        public string GeneratePackageTask(string projectId)
        {
            if (!TaskWork.Any(i => i.RefID == projectId && i.TypeCode == FlowCode.Closure_ClosurePackage && i.Status == TaskWorkStatus.UnFinish))
            {
                var taskWork = new TaskWork();
                taskWork.SourceCode = FlowCode.Closure;
                taskWork.SourceNameENUS = taskWork.SourceCode;
                taskWork.SourceNameZHCN = "关店流程";
                taskWork.Status = TaskWorkStatus.UnFinish;
                taskWork.StatusNameZHCN = "任务";
                taskWork.StatusNameENUS = "任务";
                taskWork.RefID = projectId;
                taskWork.Id = Guid.NewGuid();
                taskWork.CreateTime = DateTime.Now;
                taskWork.CreateUserAccount = ClientCookie.UserCode;

                var closureInfo = ClosureInfo.GetByProjectId(projectId);

                taskWork.Title = TaskWork.BuildTitle(projectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
                taskWork.TypeCode = WorkflowCode;
                taskWork.TypeNameENUS = "ClosurePackage";
                taskWork.TypeNameZHCN = "ClosurePackage";
                taskWork.ReceiverAccount = closureInfo.AssetActorAccount;
                taskWork.ReceiverNameENUS = closureInfo.AssetActorNameENUS;
                taskWork.ReceiverNameZHCN = closureInfo.AssetActorNameZHCN;
                taskWork.ActivityName = "Start";
                taskWork.Url = string.Format(@"/Closure/Main#/ClosurePackage?projectId={0}", projectId);
                taskWork.StoreCode = closureInfo.USCode;

                TaskWork.Add(taskWork);
                return taskWork.Url;
            }
            return "";
        }

        public bool NeedUploadSign()
        {
            //var signedConfig = System.Configuration.ConfigurationManager.AppSettings["ClosurePackage_SignedPackage_Receiver"];
            //if (!string.IsNullOrEmpty(signedConfig) && signedConfig.Split(':').Length > 0)
            //{
            //    var closureWOCheckList = ClosureWOCheckList.Get(this.ProjectId);
            //    if (closureWOCheckList != null && closureWOCheckList.TotalCost_WriteOFF != null)
            //    {
            //        //Signed Package，是根据Total Write-Off的金额是否大于152.75W，如果是，则在Package完成之后，发送任务给制定用户 Linda Lu，必传。否则则不必传。
            //        if (closureWOCheckList.TotalCost_WriteOFF.Value > decimal.Parse(signedConfig.Split(':')[0]))
            //            return true;
            //    }
            //}
            //return false;
            return true;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            switch (status)
            {
                case TaskWorkStatus.K2ProcessDeclined:
                    ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Closure, ProjectStatus.Rejected);
                    //删除未处理完成的任务
                    var oldTask = TaskWork.Search(i => i.SourceCode == FlowCode.Closure && i.RefID == this.ProjectId && i.Status == TaskWorkStatus.UnFinish).ToArray();
                    foreach (var taskItem in oldTask)
                    {
                        taskItem.Status = TaskWorkStatus.Cancel;
                    }
                    TaskWork.Update(oldTask);
                    ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Closure_ClosurePackage, ProjectStatus.Rejected);
                    break;
                case TaskWorkStatus.K2ProcessApproved:
                    //ClosurePackage的最后一步在K2中，但不属于审批
                    task.ActionName = "";
                    task.Update();

                    ProjectInfo.FinishNode(this.ProjectId, FlowCode.Closure_ClosurePackage, NodeCode.Closure_ClosurePackage_ResourceUpload, ProjectStatus.Finished);
                    //Package流程走完后，在Actor处生成一条任务，供Actor上传Signed Termination Agreement与Signed Package
                    //GenerateSignedPackageTask(ProjectId);

                    if (!TaskWork.Any(i => i.RefID == this.ProjectId && i.SourceCode == FlowCode.Closure && i.TypeCode == FlowCode.Closure_ContractInfo && i.Status != TaskWorkStatus.Cancel))
                    {
                        TaskWork taskWorkContractInfo = new TaskWork();
                        taskWorkContractInfo.SourceCode = FlowCode.Closure;
                        taskWorkContractInfo.SourceNameENUS = FlowCode.Closure;
                        taskWorkContractInfo.SourceNameZHCN = "关店流程";
                        taskWorkContractInfo.Status = TaskWorkStatus.UnFinish;
                        taskWorkContractInfo.StatusNameZHCN = "任务";
                        taskWorkContractInfo.StatusNameENUS = "任务";
                        taskWorkContractInfo.RefID = ProjectId;
                        taskWorkContractInfo.Id = Guid.NewGuid();
                        taskWorkContractInfo.CreateTime = DateTime.Now;

                        //ClosureInfo closure = new ClosureInfo();
                        var closureInfo = ClosureInfo.GetByProjectId(ProjectId);

                        taskWorkContractInfo.Title = TaskWork.BuildTitle(ProjectId, closureInfo.StoreNameZHCN, closureInfo.StoreNameENUS);
                        taskWorkContractInfo.TypeCode = FlowCode.Closure_ContractInfo;
                        taskWorkContractInfo.TypeNameENUS = "ContractInfo";
                        taskWorkContractInfo.TypeNameZHCN = "ContractInfo";
                        taskWorkContractInfo.ReceiverAccount = closureInfo.AssetActorAccount;
                        taskWorkContractInfo.ReceiverNameENUS = closureInfo.AssetActorNameENUS;
                        taskWorkContractInfo.ReceiverNameZHCN = closureInfo.AssetActorNameZHCN;
                        taskWorkContractInfo.Url = SiteInfo.GetProjectHandlerPageUrl(FlowCode.Closure_ContractInfo, closureInfo.ProjectId);
                        taskWorkContractInfo.StoreCode = closureInfo.USCode;
                        taskWorkContractInfo.ActivityName = "Start";
                        taskWorkContractInfo.Add();
                    }

                    //为了方便测试，暂时做成Package走完立刻生成ConsInvtChecking和ClourseMemo
                    //var consInvtChecking = new ClosureConsInvtChecking();
                    //consInvtChecking.GenerateConsInvtCheckingTask(ProjectId);
                    var memo = new ClosureMemo();
                    memo.GenerateClourseMemoTask(ProjectId);

                    //如果是Complete了的流程，Edit后流程走完需要重新把主流程状态改成Complete
                    ProjectInfo.CompleteMainIfEnable(ProjectId);
                    break;
            }
        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var returnDic = new Dictionary<string, string>();

            var closureInfoEntity = ClosureInfo.GetByProjectId(this.ProjectId);
            var storeInfo = StoreBasicInfo.GetStore(closureInfoEntity.USCode);
            StoreBasicInfo store = storeInfo.StoreBasicInfo;

            returnDic.Add("WorkflowName", SystemCode.Instance.GetCodeName(FlowCode.Closure, ClientCookie.Language));
            returnDic.Add("ProjectID", ProjectId);
            returnDic.Add("USCode", string.IsNullOrEmpty(closureInfoEntity.USCode) ? " " : closureInfoEntity.USCode);
            returnDic.Add("Region", string.IsNullOrEmpty(store.RegionENUS) ? " " : store.RegionENUS);
            returnDic.Add("StoreNameEN", string.IsNullOrEmpty(store.NameENUS) ? " " : store.NameENUS);
            returnDic.Add("Market", string.IsNullOrEmpty(store.MarketENUS) ? " " : store.MarketENUS);
            returnDic.Add("StoreNameCN", string.IsNullOrEmpty(store.NameZHCN) ? " " : store.NameZHCN);
            returnDic.Add("City", string.IsNullOrEmpty(store.CityENUS) ? " " : store.CityENUS);
            returnDic.Add("Address", string.IsNullOrEmpty(store.AddressZHCN) ? " " : store.AddressZHCN);
            returnDic.Add("StoreAge", (DateTime.Now.Year - store.OpenDate.Year).ToString());
            returnDic.Add("OpenDate", store.OpenDate.ToString("yyyy-MM-dd"));
            returnDic.Add("CurrentLeaseENDYear", storeInfo.StoreContractInfo.EndYear);
            returnDic.Add("AssetsManager", storeInfo.StoreDevelop.AssetMgrName);
            returnDic.Add("AssetsActor", storeInfo.StoreDevelop.AssetRepName);
            returnDic.Add("AssetsRep", closureInfoEntity.AssetActorNameENUS);
            returnDic.Add("ClosureType", string.IsNullOrEmpty(closureInfoEntity.ClosureTypeNameENUS) ? " " : closureInfoEntity.ClosureTypeNameENUS);
            returnDic.Add("LandlordName", string.IsNullOrEmpty(closureInfoEntity.LandlordName) ? " " : closureInfoEntity.LandlordName);
            if (storeInfo.StoreContractInfo != null)
            {
                returnDic.Add("LeaseExpireDate", storeInfo.StoreContractInfo.EndDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                returnDic.Add("LeaseExpireDate", " ");
            }
            if (closureInfoEntity.ActualCloseDate != null)
            {
                string closureDate = closureInfoEntity.ActualCloseDate.Value.ToString("yyyy-MM-dd");
                returnDic.Add("ClosureDate", string.IsNullOrEmpty(closureDate) ? " " : closureDate);
                returnDic.Add("CloseDate", string.IsNullOrEmpty(closureDate) ? " " : closureDate);
            }
            var packageInfo = ClosurePackage.Get(this.ProjectId);
            returnDic.Add("OriginalCFNPV", packageInfo.OriginalCFNPV.HasValue ? DataConverter.ToMoney(packageInfo.OriginalCFNPV.Value) : " ");

            var closureToolEntity = ClosureTool.Get(this.ProjectId);
            if (closureToolEntity != null)
            {
                returnDic.Add("TotalOneOffCosts", closureToolEntity.TotalOneOffCosts.HasValue ? DataConverter.ToMoney(closureToolEntity.TotalOneOffCosts.Value) : string.Empty);
                returnDic.Add("Compensation", closureToolEntity.Compensation.HasValue ? DataConverter.ToMoney(closureToolEntity.Compensation.Value).ToString() : " ");
            }
            returnDic.Add("NetOperatingIncome",
                packageInfo.NetOperatingIncome.HasValue ? DataConverter.ToMoney(packageInfo.NetOperatingIncome.Value) : " ");
            returnDic.Add("Relocation", closureInfoEntity.RelocationNameENUS);
            returnDic.Add("RelocatedPipelineID",
                packageInfo.RelocationPipelineID.HasValue ? packageInfo.RelocationPipelineID.ToString() : " ");
            returnDic.Add("PipelineName", packageInfo.PipelineName);
            returnDic.Add("NewSiteNetCFNPV", packageInfo.NewSiteNetCFNPV.HasValue ? DataConverter.ToMoney(packageInfo.NewSiteNetCFNPV.Value) : " ");
            returnDic.Add("OtherCFNPV", packageInfo.OtherCFNPV.HasValue ? DataConverter.ToMoney(packageInfo.OtherCFNPV.Value) : string.Empty);
            returnDic.Add("NetGain", packageInfo.NetGain.HasValue ? DataConverter.ToMoney(packageInfo.NetGain.Value) : " ");
            returnDic.Add("ReasonDescription", string.IsNullOrEmpty(packageInfo.ReasonDescriptionForNegativeNetGain) ? " " : packageInfo.ReasonDescriptionForNegativeNetGain);

            return returnDic;
        }


        protected override void ChangeWorkflowApprovers(List<TaskWork> taskWorks, List<ApproveDialogUser> prevApprovers, ApproveUsers currApprover)
        {
            var packageApprovers =
                                prevApprovers.FirstOrDefault(e => e.FlowCode == WorkflowCode
                                                                && e.RefTableID == Id.ToString());
            foreach (var taskWork in taskWorks)
            {
                List<ProcessDataField> dataFields = null;
                switch (taskWork.TypeCode)
                {
                    case FlowCode.Closure_ClosurePackage:
                        var package = ClosurePackage.Get(taskWork.RefID);
                        package.AppUsers = currApprover;
                        dataFields = package.SetWorkflowDataFields(null);
                        if (packageApprovers != null)
                        {
                            SimpleEmployee receiver = null;
                            if (taskWork.ReceiverAccount == packageApprovers.MarketMgrCode
                                && packageApprovers.MarketMgrCode != currApprover.MarketMgr.Code)
                            {
                                receiver = currApprover.MarketMgr;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.RegionalMgrCode
                                && packageApprovers.RegionalMgrCode != currApprover.RegionalMgr.Code)
                            {
                                receiver = currApprover.RegionalMgr;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.MDDCode
                                 && packageApprovers.MDDCode != currApprover.MDD.Code)
                            {
                                receiver = currApprover.MDD;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.GMCode
                                 && packageApprovers.GMCode != currApprover.GM.Code)
                            {
                                receiver = currApprover.GM;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.FCCode
                                 && packageApprovers.FCCode != currApprover.FC.Code)
                            {
                                receiver = currApprover.FC;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.VPGMCode
                                 && packageApprovers.VPGMCode != currApprover.VPGM.Code)
                            {
                                receiver = currApprover.VPGM;

                            }

                            if (taskWork.ReceiverAccount == packageApprovers.CDOCode
                                 && packageApprovers.CDOCode != currApprover.CDO.Code)
                            {
                                receiver = currApprover.CDO;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.MngDirectorCode
                                && packageApprovers.MngDirectorCode != currApprover.ManagingDirector.Code)
                            {
                                receiver = currApprover.ManagingDirector;
                            }
                            if (taskWork.ReceiverAccount == packageApprovers.CFOCode
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
                            packageApprovers.MarketMgrCode = currApprover.MarketMgr.Code;
                            if (currApprover.RegionalMgr != null)
                            {
                                packageApprovers.RegionalMgrCode = currApprover.RegionalMgr.Code;
                            }
                            packageApprovers.GMCode = currApprover.GM.Code;
                            packageApprovers.FCCode = currApprover.FC.Code;
                            packageApprovers.MDDCode = currApprover.MDD.Code;
                            packageApprovers.VPGMCode = currApprover.VPGM.Code;
                            if (currApprover.CDO != null)
                            {
                                packageApprovers.CDOCode = currApprover.CDO.Code;
                            }
                            if (currApprover.CFO != null)
                            {
                                packageApprovers.CFOCode = currApprover.CFO.Code;
                            }
                            packageApprovers.MngDirectorCode = currApprover.ManagingDirector.Code;
                            if (currApprover.NoticeUsers != null)
                            {
                                packageApprovers.NoticeUsers = "";
                                foreach (var noticeUser in currApprover.NoticeUsers)
                                {
                                    if (string.IsNullOrEmpty(packageApprovers.NoticeUsers))
                                        packageApprovers.NoticeUsers = noticeUser.Code;
                                    else
                                        packageApprovers.NoticeUsers += ";" + noticeUser.Code;
                                }
                            }
                            if (currApprover.NecessaryNoticeUsers != null)
                            {
                                packageApprovers.NecessaryNoticeUsers = "";
                                foreach (var noticeUser in currApprover.NecessaryNoticeUsers)
                                {
                                    if (string.IsNullOrEmpty(packageApprovers.NecessaryNoticeUsers))
                                        packageApprovers.NecessaryNoticeUsers = noticeUser.Code;
                                    else
                                        packageApprovers.NecessaryNoticeUsers += ";" + noticeUser.Code;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            string destMgrs = AppUsers.MarketMgr.Code;
            if (AppUsers.RegionalMgr != null)
            {
                destMgrs += ";" + AppUsers.RegionalMgr.Code;
            }

            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MarketMgr", destMgrs),
                new ProcessDataField("dest_GMApprovers",string.Concat(AppUsers.DD.Code, ";", AppUsers.GM.Code, ";", AppUsers.FC.Code,AppUsers.RDD==null?"":";"+AppUsers.RDD.Code)),
                new ProcessDataField("dest_VPGM",AppUsers.VPGM.Code),
                new ProcessDataField("dest_CDO",AppUsers.CDO!= null?AppUsers.CDO.Code:""),
                new ProcessDataField("dest_CFO",AppUsers.CFO!= null?AppUsers.CFO.Code:""),
                new ProcessDataField("dest_MngDirector",AppUsers.ManagingDirector.Code),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        public void UpdateRelocation(bool IsRelocation)
        {
            this.IsRelocation = IsRelocation;
            this.Update();
        }
    }
}
