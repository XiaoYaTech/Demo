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
    public partial class V_AM_DL_Reimage : BaseEntity<V_AM_DL_Reimage>
    {
        public bool Editable { get; set; }

        public static V_AM_DL_Reimage Get(Guid ID)
        {
            var reimage = FirstOrDefault(i => i.Id == ID);
            var projectInfo = ProjectInfo.Search(i => i.USCode == reimage.USCode && i.FlowCode == FlowCode.Reimage).OrderByDescending(i => i.CreateTime).ToList();
            if (reimage != null)
            {
                if (projectInfo.Count > 0 && projectInfo[0].Id == reimage.Id)
                    reimage.Editable = true;
                else
                    reimage.Editable = false;
            }
            return reimage;
        }

        public void Save(bool pushOrNot)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var projectInfo = ProjectInfo.Get(Id);
                if (projectInfo == null)
                {
                    ProjectId = ProjectInfo.CreateDLProject(Id, FlowCode.Reimage, USCode, NodeCode.Start, ClientCookie.UserCode, pushOrNot);
                    var store = StoreBasicInfo.GetStorInfo(USCode);

                    var reimageInfo = new ReimageInfo();
                    reimageInfo.Id = Guid.NewGuid();
                    reimageInfo.ProjectId = ProjectId;
                    reimageInfo.USCode = USCode;
                    reimageInfo.CreateDate = DateTime.Now;
                    reimageInfo.CreateUserAccount = ClientCookie.UserCode;
                    reimageInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                    reimageInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    reimageInfo.AssetRepAccount = "";
                    reimageInfo.AssetRepNameZHCN = "";
                    reimageInfo.AssetRepNameENUS = "";
                    reimageInfo.AssetActorAccount = "";
                    reimageInfo.AssetActorNameZHCN = "";
                    reimageInfo.AssetActorNameENUS = "";
                    reimageInfo.FinanceAccount = "";
                    reimageInfo.FinanceNameZHCN = "";
                    reimageInfo.FinanceNameENUS = "";
                    reimageInfo.PMAccount = "";
                    reimageInfo.PMNameZHCN = "";
                    reimageInfo.PMNameENUS = "";
                    reimageInfo.LegalAccount = "";
                    reimageInfo.LegalNameZHCN = "";
                    reimageInfo.LegalNameENUS = "";
                    reimageInfo.StoreNameENUS = store.NameENUS;
                    reimageInfo.StoreNameZHCN = store.NameZHCN;
                    reimageInfo.GBDate = GBDate;
                    reimageInfo.ReopenDate = ReopenDate;
                    reimageInfo.Add();

                    var reimageConsInfo = new ReimageConsInfo();
                    reimageConsInfo.Id = Guid.NewGuid();
                    reimageConsInfo.ProjectId = ProjectId;
                    reimageConsInfo.IsHistory = false;
                    reimageConsInfo.CreateTime = DateTime.Now;
                    reimageConsInfo.CreateUserAccount = ClientCookie.UserCode;
                    reimageConsInfo.Add();

                    var reinvestmentBasicInfo = new ReinvestmentBasicInfo();
                    reinvestmentBasicInfo.ConsInfoID = reimageConsInfo.Id;
                    reinvestmentBasicInfo.RightSizingSeatNo = RightSizingSeatNO;
                    reinvestmentBasicInfo.NewDesignType = AfterReimageDesignType;
                    reinvestmentBasicInfo.Add();

                    var reimageConsInvtChecking = new ReimageConsInvtChecking();
                    reimageConsInvtChecking.Id = Guid.NewGuid();
                    reimageConsInvtChecking.ProjectId = ProjectId;
                    reimageConsInvtChecking.IsHistory = false;
                    reimageConsInvtChecking.CreateTime = DateTime.Now;
                    reimageConsInvtChecking.CreateUserAccount = ClientCookie.UserCode;
                    reimageConsInvtChecking.Add();

                    var writeOffAmount = new WriteOffAmount();
                    writeOffAmount.Id = Guid.NewGuid();
                    writeOffAmount.ConsInfoID = reimageConsInvtChecking.Id;
                    writeOffAmount.TotalWriteOff = Reimage_Total_WO_Proj;
                    writeOffAmount.TotalActual = Reimage_Total_WO_Act;
                    writeOffAmount.Add();

                    var reinvestmentCost = new ReinvestmentCost();
                    reinvestmentCost.Id = Guid.NewGuid();
                    reinvestmentCost.ConsInfoID = reimageConsInvtChecking.Id;
                    reinvestmentCost.TotalReinvestmentBudget = Reimage_Total_Reinvestment_Proj;
                    reinvestmentCost.TotalReinvestmentPMAct = Reimage_Total_Reinvestment_Act;
                    reinvestmentCost.Add();

                    var reopenMemo = new ReopenMemo();
                    reopenMemo.Id = Guid.NewGuid();
                    reopenMemo.ProjectId = ProjectId;
                    reopenMemo.NewMcCafe = false;
                    reopenMemo.NewKiosk = false;
                    reopenMemo.NewMDS = false;
                    reopenMemo.Is24H = false;
                    reopenMemo.AftARSN = AfterReimageSeatNO;
                    reopenMemo.Add();
                }
                else
                {
                    ProjectId = projectInfo.ProjectId;
                    projectInfo.IsPushed = pushOrNot;
                    projectInfo.Update();
                    var store = StoreBasicInfo.GetStorInfo(USCode);

                    var reimageInfo = ReimageInfo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (reimageInfo != null)
                    {
                        reimageInfo.GBDate = GBDate;
                        reimageInfo.ReopenDate = ReopenDate;
                        reimageInfo.Update();
                    }
                    else
                    {
                        reimageInfo = new ReimageInfo();
                        reimageInfo.Id = Guid.NewGuid();
                        reimageInfo.ProjectId = ProjectId;
                        reimageInfo.USCode = USCode;
                        reimageInfo.CreateDate = DateTime.Now;
                        reimageInfo.CreateUserAccount = ClientCookie.UserCode;
                        reimageInfo.CreateUserNameENUS = ClientCookie.UserNameENUS;
                        reimageInfo.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                        reimageInfo.AssetRepAccount = "";
                        reimageInfo.AssetRepNameZHCN = "";
                        reimageInfo.AssetRepNameENUS = "";
                        reimageInfo.AssetActorAccount = "";
                        reimageInfo.AssetActorNameZHCN = "";
                        reimageInfo.AssetActorNameENUS = "";
                        reimageInfo.FinanceAccount = "";
                        reimageInfo.FinanceNameZHCN = "";
                        reimageInfo.FinanceNameENUS = "";
                        reimageInfo.PMAccount = "";
                        reimageInfo.PMNameZHCN = "";
                        reimageInfo.PMNameENUS = "";
                        reimageInfo.LegalAccount = "";
                        reimageInfo.LegalNameZHCN = "";
                        reimageInfo.LegalNameENUS = "";
                        reimageInfo.StoreNameENUS = store.NameENUS;
                        reimageInfo.StoreNameZHCN = store.NameZHCN;
                        reimageInfo.GBDate = GBDate;
                        reimageInfo.ReopenDate = ReopenDate;
                        reimageInfo.Add();
                    }

                    var reimageConsInfo = ReimageConsInfo.FirstOrDefault(i => i.ProjectId == ProjectId && i.IsHistory == false);
                    if (reimageConsInfo != null)
                    {
                        var reinvestmentBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(i => i.ConsInfoID == reimageConsInfo.Id);
                        if (reinvestmentBasicInfo != null)
                        {
                            reinvestmentBasicInfo.RightSizingSeatNo = RightSizingSeatNO;
                            reinvestmentBasicInfo.NewDesignType = AfterReimageDesignType;
                            reinvestmentBasicInfo.Update();
                        }
                        else
                        {
                            reinvestmentBasicInfo = new ReinvestmentBasicInfo();
                            reinvestmentBasicInfo.ConsInfoID = reimageConsInfo.Id;
                            reinvestmentBasicInfo.RightSizingSeatNo = RightSizingSeatNO;
                            reinvestmentBasicInfo.NewDesignType = AfterReimageDesignType;
                            reinvestmentBasicInfo.Add();
                        }
                    }
                    else
                    {
                        reimageConsInfo = new ReimageConsInfo();
                        reimageConsInfo.Id = Guid.NewGuid();
                        reimageConsInfo.ProjectId = ProjectId;
                        reimageConsInfo.IsHistory = false;
                        reimageConsInfo.CreateTime = DateTime.Now;
                        reimageConsInfo.CreateUserAccount = ClientCookie.UserCode;
                        reimageConsInfo.Add();

                        var reinvestmentBasicInfo = new ReinvestmentBasicInfo();
                        reinvestmentBasicInfo.ConsInfoID = reimageConsInfo.Id;
                        reinvestmentBasicInfo.RightSizingSeatNo = RightSizingSeatNO;
                        reinvestmentBasicInfo.NewDesignType = AfterReimageDesignType;
                        reinvestmentBasicInfo.Add();
                    }

                    var reimageConsInvtChecking = ReimageConsInvtChecking.FirstOrDefault(i => i.ProjectId == ProjectId && i.IsHistory == false);
                    if (reimageConsInvtChecking != null)
                    {
                        var writeOffAmount = WriteOffAmount.FirstOrDefault(i => i.ConsInfoID == reimageConsInvtChecking.Id);
                        if (writeOffAmount != null)
                        {
                            writeOffAmount.TotalWriteOff = Reimage_Total_WO_Proj;
                            writeOffAmount.TotalActual = Reimage_Total_WO_Act;
                            writeOffAmount.Update();
                        }
                        else
                        {
                            writeOffAmount = new WriteOffAmount();
                            writeOffAmount.Id = Guid.NewGuid();
                            writeOffAmount.ConsInfoID = reimageConsInvtChecking.Id;
                            writeOffAmount.TotalWriteOff = Reimage_Total_WO_Proj;
                            writeOffAmount.TotalActual = Reimage_Total_WO_Act;
                            writeOffAmount.Add();
                        }

                        var reinvestmentCost = ReinvestmentCost.FirstOrDefault(i => i.ConsInfoID == reimageConsInvtChecking.Id);
                        if (reinvestmentCost != null)
                        {
                            reinvestmentCost.TotalReinvestmentBudget = Reimage_Total_Reinvestment_Proj;
                            reinvestmentCost.TotalReinvestmentPMAct = Reimage_Total_Reinvestment_Act;
                            reinvestmentCost.Update();
                        }
                        else
                        {
                            reinvestmentCost = new ReinvestmentCost();
                            reinvestmentCost.Id = Guid.NewGuid();
                            reinvestmentCost.ConsInfoID = reimageConsInvtChecking.Id;
                            reinvestmentCost.TotalReinvestmentBudget = Reimage_Total_Reinvestment_Proj;
                            reinvestmentCost.TotalReinvestmentPMAct = Reimage_Total_Reinvestment_Act;
                            reinvestmentCost.Add();
                        }
                    }
                    else
                    {
                        reimageConsInvtChecking = new ReimageConsInvtChecking();
                        reimageConsInvtChecking.Id = Guid.NewGuid();
                        reimageConsInvtChecking.ProjectId = ProjectId;
                        reimageConsInvtChecking.IsHistory = false;
                        reimageConsInvtChecking.CreateTime = DateTime.Now;
                        reimageConsInvtChecking.CreateUserAccount = ClientCookie.UserCode;
                        reimageConsInvtChecking.Add();

                        var writeOffAmount = new WriteOffAmount();
                        writeOffAmount.Id = Guid.NewGuid();
                        writeOffAmount.ConsInfoID = reimageConsInvtChecking.Id;
                        writeOffAmount.TotalWriteOff = Reimage_Total_WO_Proj;
                        writeOffAmount.TotalActual = Reimage_Total_WO_Act;
                        writeOffAmount.Add();

                        var reinvestmentCost = new ReinvestmentCost();
                        reinvestmentCost.Id = Guid.NewGuid();
                        reinvestmentCost.ConsInfoID = reimageConsInvtChecking.Id;
                        reinvestmentCost.TotalReinvestmentBudget = Reimage_Total_Reinvestment_Proj;
                        reinvestmentCost.TotalReinvestmentPMAct = Reimage_Total_Reinvestment_Act;
                        reinvestmentCost.Add();
                    }

                    var reopenMemo = ReopenMemo.FirstOrDefault(i => i.ProjectId == ProjectId);
                    if (reopenMemo != null)
                    {
                        reopenMemo.AftARSN = AfterReimageSeatNO;
                        reopenMemo.Update();
                    }
                    else
                    {
                        reopenMemo = new ReopenMemo();
                        reopenMemo.Id = Guid.NewGuid();
                        reopenMemo.ProjectId = ProjectId;
                        reopenMemo.NewMcCafe = false;
                        reopenMemo.NewKiosk = false;
                        reopenMemo.NewMDS = false;
                        reopenMemo.Is24H = false;
                        reopenMemo.AftARSN = AfterReimageSeatNO;
                        reopenMemo.Add();
                    }
                }
                tranScope.Complete();
            }
        }
    }
}
