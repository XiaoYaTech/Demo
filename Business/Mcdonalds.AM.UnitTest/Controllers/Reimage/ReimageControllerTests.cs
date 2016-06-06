using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.Services.Controllers.Reimage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.Services.Controllers.Reimage.Tests
{
    [TestClass()]
    public class ReimageControllerTests
    {
        [TestMethod()]
        public void CreateProjectTest()
        {
            try
            {
                var reimageInfo =
                    JsonConvert.DeserializeObject<ReimageInfo>(
                        "{'NecessaryNoticeUserList':null,'NoticeUserList':[{'TaskStatus':null,'Id':'00000000-0000-0000-0000-000000000000','ProjectId':null,'UserAccount':'E5001122','RoleCode':null,'RoleNameZHCN':null,'RoleNameENUS':null,'CreateDate':null,'CreateUserAccount':null,'CreateUserName':null,'Sequence':null,'UserNameZHCN':'HuangAaron','UserNameENUS':'Aaron Huang'},{'TaskStatus':null,'Id':'00000000-0000-0000-0000-000000000000','ProjectId':null,'UserAccount':'E5001029','RoleCode':null,'RoleNameZHCN':null,'RoleNameENUS':null,'CreateDate':null,'CreateUserAccount':null,'CreateUserName':null,'Sequence':null,'UserNameZHCN':'刘俊霞','UserNameENUS':'Ada Liu'}],'ConsInfo':null,'IsSiteInfoSaveable':false,'SiteInfoId':'00000000-0000-0000-0000-000000000000','Year':0,'EstimatedVsActualConstruction':null,'Id':'4c906c1d-92d7-4e8d-a223-9529809a6425','ProjectId':'Reimage15040702','USCode':'1410128','GBDate':'2015-04-16T00:00:00+08:00','ReopenDate':'2015-04-29T00:00:00+08:00','ConstCompletionDate':null,'CreateDate':'2015-04-07T15:55:30.5408108+08:00','CreateUserAccount':'E5001604','CreateUserNameZHCN':'徐文茂','CreateUserNameENUS':'Jerry Xu','AssetRepAccount':null,'AssetRepNameZHCN':'','AssetRepNameENUS':'Iran Cheong','AssetActorAccount':'E5001604','AssetActorNameZHCN':'徐文茂','AssetActorNameENUS':'Jerry Xu','FinanceAccount':'E5011115','FinanceNameZHCN':'RachelLee','FinanceNameENUS':'Rachel Lee','PMAccount':'E5019616','PMNameZHCN':'郑建森','PMNameENUS':'Eric zheng','LegalAccount':'','LegalNameZHCN':'','LegalNameENUS':'','StoreNameENUS':'Xikezhan Store','StoreNameZHCN':'西客站餐厅','AssetManagerAccount':null,'AssetManagerNameZHCN':'','AssetManagerNameENUS':'Iran Cheong','CMAccount':'E5015169','CMNameZHCN':'聂宗华','CMNameENUS':'Ethan Nie','EntityId':'00000000-0000-0000-0000-000000000000','IsProjectFreezed':false,'IsMainProject':false,'WFProjectId':null,'WFCode':null,'WorkflowProcessName':null,'WorkflowProcessCode':null,'TableName':'MajorLeaseLegalReview','WorkflowActOriginator':null,'WorkflowCode':null,'WorkflowNormalActors':[]}");
                reimageInfo.Add();
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
