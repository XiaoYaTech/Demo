using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class V_AM_DL_Closure : BaseEntity<V_AM_DL_Closure>
    {
        public bool Editable { get; set; }

        public static V_AM_DL_Closure Get(Guid ID)
        {
            var closure = FirstOrDefault(i => i.Id == ID);
            var projectInfo = ProjectInfo.Search(i => i.USCode == closure.USCode && i.FlowCode == FlowCode.Closure).OrderByDescending(i => i.CreateTime).ToList();
            if (closure != null)
            {
                if (projectInfo.Count > 0 && projectInfo[0].Id == closure.Id)
                    closure.Editable = true;
                else
                    closure.Editable = false;
            }
            return closure;
        }

        public void Save(bool pushOrNot)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.Get(Id);
                if (projectInfo == null)
                {
                    ProjectId = ProjectInfo.CreateDLProject(Id, FlowCode.Closure, USCode, NodeCode.Start, ClientCookie.UserCode, pushOrNot);

                    var closureInfo = new ClosureInfo();
                    closureInfo.Id = Guid.NewGuid();
                    closureInfo.ProjectId = ProjectId;
                    closureInfo.USCode = USCode;
                    closureInfo.ActualCloseDate = CloseDate;
                    closureInfo.ClosureTypeCode = ClosureTypeCode;
                    closureInfo.ClosureReasonCode = ClosureReasonCode;
                    closureInfo.ClosureReasonRemark = ClosureReasonRemark;
                    closureInfo.RelocationCode = RelocationCode;
                    closureInfo.CreateDate = DateTime.Now;
                    closureInfo.CreateUserAccount = ClientCookie.UserCode;
                    closureInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                    closureInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    closureInfo.Add();

                    var closurePackage = new ClosurePackage();
                    closurePackage.Id = Guid.NewGuid();
                    closurePackage.ProjectId = ProjectId;
                    closurePackage.RelocationPipelineID = RelocatedPipelineID;
                    closurePackage.IsHistory = false;
                    closurePackage.CreateUserAccount = ClientCookie.UserCode;
                    closurePackage.CreateTime = DateTime.Now;
                    closurePackage.Add();

                    var closureConsInvtChecking = new ClosureConsInvtChecking();
                    closureConsInvtChecking.Id = Guid.NewGuid();
                    closureConsInvtChecking.ProjectId = ProjectId;
                    closureConsInvtChecking.IsHistory = false;
                    closureConsInvtChecking.CreateTime = DateTime.Now;
                    closureConsInvtChecking.TotalActual = Closure_WO_Total_Act;
                    closureConsInvtChecking.ClosingCostActual = Closure_ClosingCost_Act;
                    closureConsInvtChecking.Add();

                    var closureMemo = new ClosureMemo();
                    closureMemo.Id = Guid.NewGuid();
                    closureMemo.ProjectId = ProjectId;
                    closureMemo.CreateTime = DateTime.Now;
                    closureMemo.ClosureNature = ClosureNatureType.Permanent;
                    closureMemo.Status = true;
                    closureMemo.Compensation = Closure_Compensation_Act;
                    closureMemo.Add();
                }
                else
                {
                    ProjectId = projectInfo.ProjectId;
                    projectInfo.IsPushed = pushOrNot;
                    projectInfo.Update();

                    var closureInfo = ClosureInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (closureInfo != null)
                    {
                        closureInfo.ActualCloseDate = CloseDate;
                        closureInfo.ClosureTypeCode = ClosureTypeCode;
                        closureInfo.ClosureReasonCode = ClosureReasonCode;
                        closureInfo.ClosureReasonRemark = ClosureReasonRemark;
                        closureInfo.RelocationCode = RelocationCode;
                        closureInfo.Update();
                    }
                    else
                    {
                        closureInfo = new ClosureInfo();
                        closureInfo.Id = Guid.NewGuid();
                        closureInfo.ProjectId = ProjectId;
                        closureInfo.USCode = USCode;
                        closureInfo.ActualCloseDate = CloseDate;
                        closureInfo.ClosureTypeCode = ClosureTypeCode;
                        closureInfo.ClosureReasonCode = ClosureReasonCode;
                        closureInfo.ClosureReasonRemark = ClosureReasonRemark;
                        closureInfo.RelocationCode = RelocationCode;
                        closureInfo.CreateDate = DateTime.Now;
                        closureInfo.CreateUserAccount = ClientCookie.UserCode;
                        closureInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                        closureInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                        closureInfo.Add();
                    }

                    var closurePackage = ClosurePackage.Get(ProjectId);
                    if (closurePackage != null)
                    {
                        closurePackage.RelocationPipelineID = RelocatedPipelineID;
                        closurePackage.Update();
                    }
                    else
                    {
                        closurePackage = new ClosurePackage();
                        closurePackage.Id = Guid.NewGuid();
                        closurePackage.ProjectId = ProjectId;
                        closurePackage.RelocationPipelineID = RelocatedPipelineID;
                        closurePackage.IsHistory = false;
                        closurePackage.CreateUserAccount = ClientCookie.UserCode;
                        closurePackage.CreateTime = DateTime.Now;
                        closurePackage.Add();
                    }

                    var closureConsInvtChecking = ClosureConsInvtChecking.Get(ProjectId);
                    if (closureConsInvtChecking != null)
                    {
                        closureConsInvtChecking.TotalActual = Closure_WO_Total_Act;
                        closureConsInvtChecking.ClosingCostActual = Closure_ClosingCost_Act;
                        closureConsInvtChecking.Update();
                    }
                    else
                    {
                        closureConsInvtChecking = new ClosureConsInvtChecking();
                        closureConsInvtChecking.Id = Guid.NewGuid();
                        closureConsInvtChecking.ProjectId = ProjectId;
                        closureConsInvtChecking.IsHistory = false;
                        closureConsInvtChecking.CreateTime = DateTime.Now;
                        closureConsInvtChecking.TotalActual = Closure_WO_Total_Act;
                        closureConsInvtChecking.ClosingCostActual = Closure_ClosingCost_Act;
                        closureConsInvtChecking.Add();
                    }

                    var closureMemo = ClosureMemo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (closureMemo != null)
                    {
                        closureMemo.Compensation = Closure_Compensation_Act;
                        closureMemo.Update();
                    }
                    else
                    {
                        closureMemo = new ClosureMemo();
                        closureMemo.Id = Guid.NewGuid();
                        closureMemo.ProjectId = ProjectId;
                        closureMemo.CreateTime = DateTime.Now;
                        closureMemo.ClosureNature = ClosureNatureType.Permanent;
                        closureMemo.Status = true;
                        closureMemo.Compensation = Closure_Compensation_Act;
                        closureMemo.Add();
                    }
                }
                tranScope.Complete();
            }
        }
    }
}
