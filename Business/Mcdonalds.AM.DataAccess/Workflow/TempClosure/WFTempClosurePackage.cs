/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/9/2014 2:25:06 PM
 * FileName     :   WFTempClosurePackage
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Workflow.TempClosure
{
    public class WFTempClosurePackage:BaseWorkflow<TempClosurePackage>
    {
        public WFTempClosurePackage(TempClosurePackage entity, ApproveUsers approvers)
            : base(entity)
        {
            var task = TaskWork.FirstOrDefault(t => t.RefID == entity.ProjectId && t.TypeCode == FlowCode.TempClosure_ClosurePackage);
            string destMRMgrs = approvers.MarketMgr.Code;
            if(approvers.RegionalMgr != null){
                destMRMgrs +=";"+approvers.RegionalMgr.Code;
            }
            K2Param.AddDataField("dest_Creator", entity.CreateUserAccount);
            K2Param.AddDataField("dest_MRMgrs", destMRMgrs);
            K2Param.AddDataField("dest_GMApprovers", string.Concat(approvers.MDD.Code, ";", approvers.GM.Code, ";", approvers.FC.Code));
            K2Param.AddDataField("dest_VPGM", approvers.VPGM.Code);
            //K2Param.AddDataField("dest_DevVP", Entity.DevVP); Cary: 没有这个环节了。2014-9-11
            K2Param.AddDataField("ProcessCode", ProcessCode);
            K2Param.AddDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task));
        }

        public WFTempClosurePackage(TempClosurePackage entity)
            : base(entity)
        {
            var approvers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            var task = TaskWork.FirstOrDefault(t => t.RefID == entity.ProjectId && t.TypeCode == FlowCode.TempClosure_ClosurePackage);
            K2Param.AddDataField("dest_Creator", Entity.CreateUserAccount);
            K2Param.AddDataField("dest_MRMgrs", string.Concat(approvers.MarketMgrCode, ";", !string.IsNullOrEmpty(approvers.RegionalMgrCode) ? approvers.RegionalMgrCode : ""));
            K2Param.AddDataField("dest_GMApprovers", string.Concat(approvers.MDDCode, ";", approvers.GMCode, ";", approvers.FCCode));
            K2Param.AddDataField("dest_VPGM", approvers.VPGMCode);
            //K2Param.AddDataField("dest_DevVP", Entity.DevVP); Cary: 没有这个环节了。2014-9-11
            K2Param.AddDataField("ProcessCode", ProcessCode);
            K2Param.AddDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task));
        }

        public override string ProcessName
        {
            get { return @"MCDAMK2Project\TempClosurePackage"; }
        }

        public override string ProcessCode
        {
            get { return @"MCD_AM_TempClosure_Package"; }
        }

        public override string TableName
        {
            get { return "TempClosurePackage"; }
        }

        public override string Act_Originator
        {
            get { return "Originator"; }
        }

        public override string ProjectFlowCode
        {
            get { return FlowCode.TempClosure_ClosurePackage; }
        }

        public override string[] NormalActors
        {
            get
            {
                return new string[]{
                    Act_Originator,
                    "M&R Manager",
                    "DD_GM_FC_RDD", //要支持在途老流程
                    "DD_GM_FC"
                };
            }
        }
    }
}
