using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using System.IO;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.Services.Workflows.Closure;
using Mcdonalds.AM.Services.Workflows.Enums;
using Mcdonalds.AM.Services.Workflows;
using Newtonsoft.Json;
using Mcdonalds.AM.Services.Infrastructure;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using NTTMNC.BPM.Fx.Core;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ClosureToolController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>

        [Route("api/ClosureTool/GetByProcInstID/{procInstID}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetByProcInstID(int procInstID)
        {
            var entity = ClosureTool.GetByProcInstID(procInstID);
            return Ok(entity);

        }

        [Route("api/ClosureTool/GetById/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetById(Guid id)
        {
            var entity = ClosureTool.GetById(id);
            entity.ImpactOtherStores = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToList();
            return Ok(entity);
        }

        [Route("api/ClosureTool/GetClosureToolByProjectId/{projectId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetClosureToolByProjectId(string projectId)
        {
            var entity = ClosureTool.Get(projectId);
            if (entity != null)
            {
                var condition = new ProjectCommentCondition();
                condition.RefTableId = entity.Id;
                condition.RefTableName = ClosureTool.TableName;
                condition.UserAccount = ClientCookie.UserCode;
                condition.Status = ProjectCommentStatus.Save;

                var commentList = ProjectComment.SearchList(condition);
                entity.ImpactOtherStores = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToList();
                if (commentList != null && commentList.Count > 0)
                {
                    entity.Comments = commentList[0].Content;
                }

            }
            return Ok(entity);
        }




        [Route("api/ClosureTool/GetImpactStores/{count}/{projectId}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetImpactStores(int count, string projectId, string code = "", string name = "")
        {
            var bll = new ClosureTool();
            //            string sql = string.Format(@"
            //                SELECT ISNULL(cis.StoreCode,s.Code) StoreCode,ISNULL(cis.StoreName,s.NameZHCN) StoreName,cis.ImpactSaltes,                   
            //                CAST(CASE WHEN cis.Id IS NULL THEN 0 ELSE 1 END AS BIT) IsSelected           
            //                FROM dbo.Store s INNER JOIN Store sOwner  
            //                ON sOwner.CityName = s.CityName INNER JOIN (
            //	                SELECT DISTINCT ProjectId,USCode 
            //	                FROM dbo.ProjectInfo 
            //	                WHERE ProjectId='{0}') pInfo
            //                ON pInfo.USCode = sOwner.Code INNER JOIN  ClosureTool ct 
            //                ON ct.ProjectId = pInfo.ProjectId LEFT JOIN ClosureToolImpactOtherStore cis                
            //                ON cis.StoreCode = s.Code AND  ct.Id = cis.ClosureId ", projectId);
            //            var list = bll.SqlQuery<SimpleClosureImpactStore>(sql, null);



            var entity = ClosureInfo.GetByProjectId(projectId);
            List<StoreBasicInfo> storeList = null;
            if (entity != null)
            {
                var store = StoreBasicInfo.FirstOrDefault(e => e.StoreCode == entity.USCode);
                storeList = StoreBasicInfo.Search(e => (e.CityCode == store.CityCode) &&
                    (e.StoreCode.Contains(code) || e.NameENUS.Contains(name) || e.NameZHCN.Contains(name)))
                    .OrderBy(e => e.StoreCode).Skip(0).Take(count).ToList();


            }
            return Ok(storeList);
        }


        public List<SimpleClosureImpactStore> ConvertToSimpleClosureImpactStore(List<ClosureToolImpactOtherStore> list)
        {
            return list.Select(item => new SimpleClosureImpactStore
            {
                StoreCode = item.StoreCode,
                NameENUS = item.NameENUS,
                NameZHCN = item.NameZHCN,
                ImpactSaltes = item.ImpactSaltes,
                IsSelected = true
            }).ToList();
        }

        [Route("api/ClosureTool/PostActorClosureTool")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult PostActorClosureTool(PostClosureTool postCT)
        {
            //// 获取Taskwork 数据
            //string errMsg = String.Empty;
            ////判断是否生成过ClosureTool
            //var isGenExecutiveSummaryTask = taskWorkBll.IsExistsTask(entity.ProjectId,
            //    FlowCode.Closure_ExecutiveSummary);
            //if (!isGenExecutiveSummaryTask)
            //{
            //    errMsg = "请先生成Closure Tool!";
            //    return Ok(errMsg);
            //}
            var entity = postCT.Entity;

            string actionLower = entity.Action.ToLower();
            string account = ClientCookie.UserCode;
            //评论信息
            string comments = entity.Comments;
            ClosureTool.UpdateEntity(entity);

            var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToArray();
            ClosureToolImpactOtherStore.Delete(impactList);

            if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
            {
                foreach (var item in postCT.ImpactStores)
                {
                    item.Id = Guid.NewGuid();
                    item.CreateTime = DateTime.Now;
                    item.ClosureId = entity.Id;
                    item.CreateUserAccount = ClientCookie.UserCode;
                }
                ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
            }
            // To-Do K2 action
            ProcessActionResult _action = BPMHelper.ConvertToProcAction(actionLower);

            K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments);

            var isGenExecutiveSummaryTask = TaskWork.IsExistsTask(entity.ProjectId,
                 FlowCode.Closure_ExecutiveSummary);

            var closurePackage = new ClosurePackage();

            if (!isGenExecutiveSummaryTask)
            {
                var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);
                var executiveSummary = new ClosureExecutiveSummary();
                executiveSummary.GenerateExecutiveSummaryTask(entity.ProjectId);

                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_RepInput);

                var noticerList = new List<string>();
                if (closureInfo.NoticeUserList != null && closureInfo.NoticeUserList.Count > 0)
                {
                    noticerList = closureInfo.NoticeUserList.Select(e => e.Account).ToList();
                }


            }
            //如果是从ClosurePackage Edit之后撤回Task的，添加ClosurePackage的Task
            else if (TaskWork.Count(i => i.RefID == entity.ProjectId && i.TypeCode == FlowCode.Closure_ClosurePackage && i.Status == TaskWorkStatus.Cancel) > 0)
            {
                if (ProjectInfo.Any(e => e.ProjectId == entity.ProjectId && e.Status == ProjectStatus.Finished && e.FlowCode == FlowCode.Closure_LegalReview))
                {
                    var package = new ClosurePackage();
                    package.GeneratePackageTask(entity.ProjectId);
                }

                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Finish);

                var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);
                var noticerList = new List<string>();
                if (closureInfo.NoticeUserList != null && closureInfo.NoticeUserList.Count > 0)
                {
                    noticerList = closureInfo.NoticeUserList.Select(e => e.Account).ToList();
                }
            }

            return Ok(true);

        }

        [Route("api/ClosureTool/PostClosureTool")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult PostClosureTool(PostClosureTool postCT)
        {
            int result = 0;
            // Log
            string _debugInfo = string.Format("[Ln 103] Start Run PostClosureTool - Entity: {0}", JsonConvert.SerializeObject(postCT));
            Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureTool");

            int _procInstID = 0;
            var entity = postCT.Entity;
            // 获取Taskwork 数据

            //第一次打开 TTM Financial Data 时，数据存到LDW_FinanceData表
            LDW_FinanceData.OriginalDataOperation(entity.Id, entity.ProjectId, postCT.yearMonth, Utils.GetLatestYear(), Utils.GetLatestMonth());
            var task = TaskWork.FirstOrDefault(
               e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == TaskWorkStatus.UnFinish && e.SourceCode == FlowCode.Closure
                    && e.TypeCode == FlowCode.Closure_ClosureTool && e.RefID == entity.ProjectId
               );

            var taskJson = TaskWork.ConvertToJson(task);

            // Log
            _debugInfo = string.Format("[Ln 126] Task:{0}", taskJson);
            Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureTool");

            // Start K2 Process
            string _procCode = WFClosureTool.ProcessCode;
            List<ProcessDataField> _listDataFields = new List<ProcessDataField>();

            _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));

            //获取财务上级账号（勿删！目前账号不全，先用文本框输入账号替代）
            //UserPositionHandler handler = new UserPositionHandler();
            //var finReportToAccount = handler.GetReportToAccounts(ClientCookie.UserCode);//TODO::Cary

            _listDataFields.Add(new ProcessDataField("dest_FinSupervisor", entity.FinReportToAccount));//entity.   获取财务汇报上级
            _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));
            var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);
            _listDataFields.Add(new ProcessDataField("dest_Actor", closureInfo.AssetActorAccount));
            //_listDataFields.Add(new ProcessDataField("dest_Receiver", "")); 知会人

            // Log
            _debugInfo = string.Format("[Ln 144] DataFields: {0}", taskJson);
            Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureTool");

            if (_listDataFields.Exists(o => string.IsNullOrEmpty(o.DataFieldValue)))
            {
                result = 0;
            }
            else
            {
                //将TaskWork生成任务传给K2
                _listDataFields.Add(new ProcessDataField("ProjectTaskInfo", taskJson));
                _procInstID = K2FxContext.Current.StartProcess(_procCode, ClientCookie.UserCode, _listDataFields);


                // Log
                _debugInfo = string.Format("[Ln 159] ProcInstID: {0}", _procInstID);
                Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureTool");
            }
            ////// completed K2 block

            using (TransactionScope tranScope = new TransactionScope())
            {
                if (task != null)
                {
                    task.Finish();
                }
                if (_procInstID > 0)
                {
                    // 更新业务表

                    entity.ProcInstID = _procInstID;



                    if (entity.Id == new Guid())
                    {

                        entity.Id = Guid.NewGuid();
                        entity.CreateTime = DateTime.Now;
                        entity.CreateUserAccount = ClientCookie.UserCode;
                        entity.IsHistory = false;
                        ClosureTool.Add(entity);
                    }
                    else
                    {
                        entity.IsHistory = false;
                        ClosureTool.UpdateEntity(entity);
                        var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToArray();
                        ClosureToolImpactOtherStore.Delete(impactList);
                    }

                    if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
                    {
                        foreach (var item in postCT.ImpactStores)
                        {
                            item.Id = Guid.NewGuid();
                            item.CreateTime = DateTime.Now;
                            item.ClosureId = entity.Id;
                            item.CreateUserAccount = ClientCookie.UserCode;
                        }
                        ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
                    }




                    SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Submit);


                    ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_FinInput);
                    // Log
                    _debugInfo = string.Format("[Ln 212] Save Result:{0}", result);
                    Log4netHelper.WriteInfoLog(_debugInfo, this.GetType(), "PostClosureTool");


                    // 更新业务表 ProcInstID
                    //entity.ProcInstID = _procInstID;
                    //_db.ClosureTool.Attach(entity);
                    //_db.Entry(entity).State = EntityState.Modified;
                    //result = _db.SaveChanges();
                }
                tranScope.Complete();
            }
            return Ok(result);

        }


        [Route("api/ClosureTool/EnableGenClosureTool/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult EnableGenClosureTool(Guid id)
        {


            var enable = true;
            string err = string.Empty;


            var closureTool = ClosureTool.Get(id);
            var closureCurrentNode = NodeInfo.GetCurrentNode(closureTool.ProjectId, FlowCode.Closure_ClosureTool);
            var repInputNode = NodeInfo.GetNodeInfo(NodeCode.Closure_ClosureTool_FinApprove);

            if (closureCurrentNode.Sequence < repInputNode.Sequence)
            {
                err = "Closure Tool 未审批！</br>";
            }

            //  var wocheckCurrentNode = nodeInfoBLL.GetCurrentNode(closureTool.ProjectId, FlowCode.Closure_WOCheckList);
            var projectInfo = ProjectInfo.Get(closureTool.ProjectId, FlowCode.Closure_WOCheckList);
            if (projectInfo != null && projectInfo.Status != ProjectStatus.Finished)
            {
                err += "WOCheckList 未完成！</br>";
            }


            if (!string.IsNullOrEmpty(err))
            {
                return Ok(err);
            }
            else
            {
                return Ok(true);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sn">如果sn为null不需要走K2流程</param>
        /// <param name="userAccount"></param>
        /// <param name="userNameENUS"></param>
        /// <param name="userNameZHCN"></param>
        /// <returns></returns>
        [Route("api/ClosureTool/GenClosureTool/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GenClosureTool(Guid id, string sn = null, string userAccount = null,
            string userNameENUS = null, string userNameZHCN = null)
        {
            var attachmentName = "ClosureTool.xls";
            var closureTool = ClosureTool.Get(id);
            //评论信息
            var att = Attachment.FirstOrDefault(_att => _att.RefTableID == id.ToString() && _att.Name == attachmentName);
            if (att == null)
            {
                att = new Attachment();
                att.ID = Guid.NewGuid();
            }
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (string.IsNullOrEmpty(userAccount))
                {
                    userAccount = ClientCookie.UserCode;
                }
                if (string.IsNullOrEmpty(userNameENUS))
                {
                    userNameENUS = ClientCookie.UserNameENUS;
                }
                if (string.IsNullOrEmpty(userNameZHCN))
                {
                    userNameZHCN = ClientCookie.UserNameZHCN;
                }

                var current = System.Web.HttpContext.Current;
                string path = SiteFilePath.Template_DIRECTORY + "\\" + SiteFilePath.Store_Closure_Tool_Template;

                // Excel excel = new Excel();
                string templatePath = path;
                string internalName = Guid.NewGuid() + ".xlsx";


                string tempFilePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + internalName;
                //excel.Open(path);
                //excel.Save(tempFilePath);

                //  Excel tempExcel = new Excel();
                //  tempExcel.Open(tempFilePath);
                #region Old Code
                //var sheet = tempExcel.Sheets["PMT"];


                //Store store = new Store();




                //var closureInfo = closureInfoHandler.GetByProjectId(closureTool.ProjectId);

                //var storeInfo = store.GetStoreEntity(closureInfo.USCode);


                //sheet.Cells[1, 1].StrValue = storeInfo.MarketName;
                //sheet.Cells[3, 1].StrValue = storeInfo.NameZHCN;

                //if (storeInfo.OpenDate != null)
                //    sheet.Cells[4, 1].StrValue = storeInfo.OpenDate.Value.ToString("yyyy-MM-dd");
                //sheet.Cells[5, 1].StrValue = storeInfo.LL0Contact;

                //sheet.Cells[7, 1].StrValue = closureInfo.ClosureTypeNameZHCN;

                //if (closureInfo.ActualCloseDate != null)
                //{
                //    sheet.Cells[8, 1].StrValue = closureInfo.ActualCloseDate.Value.ToString("yyyy-MM-dd");
                //}
                //sheet.Cells[9, 1].StrValue = GetCellValue(closureTool.NonProduct_Sales_RMB);
                //sheet.Cells[10, 1].StrValue = GetCellValue(closureTool.PAC_RMB_Adjustment);


                //    sheet.Cells[11, 1].StrValue = GetCellValue(closureTool.Rent_RMB_Adjustment);



                //    sheet.Cells[12, 1].StrValue = GetCellValue(closureTool.DepreciationLHI_RMB_Adjustment);


                //    sheet.Cells[13, 1].StrValue = GetCellValue(closureTool.Interest_ESSD_RMB_Adjustment);





                ////INTEREST LHI 
                //// sheet.Cells[15, 2].StrValue = closureTool..ToString();

                //    sheet.Cells[14, 1].StrValue = GetCellValue(closureTool.ServiceFee_RMB_Adjustment);


                //    sheet.Cells[15, 1].StrValue = GetCellValue(closureTool.Accounting_RMB_Adjustment);


                //    sheet.Cells[16, 1].StrValue = GetCellValue(closureTool.Accounting_RMB_Adjustment);


                //    sheet.Cells[17, 1].StrValue = GetCellValue(closureTool.TaxesLicenses_RMB_Adjustment);


                //    sheet.Cells[18, 1].StrValue = GetCellValue(closureTool.Depreciation_ESSD_RMB_Adjustment);


                //    sheet.Cells[19, 1].StrValue = GetCellValue(closureTool.Interest_ESSD_RMB_Adjustment);


                //    sheet.Cells[20, 1].StrValue = GetCellValue(closureTool.OtherIncExp_RMB_Adjustment);

                //sheet.Cells[21, 1].StrValue = GetCellValue(closureTool.NonProduct_Sales_RMB_Adjustment);
                //sheet.Cells[22, 1].StrValue = GetCellValue(closureTool.NonProduct_Costs_RMB_Adjustment);
                //sheet.Cells[23, 1].StrValue = GetCellValue(closureTool.CompSalesMacket_Adjustment);
                //sheet.Cells[24, 1].StrValue = GetCellValue(closureTool.CompCG_Adjustment);
                //sheet.Cells[25, 1].StrValue = GetCellValue(closureTool.CompCGMacket_Adjustment);
                //sheet.Cells[26, 1].StrValue = GetCellValue(closureTool.PACMarket_Adjustment);
                //sheet.Cells[27, 1].StrValue = GetCellValue(closureTool.SOIMarket_Adjustment);
                //sheet.Cells[28, 1].StrValue = GetCellValue(closureTool.TotalSales_TTMY1);
                //sheet.Cells[29, 1].StrValue = GetCellValue(closureTool.CompSales_TTMY1);
                //sheet.Cells[30, 1].StrValue = GetCellValue(closureTool.CompSales_Market_TTMY1);
                //sheet.Cells[31, 1].StrValue = GetCellValue(closureTool.CompGC_TTMY1);
                //sheet.Cells[32, 1].StrValue = GetCellValue(closureTool.CompGCMarket_TTMY1);

                //sheet.Cells[33, 1].StrValue = GetCellValue(closureTool.PAC_TTMY1);
                //sheet.Cells[34, 1].StrValue = GetCellValue(closureTool.PACMarket_TTMY1);

                //sheet.Cells[35, 1].StrValue = GetCellValue(closureTool.SOI_TTMY1);
                //sheet.Cells[36, 1].StrValue = GetCellValue(closureTool.SOIMarket_TTMY1);

                //sheet.Cells[37, 1].StrValue = GetCellValue(closureTool.CashFlow_TTMY1);
                //sheet.Cells[38, 1].StrValue = GetCellValue(closureTool.TotalSales_TTMY2);
                ////sheet.Cells[36, 2].StrValue = closureTool.cashflow.ToString(); 


                //sheet.Cells[39, 1].StrValue = GetCellValue(closureTool.CompSales_TTMY2);
                //sheet.Cells[40, 1].StrValue = GetCellValue(closureTool.CompSales_Market_TTMY2);

                //sheet.Cells[41, 1].StrValue = GetCellValue(closureTool.CompGC_TTMY2);
                //sheet.Cells[42, 1].StrValue = GetCellValue(closureTool.CompGCMarket_TTMY2);

                //sheet.Cells[43, 1].StrValue = GetCellValue(closureTool.PAC_TTMY2);
                //sheet.Cells[44, 1].StrValue = GetCellValue(closureTool.PACMarket_TTMY2);

                //sheet.Cells[45, 1].StrValue = GetCellValue(closureTool.SOI_TTMY2);
                //sheet.Cells[46, 1].StrValue = GetCellValue(closureTool.SOIMarket_TTMY2);

                //sheet.Cells[47, 1].StrValue = GetCellValue(closureTool.CashFlow_TTMY2);

                //ClosureWOCheckList woCheckList = new ClosureWOCheckList();
                //var woEntity = woCheckList.GetByProjectID(closureTool.ProjectId);
                //if (woEntity != null)
                //{


                //    sheet.Cells[49, 1].StrValue = GetCellValue(woEntity.RE_Original);
                //    sheet.Cells[50, 1].StrValue = GetCellValue(woEntity.LHI_Original);
                //    sheet.Cells[51, 1].StrValue = GetCellValue(woEntity.ESSD_Original);
                //    sheet.Cells[52, 1].StrValue = GetCellValue(woEntity.RE_NBV);
                //    sheet.Cells[53, 1].StrValue = GetCellValue(woEntity.LHI_NBV);
                //    sheet.Cells[54, 1].StrValue = GetCellValue(woEntity.ESSD_NBV);
                //    sheet.Cells[55, 1].StrValue = GetCellValue(woEntity.EquipmentTransfer);
                //    sheet.Cells[56, 1].StrValue = GetCellValue(woEntity.ClosingCost);
                //}

                //var list = bllImpactStore.Search(e => e.ClosureId == id).ToList();
                //if (list.Count > 0)
                //{
                //    sheet.Cells[57, 1].StrValue = list[0].StoreCode;
                //    sheet.Cells[58, 1].StrValue = list[0].StoreName;
                //    sheet.Cells[59, 1].StrValue = list[0].ImpactSaltes.ToString();
                //}

                //if (list.Count > 1)
                //{
                //    sheet.Cells[60, 1].StrValue = list[0].StoreCode;
                //    sheet.Cells[61, 1].StrValue = list[1].StoreName;
                //    sheet.Cells[62, 1].StrValue = list[1].ImpactSaltes.ToString();
                //}



                //sheet.Cells[63, 1].StrValue = GetCellValue(closureTool.McppcoMargin);


                //sheet.Cells[64, 1].StrValue = GetCellValue(closureTool.MccpcoCashFlow);

                //sheet.Cells[65, 1].StrValue = GetCellValue(closureTool.Compensation);


                //sheet.Cells[66, 1].StrValue = GetCellValue(closureTool.CompAssumption);


                //sheet.Cells[67, 1].StrValue = GetCellValue(closureTool.CashflowGrowth);

                //if (closureTool.IsOptionOffered.HasValue )
                //{
                //    sheet.Cells[68, 1].StrValue =   closureTool.IsOptionOffered.Value?"Yes":"No";
                //}
                //sheet.Cells[69, 1].StrValue = closureTool.LeaseTerm;


                //sheet.Cells[70, 1].StrValue = GetCellValue(closureTool.Investment);


                //sheet.Cells[71, 1].StrValue = GetCellValue(closureTool.NPVRestaurantCashflows);

                //if (closureTool.Yr1SOI != null)
                //    sheet.Cells[72, 1].StrValue = closureTool.Yr1SOI.ToString();

                //if (closureTool.Yr1SOI != null)
                //    sheet.Cells[73, 1].StrValue = closureTool.IRR.ToString();

                ////if (closureTool.IRR != null)
                ////    sheet.Cells[71, 1].StrValue = closureTool.IRR.ToString();

                #endregion




                //tempExcel.Save(tempFilePath);
                File.Copy(templatePath, tempFilePath);
                var fileInfo = new FileInfo(tempFilePath);
                var excelOutputDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.ClosureTool);

                var inputInfo = new ExcelInputDTO
                {
                    ProjectId = closureTool.ProjectId

                };

                excelOutputDirector.Input(inputInfo);
                att.InternalName = internalName;
                att.RefTableName = ClosureTool.TableName;
                att.RefTableID = closureTool.Id.ToString();
                att.RelativePath = "//";
                att.Name = attachmentName;
                att.Extension = ".xlsx";
                att.CreateTime = DateTime.Now;
                att.TypeCode = ClosureTool.TableName;
                att.CreatorID = userAccount;
                att.CreatorNameENUS = userNameENUS;
                att.CreatorNameZHCN = userNameZHCN;
                att.RequirementId = new Guid("64A039C8-04A6-44F3-9310-F041D233A997");

                Attachment.SaveSigleFile(att);

                //att.FileURL = SiteFilePath.UploadFiles_URL + internalName;
                att.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + att.ID;
                //projectInfoBLL.UpdateProjectNode(closureTool.ProjectId, FlowCode.Closure_ClosureTool,
                //    NodeCode.Finish);
                ProjectInfo.FinishNode(closureTool.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_Generate);



                tranScope.Complete();

            }
            return Ok(att);
        }

        public void RefreshClosureTool(string projectId)
        {
            var woCheckList = ClosureWOCheckList.Get(projectId);

            if (woCheckList != null && woCheckList.RefreshClosureTool.HasValue && woCheckList.RefreshClosureTool.Value)
            {
                var toolEntity = ClosureTool.Get(projectId);
                if (toolEntity != null)
                {
                    //判断是否满足生成closureTools的条件
                    if (toolEntity.EnableReGenClosureTool())
                    {

                        GenClosureTool(toolEntity.Id, toolEntity.UserAccount, toolEntity.UserNameZHCN, toolEntity.UserNameENUS);
                        CallClosureTool(toolEntity.Id);

                        //通知Finance Specialist和Asset Actor
                        var closureInfo = ClosureInfo.FirstOrDefault(i => i.ProjectId == projectId);
                        List<string> receiverList = new List<string>();
                        receiverList.Add(closureInfo.AssetActorAccount);
                        receiverList.Add(closureInfo.FinanceAccount);
                        var notificationMsg = new NotificationMsg()
                        {
                            FlowCode = FlowCode.Closure_WOCheckList,
                            ProjectId = projectId,
                            SenderCode = ClientCookie.UserCode,
                            Title = "由于WO Tool数据发生变化，Closure Tool文件已自动更新",
                            RefId = woCheckList.Id,
                            UsCode = woCheckList.USCode,
                            IsSendEmail = false,
                            ReceiverCodeList = receiverList
                        };
                        Notification.Send(notificationMsg);
                    }
                }
            }
        }

        [Route("api/ClosureTool/CallClosureTool/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult CallClosureTool(Guid id)
        {
            var attachmentName = "ClosureTool.xls";

            var closureTool = ClosureTool.Get(id);
            var att = Attachment.FirstOrDefault(_att => _att.RefTableID == id.ToString() && _att.Name == attachmentName);
            if (att != null && closureTool != null)
            {
                var filePath = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                //var fileInfo = new FileInfo(filePath);
                //var excelOutputDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.ClosureTool);
                //decimal TotalOneOffCosts = 0;
                //decimal.TryParse(excelOutputDirector.GetCellValue(2, "E"), out TotalOneOffCosts);
                //decimal OperatingIncome = 0;
                //decimal.TryParse(excelOutputDirector.GetCellValue(3, "E"), out OperatingIncome);
                //decimal CompensationReceipt = 0;
                //decimal.TryParse(excelOutputDirector.GetCellValue(4, "E"), out CompensationReceipt);
                //decimal ClosingCosts = 0;
                //decimal.TryParse(excelOutputDirector.GetCellValue(5, "E"), out ClosingCosts);
                //decimal NPVSC = 0;
                //decimal.TryParse(excelOutputDirector.GetCellValue(6, "E"), out NPVSC);

                var excelHandler = new ExcelHanlder(filePath, "PMT");

                decimal TotalOneOffCosts = TryParseDecimal(excelHandler.GetCellValue("E2").ToString());
                decimal OperatingIncome = TryParseDecimal(excelHandler.GetCellValue("E3").ToString());
                decimal CompensationReceipt = TryParseDecimal(excelHandler.GetCellValue("E4").ToString());
                decimal ClosingCosts = TryParseDecimal(excelHandler.GetCellValue("E5").ToString());
                double NPVSC = 0;
                double.TryParse(excelHandler.GetCellValue("E6").ToString(), out NPVSC);

                excelHandler.Dispose();

                closureTool.TotalOneOffCosts = TotalOneOffCosts;
                closureTool.OperatingIncome = OperatingIncome;
                closureTool.CompensationReceipt = CompensationReceipt;
                closureTool.ClosingCosts = ClosingCosts;
                closureTool.NPVSC = NPVSC;

                ClosureTool.Update(closureTool);
            }
            return Ok(closureTool);
        }

        public decimal TryParseDecimal(object val)
        {
            decimal result = 0;
            double temp = 0;
            if (double.TryParse(val.ToString(), out temp))
            {
                decimal.TryParse(temp.ToString("f10"), out result);
            }
            return result;
        }

        [Route("api/ClosureTool/DownLoadClosureTool/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult DownLoadClosureTool(Guid id)
        {
            var current = HttpContext.Current;
            var att = Attachment.Get(id);

            string absolutePath = current.Server.MapPath("~/") + "Temp/" + att.InternalName;
            string date = DateTime.Now.ToString("yyyyMMdd");
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DataConverter.ToHexString(att.Name + date + att.Extension));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile("" + absolutePath + "");
            current.Response.End();
            return Ok();

        }

        [Route("api/ClosureTool/SaveClosureTool")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult SaveClosureTool(PostClosureTool postCT)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {

                var entity = postCT.Entity;

                //var closureToolInfo = closureToolHandler.FirstOrDefault(e => e.ProjectId == entity.ProjectId);

                //第一次打开 TTM Financial Data 时，数据存到LDW_FinanceData表
                LDW_FinanceData.OriginalDataOperation(entity.Id, entity.ProjectId, postCT.yearMonth, Utils.GetLatestYear(), Utils.GetLatestMonth());

                if (entity.Id == new Guid())
                {

                    entity.Id = Guid.NewGuid();
                    entity.IsHistory = false;
                    entity.CreateTime = DateTime.Now;
                    entity.CreateUserAccount = ClientCookie.UserCode;
                    ClosureTool.Add(entity);
                }
                else
                {
                    entity.IsHistory = false;
                    ClosureTool.UpdateEntity(entity);
                    var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToArray();
                    ClosureToolImpactOtherStore.Delete(impactList);
                }

                if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
                {
                    foreach (var item in postCT.ImpactStores)
                    {
                        item.Id = Guid.NewGuid();
                        item.CreateTime = DateTime.Now;
                        item.ClosureId = entity.Id;
                        item.CreateUserAccount = ClientCookie.UserCode;
                    }
                    ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
                }

                string action = string.IsNullOrEmpty(entity.Action) ?
                ProjectCommentAction.Submit : entity.Action;


                SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Save);
                tranScope.Complete();
                return Ok(entity.Id);
            }

        }

        [Route("api/ClosureTool/UpdateClosureTool")]
        public IHttpActionResult UpdateClosureTool(PostClosureTool postCT)
        {
            var entity = postCT.Entity;
            ClosureTool.UpdateEntity(entity);
            SaveCommers(entity, ProjectCommentAction.Submit, ProjectCommentStatus.Save);

            var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToArray();
            ClosureToolImpactOtherStore.Delete(impactList);

            if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
            {
                foreach (var item in postCT.ImpactStores)
                {
                    item.Id = Guid.NewGuid();
                    item.CreateTime = DateTime.Now;
                    item.ClosureId = entity.Id;
                    item.CreateUserAccount = ClientCookie.UserCode;
                }
                ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
            }
            if (entity.Compensation != null)
            {
                var closureCurrentNode = NodeInfo.GetCurrentNode(entity.ProjectId, FlowCode.Closure_ClosureTool);
                var repInputNode = NodeInfo.GetNodeInfo(NodeCode.Closure_ClosureTool_RepInput);
                if (closureCurrentNode.Sequence < repInputNode.Sequence)
                    ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_RepInput);
            }
            return Ok(true);
        }

        [Route("api/ClosureTool/UploadAttachement/{projectId}")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UploadAttachement(string projectId)
        {
            var closureToolInfo = ClosureTool.FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
            using (TransactionScope tranScope = new TransactionScope())
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                HttpFileCollection FileCollect = request.Files;
                int result = 0;
                string internalName = string.Empty;
                string resultStr = string.Empty;


                if (FileCollect.Count > 0) //如果集合的数量大于0
                {
                    ClosureTool entity = new ClosureTool();


                    if (closureToolInfo == null)
                    {
                        closureToolInfo = new ClosureTool();
                        closureToolInfo.ProjectId = projectId;
                        closureToolInfo.Id = Guid.NewGuid();
                        closureToolInfo.CreateTime = DateTime.Now;
                        closureToolInfo.CreateUserAccount = ClientCookie.UserCode;


                        ClosureTool.Add(closureToolInfo);
                    }

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
                        att.RefTableName = ClosureTool.TableName;
                        att.RefTableID = closureToolInfo.Id.ToString();
                        att.RelativePath = "//";
                        att.Name = fileName;
                        att.Extension = fileExtension;
                        att.Length = FileCollect[0].ContentLength;
                        att.CreateTime = DateTime.Now;
                        att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                        att.CreatorNameENUS = ClientCookie.UserNameENUS;
                        att.CreatorID = ClientCookie.UserCode;
                        att.ID = Guid.NewGuid();
                        att.TypeCode = "Attachment";
                        Attachment.Add(att);
                    }
                }
                tranScope.Complete();
                return Ok();
            }

        }

        [System.Web.Http.HttpGet]
        [Route("api/ClosureTool/GetAttachements/{id}")]
        public IHttpActionResult GetAttachements(Guid id)
        {
            var list = Attachment.GetList(ClosureTool.TableName, id.ToString(), "Attachment");
            foreach (var item in list)
            {
                //item.FileURL = SiteFilePath.UploadFiles_URL + item.InternalName;
                item.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + item.ID;
            }
            return Ok(list);
        }
        [System.Web.Http.HttpGet]
        [Route("api/ClosureTool/GetClosureTool/{refId}")]
        public IHttpActionResult GetClosureTool(Guid refId)
        {
            var attInfo = Attachment.GetAttachment(ClosureTool.TableName, refId.ToString(), ClosureTool.TableName);
            // var list = att.GetList(ClosureTool.TableName, id.ToString(), "Attachment");
            if (attInfo != null)
            {
                //attInfo.FileURL = SiteFilePath.UploadFiles_URL + attInfo.InternalName;
                attInfo.FileURL = SiteInfo.ServiceUrl + "api/attachment/download?id=" + attInfo.ID;
            }
            return Ok(attInfo);
        }

        [Route("api/ClosureTool/ProcessClosureTool")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult ProcessClosureTool(PostClosureTool postCT)
        {
            //using (TransactionScope tranScope = new TransactionScope())
            //{
            var entity = postCT.Entity;
            int procInstID = postCT.Entity.ProcInstID.Value;

            string actionLower = entity.Action.ToLower();
            string account = ClientCookie.UserCode;
            //评论信息
            string comments = entity.Comments;
            string _procCode = WFClosureTool.ProcessCode;
            // To-Do K2 action
            ProcessActionResult _action = BPMHelper.ConvertToProcAction(actionLower);

            if (actionLower == "resubmit")
            {
                UserPositionHandler handler = new UserPositionHandler();
                var closureInfo = ClosureInfo.GetByProjectId(entity.ProjectId);
                //第一次打开 TTM Financial Data 时，数据存到LDW_FinanceData表
                LDW_FinanceData.OriginalDataOperation(entity.Id, entity.ProjectId, postCT.yearMonth, Utils.GetLatestYear(), Utils.GetLatestMonth());

                var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == entity.Id).ToArray();
                ClosureToolImpactOtherStore.Delete(impactList);

                if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
                {
                    foreach (var item in postCT.ImpactStores)
                    {
                        item.Id = Guid.NewGuid();
                        item.CreateTime = DateTime.Now;
                        item.ClosureId = entity.Id;
                        item.CreateUserAccount = ClientCookie.UserCode;
                    }
                    ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
                }

                List<ProcessDataField> _listDataFields = new List<ProcessDataField>();
                _listDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                _listDataFields.Add(new ProcessDataField("dest_FinSupervisor", entity.FinReportToAccount));
                _listDataFields.Add(new ProcessDataField("ProcessCode", _procCode));
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments, _listDataFields);
            }
            else
                K2FxContext.Current.ApprovalProcess(entity.SN, account, _action.ToString(), comments);
            ClosureTool.UpdateEntity(entity);
            switch (entity.Action)
            {
                case ProjectAction.Recall:
                    ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosureTool, ProjectStatus.Recalled);
                    break;
                case ProjectAction.Return:
                    ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosureTool);
                    break;
                case ProjectAction.Decline:
                    ProjectInfo.Reject(entity.ProjectId, FlowCode.Closure_ClosureTool);
                    break;
                case ProjectAction.ReSubmit:
                    ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_FinInput);
                    break;
                default:
                    ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_ClosureTool, NodeCode.Closure_ClosureTool_FinApprove);
                    break;
            }


            if (postCT.Entity.Action == "ReSubmit" || postCT.Entity.Action == "Return")
            {

                ClosureTool.UpdateEntity(postCT.Entity);


                if (postCT.Entity.Action == "ReSubmit")
                {
                    var impactList = ClosureToolImpactOtherStore.Search(e => e.ClosureId == postCT.Entity.Id).ToArray();
                    ClosureToolImpactOtherStore.Delete(impactList);
                    if (postCT.ImpactStores != null && postCT.ImpactStores.Count > 0)
                    {
                        foreach (var item in postCT.ImpactStores)
                        {
                            item.Id = Guid.NewGuid();
                            item.CreateTime = DateTime.Now;
                            item.ClosureId = postCT.Entity.Id;
                            item.CreateUserAccount = ClientCookie.UserCode;
                        }
                        ClosureToolImpactOtherStore.Add(postCT.ImpactStores.ToArray());
                    }



                }
            }
            //ProjectInfo.UpdateProjectStatus(entity.ProjectId, FlowCode.Closure_ClosureTool, entity.GetProjectStatus(entity.Action));
            SaveCommers(entity, _action.ToString(), ProjectCommentStatus.Submit);
            // tranScope.Complete();
            return Ok();
            //}
        }



        [Route("api/ClosureTool/GetK2Status/{account}/{sn}/{procInstID}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetK2Status(string account, string sn, string procInstID)
        {
            // Load K2 Process
            bool result = false;
            var resultStr = K2FxContext.Current.GetCurrentActivityName(sn, account);
            result = !resultStr.Equals(WFClosureTool.Act_Originator, StringComparison.CurrentCultureIgnoreCase);

            // 判断是否为Asset Actor 节点, 用于显示“生成closure tool”按钮
            bool _isActor = resultStr.Equals(WFClosureTool.Act_AssetActor, StringComparison.CurrentCultureIgnoreCase);

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

        [Route("api/closure/closureTool/compensation")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetCompensation(string projectId)
        {
            var closureTool = ClosureTool.FirstOrDefault(ct => ct.ProjectId == projectId && !ct.IsHistory.Value);
            if (closureTool != null)
            {
                return Ok(closureTool.Compensation);
            }
            else
            {
                return NotFound();
            }
        }


        [Route("api/closure/closureTool/financedata")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetFinanceData(Guid refTableId, string projectId, string financeYear = "", string financeMonth = "")
        {
            McdAMEntities amdb = new McdAMEntities();

            #region Fields Define

            decimal totalSales;
            decimal rent;
            decimal comp_sales_ttm;
            decimal comp_sales_market_ttm;
            decimal comp_gc_ttm;
            decimal comp_gc_market_ttm;
            decimal Pac_TTM;
            decimal PACPct_TTM;
            decimal PACPct_MARKET_TTM;
            decimal Depreciation_LHI_TTM;
            decimal Interest_LHI_TTM;
            decimal Service_Fee_TTM;
            decimal Insurance_TTM;
            decimal Accounting_TTM;
            decimal Taxes_Licenses_TTM;
            decimal Depreciation_Essd_TTM;
            decimal Interest_Essd_TTM;
            decimal Other_Exp_TTM;
            decimal Product_Sales_TTM;
            decimal Non_Product_Sales_TTM;
            decimal Non_Product_Costs_TTM;
            decimal SOI;
            decimal SOIPct_MARKET_TTM;
            decimal Cash_Flow_TTM;
            decimal Total_Sales_TTMPY1;
            decimal Total_Sales_TTMPY2;
            decimal comp_sales_ttm_py1;
            decimal comp_sales_ttm_py2;
            decimal comp_sales_market_ttm_py1;
            decimal comp_sales_market_ttm_py2;
            decimal comp_gc_ttm_py1;
            decimal comp_gc_ttm_py2;
            decimal comp_gc_market_ttm_py1;
            decimal comp_gc_market_ttm_py2;
            decimal PAC_TTMPreviousY1;
            decimal PAC_TTMPreviousY2;
            decimal PACPct_MARKET_TTMPreviousY1;
            decimal PACPct_MARKET_TTMPreviousY2;
            decimal SOI_TTMPreviousY1;
            decimal SOI_TTMPreviousY2;
            decimal SOIPct_MARKET_TTMPreviousY1;
            decimal SOIPct_MARKET_TTMPreviousY2;
            decimal Cash_Flow_TTMPreviousY1;
            decimal Cash_Flow_TTMPreviousY2;

            //是否需要重置FinanceData
            bool reset = false;
            #endregion

            LDW_FinanceData ldw_FinanceData;
            if (refTableId == Guid.Empty)
                ldw_FinanceData = LDW_FinanceData.Get(projectId);
            else
                ldw_FinanceData = LDW_FinanceData.GetByRefId(refTableId);

            if (string.IsNullOrEmpty(financeYear) && string.IsNullOrEmpty(financeMonth) && ldw_FinanceData != null)
            {
                financeYear = ldw_FinanceData.FinanceYear;
                financeMonth = ldw_FinanceData.FinanceMonth;

                #region Fields Set
                totalSales = CovertToDecimal(ldw_FinanceData.Total_Sales_TTM);
                rent = CovertToDecimal(ldw_FinanceData.Rent_TTM);
                comp_sales_ttm = CovertToDecimalPercent(ldw_FinanceData.comp_sales_ttm);
                comp_sales_market_ttm = CovertToDecimalPercent(ldw_FinanceData.comp_sales_market_ttm);
                comp_gc_ttm = CovertToDecimalPercent(ldw_FinanceData.comp_gc_ttm);
                comp_gc_market_ttm = CovertToDecimalPercent(ldw_FinanceData.comp_gc_market_ttm);
                Pac_TTM = CovertToDecimal(ldw_FinanceData.Pac_TTM);
                PACPct_TTM = CovertToDecimalPercent(ldw_FinanceData.PACPct_TTM);
                PACPct_MARKET_TTM = CovertToDecimal(ldw_FinanceData.PACPct_MARKET_TTM);
                Depreciation_LHI_TTM = CovertToDecimal(ldw_FinanceData.Depreciation_LHI_TTM);
                Interest_LHI_TTM = CovertToDecimal(ldw_FinanceData.Interest_LHI_TTM);
                Service_Fee_TTM = CovertToDecimal(ldw_FinanceData.Service_Fee_TTM);
                Insurance_TTM = CovertToDecimal(ldw_FinanceData.Insurance_TTM);
                Accounting_TTM = CovertToDecimal(ldw_FinanceData.Accounting_TTM);
                Taxes_Licenses_TTM = CovertToDecimal(ldw_FinanceData.Taxes_Licenses_TTM);
                Depreciation_Essd_TTM = CovertToDecimal(ldw_FinanceData.Depreciation_Essd_TTM);
                Interest_Essd_TTM = CovertToDecimal(ldw_FinanceData.Interest_Essd_TTM);
                Other_Exp_TTM = CovertToDecimal(ldw_FinanceData.Other_Exp_TTM);
                Product_Sales_TTM = CovertToDecimal(ldw_FinanceData.ProductSales_TTM);
                Non_Product_Sales_TTM = CovertToDecimal(ldw_FinanceData.Non_Product_Sales_TTM);
                Non_Product_Costs_TTM = CovertToDecimal(ldw_FinanceData.Non_Product_Costs_TTM);
                SOI = CovertToDecimalPercent(ldw_FinanceData.SOIPct_TTM);
                SOIPct_MARKET_TTM = CovertToDecimalPercent(ldw_FinanceData.SOIPct_MARKET_TTM);
                Cash_Flow_TTM = CovertToDecimal(ldw_FinanceData.CashFlow_TTM);
                Total_Sales_TTMPY1 = CovertToDecimal(ldw_FinanceData.Total_Sales_TTMPY1);
                Total_Sales_TTMPY2 = CovertToDecimal(ldw_FinanceData.Total_Sales_TTMPY2);
                comp_sales_ttm_py1 = CovertToDecimalPercent(ldw_FinanceData.comp_sales_ttm_py1);
                comp_sales_ttm_py2 = CovertToDecimalPercent(ldw_FinanceData.comp_sales_ttm_py2);
                comp_sales_market_ttm_py1 = CovertToDecimalPercent(ldw_FinanceData.comp_sales_market_ttm_py1);
                comp_sales_market_ttm_py2 = CovertToDecimalPercent(ldw_FinanceData.comp_sales_market_ttm_py2);
                comp_gc_ttm_py1 = CovertToDecimalPercent(ldw_FinanceData.comp_gc_ttm_py1);
                comp_gc_ttm_py2 = CovertToDecimalPercent(ldw_FinanceData.comp_gc_ttm_py2);
                comp_gc_market_ttm_py1 = CovertToDecimalPercent(ldw_FinanceData.comp_gc_market_ttm_py1);
                comp_gc_market_ttm_py2 = CovertToDecimalPercent(ldw_FinanceData.comp_gc_market_ttm_py2);
                PAC_TTMPreviousY1 = CovertToDecimalPercent(ldw_FinanceData.PAC_TTMPreviousY1);
                PAC_TTMPreviousY2 = CovertToDecimalPercent(ldw_FinanceData.PAC_TTMPreviousY2);
                PACPct_MARKET_TTMPreviousY1 = CovertToDecimalPercent(ldw_FinanceData.PACPct_MARKET_TTMPreviousY1);
                PACPct_MARKET_TTMPreviousY2 = CovertToDecimalPercent(ldw_FinanceData.PACPct_MARKET_TTMPreviousY2);
                SOI_TTMPreviousY1 = CovertToDecimalPercent(ldw_FinanceData.SOI_TTMPreviousY1);
                SOI_TTMPreviousY2 = CovertToDecimalPercent(ldw_FinanceData.SOI_TTMPreviousY2);
                SOIPct_MARKET_TTMPreviousY1 = CovertToDecimalPercent(ldw_FinanceData.SOIPct_MARKET_TTMPreviousY1);
                SOIPct_MARKET_TTMPreviousY2 = CovertToDecimalPercent(ldw_FinanceData.SOIPct_MARKET_TTMPreviousY2);
                Cash_Flow_TTMPreviousY1 = CovertToDecimal(ldw_FinanceData.Cash_Flow_TTMPreviousY1);
                Cash_Flow_TTMPreviousY2 = CovertToDecimal(ldw_FinanceData.Cash_Flow_TTMPreviousY2);
                #endregion
            }
            else
            {
                if (string.IsNullOrEmpty(financeYear) && string.IsNullOrEmpty(financeMonth))
                {
                    var yearMonthObj = amdb.StoreSTMonthlyFinaceInfoTTM.FirstOrDefault();
                    financeYear = Utils.GetLatestYear();
                    financeMonth = Utils.GetLatestMonth();
                    if (yearMonthObj != null && !string.IsNullOrEmpty(yearMonthObj.TTMValue))
                    {
                        financeYear = yearMonthObj.TTMValue.Substring(0, yearMonthObj.TTMValue.IndexOf('-'));
                        financeMonth = yearMonthObj.TTMValue.Substring(yearMonthObj.TTMValue.IndexOf('-') + 1);
                    }
                }
                var uscode = ClosureInfo.FirstOrDefault(ci => ci.ProjectId == projectId).USCode;
                var storeId = amdb.StoreBasicInfo.Where(s => s.StoreCode.Equals(uscode)).Select(id => id.StoreID).FirstOrDefault();
                var financeData = amdb.DataSync_LDW_AM_STFinanceData.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();
                var financeData2 = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();

                #region Fields Set
                totalSales = CovertToDecimal(financeData.Total_Sales_TTM);
                rent = CovertToDecimal(financeData2.Rent_TTM);
                comp_sales_ttm = CovertToDecimalPercent(financeData2.comp_sales_ttm);
                comp_sales_market_ttm = CovertToDecimalPercent(financeData2.comp_sales_market_ttm);
                comp_gc_ttm = CovertToDecimalPercent(financeData2.comp_gc_ttm);
                comp_gc_market_ttm = CovertToDecimalPercent(financeData2.comp_gc_market_ttm);
                Pac_TTM = CovertToDecimal(financeData2.Pac_TTM);
                PACPct_TTM = CovertToDecimalPercent(financeData2.PACPct_TTM);
                PACPct_MARKET_TTM = CovertToDecimal(financeData2.PACPct_MARKET_TTM);
                Depreciation_LHI_TTM = CovertToDecimal(financeData2.Depreciation_LHI_TTM);
                Interest_LHI_TTM = CovertToDecimal(financeData2.Interest_LHI_TTM);
                Service_Fee_TTM = CovertToDecimal(financeData2.Service_Fee_TTM);
                Insurance_TTM = CovertToDecimal(financeData2.Insurance_TTM);
                Accounting_TTM = CovertToDecimal(financeData2.Accounting_TTM);
                Taxes_Licenses_TTM = CovertToDecimal(financeData2.Taxes_Licenses_TTM);
                Depreciation_Essd_TTM = CovertToDecimal(financeData2.Depreciation_Essd_TTM);
                Interest_Essd_TTM = CovertToDecimal(financeData2.Interest_Essd_TTM);
                Other_Exp_TTM = CovertToDecimal(financeData2.Other_Exp_TTM);
                Product_Sales_TTM = CovertToDecimal(financeData.ProductSales_TTM);
                Non_Product_Sales_TTM = CovertToDecimal(financeData2.Non_Product_Sales_TTM);
                Non_Product_Costs_TTM = CovertToDecimal(financeData2.Non_Product_Costs_TTM);
                SOI = CovertToDecimalPercent(financeData.SOIPct_TTM);
                SOIPct_MARKET_TTM = CovertToDecimalPercent(financeData2.SOIPct_MARKET_TTM);
                Cash_Flow_TTM = CovertToDecimal(financeData2.CASH_FLOW_TTM);
                Total_Sales_TTMPY1 = CovertToDecimal(financeData2.Total_Sales_TTMPY1);
                Total_Sales_TTMPY2 = CovertToDecimal(financeData2.Total_Sales_TTMPY2);
                comp_sales_ttm_py1 = CovertToDecimalPercent(financeData2.comp_sales_ttm_py1);
                comp_sales_ttm_py2 = CovertToDecimalPercent(financeData2.comp_sales_ttm_py2);
                comp_sales_market_ttm_py1 = CovertToDecimalPercent(financeData2.comp_sales_market_ttm_py1);
                comp_sales_market_ttm_py2 = CovertToDecimalPercent(financeData2.comp_sales_market_ttm_py2);
                comp_gc_ttm_py1 = CovertToDecimalPercent(financeData2.comp_gc_ttm_py1);
                comp_gc_ttm_py2 = CovertToDecimalPercent(financeData2.comp_gc_ttm_py2);
                comp_gc_market_ttm_py1 = CovertToDecimalPercent(financeData2.comp_gc_market_ttm_py1);
                comp_gc_market_ttm_py2 = CovertToDecimalPercent(financeData2.comp_gc_market_ttm_py2);
                PAC_TTMPreviousY1 = CovertToDecimalPercent(financeData2.PAC_TTMPreviousY1);
                PAC_TTMPreviousY2 = CovertToDecimalPercent(financeData2.PAC_TTMPreviousY2);
                PACPct_MARKET_TTMPreviousY1 = CovertToDecimalPercent(financeData2.PACPct_MARKET_TTMPreviousY1);
                PACPct_MARKET_TTMPreviousY2 = CovertToDecimalPercent(financeData2.PACPct_MARKET_TTMPreviousY2);
                SOI_TTMPreviousY1 = CovertToDecimalPercent(financeData2.SOI_TTMPreviousY1);
                SOI_TTMPreviousY2 = CovertToDecimalPercent(financeData2.SOI_TTMPreviousY2);
                SOIPct_MARKET_TTMPreviousY1 = CovertToDecimalPercent(financeData2.SOIPct_MARKET_TTMPreviousY1);
                SOIPct_MARKET_TTMPreviousY2 = CovertToDecimalPercent(financeData2.SOIPct_MARKET_TTMPreviousY2);
                Cash_Flow_TTMPreviousY1 = CovertToDecimal(financeData2.Cash_Flow_TTMPreviousY1);
                Cash_Flow_TTMPreviousY2 = CovertToDecimal(financeData2.Cash_Flow_TTMPreviousY2);
                #endregion
            }
            string yearMonth = Utils.GetYearMonth(financeYear, financeMonth);

            ////填写了Historical Financial Data后，判断是否保存过。（ClosureTool第一个环节提交时，会新建一条数据，但是没填写CreateTime，实际新建时间是填写完Historical Financial Data后，点保存或提交）
            //bool closureToolSaved = true;
            //var closureTool = ClosureTool.Get(projectId);
            //if (closureTool == null || closureTool.CreateTime == null)
            //    closureToolSaved = false;
            if (ldw_FinanceData != null && (financeYear != ldw_FinanceData.FinanceYear || financeMonth != ldw_FinanceData.FinanceMonth))
                reset = true;
            else if (ldw_FinanceData == null)
                reset = true;

            return Ok(new
            {
                yearMonth = yearMonth,
                totalSales = totalSales,
                rent = rent,
                comp_sales_ttm = comp_sales_ttm,
                comp_sales_market_ttm = comp_sales_market_ttm,
                comp_gc_ttm = comp_gc_ttm,
                comp_gc_market_ttm = comp_gc_market_ttm,
                Pac_TTM = Pac_TTM,
                PACPct_TTM = PACPct_TTM,
                PACPct_MARKET_TTM = PACPct_MARKET_TTM,
                Depreciation_LHI_TTM = Depreciation_LHI_TTM,
                Interest_LHI_TTM = Interest_LHI_TTM,
                Service_Fee_TTM = Service_Fee_TTM,
                Insurance_TTM = Insurance_TTM,
                Accounting_TTM = Accounting_TTM,
                Taxes_Licenses_TTM = Taxes_Licenses_TTM,
                Depreciation_Essd_TTM = Depreciation_Essd_TTM,
                Interest_Essd_TTM = Interest_Essd_TTM,
                Other_Exp_TTM = Other_Exp_TTM,
                Product_Sales_TTM = Product_Sales_TTM,
                Non_Product_Sales_TTM = Non_Product_Sales_TTM,
                Non_Product_Costs_TTM = Non_Product_Costs_TTM,
                SOI = SOI,
                SOIPct_MARKET_TTM = SOIPct_MARKET_TTM,
                Cash_Flow_TTM = Cash_Flow_TTM,
                Total_Sales_TTMPY1 = Total_Sales_TTMPY1,
                Total_Sales_TTMPY2 = Total_Sales_TTMPY2,
                comp_sales_ttm_py1 = comp_sales_ttm_py1,
                comp_sales_ttm_py2 = comp_sales_ttm_py2,
                comp_sales_market_ttm_py1 = comp_sales_market_ttm_py1,
                comp_sales_market_ttm_py2 = comp_sales_market_ttm_py2,
                comp_gc_ttm_py1 = comp_gc_ttm_py1,
                comp_gc_ttm_py2 = comp_gc_ttm_py2,
                comp_gc_market_ttm_py1 = comp_gc_market_ttm_py1,
                comp_gc_market_ttm_py2 = comp_gc_market_ttm_py2,
                PAC_TTMPreviousY1 = PAC_TTMPreviousY1,
                PAC_TTMPreviousY2 = PAC_TTMPreviousY2,
                PACPct_MARKET_TTMPreviousY1 = PACPct_MARKET_TTMPreviousY1,
                PACPct_MARKET_TTMPreviousY2 = PACPct_MARKET_TTMPreviousY2,
                SOI_TTMPreviousY1 = SOI_TTMPreviousY1,
                SOI_TTMPreviousY2 = SOI_TTMPreviousY2,
                SOIPct_MARKET_TTMPreviousY1 = SOIPct_MARKET_TTMPreviousY1,
                SOIPct_MARKET_TTMPreviousY2 = SOIPct_MARKET_TTMPreviousY2,
                Cash_Flow_TTMPreviousY1 = Cash_Flow_TTMPreviousY1,
                Cash_Flow_TTMPreviousY2 = Cash_Flow_TTMPreviousY2,
                reset = reset
            });
        }

        [Route("api/closure/closureTool/selectyearmonth")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetSelectYearMonth(string projectId)
        {
            using (McdAMEntities amdb = new McdAMEntities())
            {
                var selectItemList = new List<SelectItem>();
                var uscode = ClosureInfo.FirstOrDefault(ci => ci.ProjectId == projectId).USCode;
                var yearMonthList = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == uscode).Select(i => new { financeYearMonth = i.FinanceYear + "-" + i.FinanceMonth }).Distinct().OrderByDescending(i => i.financeYearMonth).Take(12).ToList();
                var ldw_financeData = LDW_FinanceData.Get(projectId);
                foreach (var _yearMonth in yearMonthList)
                {
                    var selectItem = new SelectItem();
                    selectItem.name = _yearMonth.financeYearMonth;
                    selectItem.value = _yearMonth.financeYearMonth;
                    if (ldw_financeData != null)
                        selectItem.selected = _yearMonth.financeYearMonth == ldw_financeData.FinanceYear + "-" + ldw_financeData.FinanceMonth;
                    else
                        selectItem.selected = false;
                    selectItemList.Add(selectItem);
                }
                return Ok(selectItemList);
            }
        }


        private decimal CovertToDecimal(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }
            else
            {
                data = data.Trim().TrimEnd('%').Replace(",", "");
                decimal r = 0;
                decimal.TryParse(data, out r);
                return r;
            }
        }

        public decimal CovertToDecimalPercent(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }
            else
            {
                data = data.Trim().TrimEnd('%').Replace(",", "");
                decimal r = 0;
                decimal.TryParse(data, out r);
                return r;
            }
        }

        private void SaveCommers(ClosureTool entity, string action, ProjectCommentStatus status)
        {
            if (status == ProjectCommentStatus.Save)
            {
                var list = ProjectComment.Search(c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == status && c.RefTableName == ClosureTool.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    if (entity.Comments != null)
                    {
                        closureCommens.Content = entity.Comments.Trim();
                    }
                    ProjectComment.Update(closureCommens);
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }
            else
            {
                var list = ProjectComment.Search(c => c.CreateUserAccount == ClientCookie.UserCode && c.RefTableId == entity.Id && c.Status == ProjectCommentStatus.Save && c.RefTableName == ClosureTool.TableName && c.SourceCode == FlowCode.Closure).ToList();

                if (list.Count > 0)
                {
                    ProjectComment closureCommens = list[0];
                    if (entity.Comments != null)
                    {
                        closureCommens.Content = entity.Comments.Trim();
                    }
                    closureCommens.Status = ProjectCommentStatus.Submit;
                    ProjectComment.Update(closureCommens);
                }
                else
                {
                    AddProjectComment(entity, action, status);
                }
            }

        }

        private void AddProjectComment(ClosureTool entity, string action,
            ProjectCommentStatus status)
        {
            ProjectComment closureCommens = new ProjectComment();
            closureCommens.RefTableId = entity.Id;
            closureCommens.RefTableName = ClosureTool.TableName;

            closureCommens.TitleNameENUS = ClientCookie.TitleENUS;
            closureCommens.TitleNameZHCN = ClientCookie.TitleENUS;
            closureCommens.TitleCode = ClientCookie.TitleENUS;

            closureCommens.CreateTime = DateTime.Now;
            closureCommens.CreateUserAccount = ClientCookie.UserCode;
            if (entity.ProcInstID > 0)
            {
                closureCommens.ProcInstID = entity.ProcInstID;
            }
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

            closureCommens.SourceCode = FlowCode.Closure;
            closureCommens.SourceNameENUS = FlowCode.Closure;
            closureCommens.SourceNameZHCN = "关店流程";
            closureCommens.Add();
            //ProjectComment.Add(closureCommens);
        }

        private class SelectItem
        {
            public string name;
            public string value;
            public bool selected;
        }

        [Route("api/closureTool/Edit")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Edit(ClosureTool entity)
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

        //private void ModifyProject(ClosureTool entity, string action)
        //{
        //    using (TransactionScope tranScope = new TransactionScope())
        //    {

        //        if (action == ProjectAction.Edit)
        //        {
        //            entity.IsHistory = true;

        //            ClosureTool.UpdateEntity(entity);
        //            var projectInfo = ProjectInfo.Get(entity.ProjectId, FlowCode.Closure_ClosureTool);
        //            projectInfo.Status = ProjectStatus.UnFinish;
        //            ProjectInfo.Update(projectInfo);
        //        }

        //        ProjectInfo.UpdateProjectNode(entity.ProjectId, FlowCode.Closure_ClosureTool,

        //        NodeCode.Start);
        //        tranScope.Complete();

        //    }
        //}

        [Route("api/closureTool/Recall")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Recall(ClosureTool entity)
        {
            bool _recallSuccess = false;
            if (entity.ProcInstID != null)
            {
                _recallSuccess = K2FxContext.Current.GoToActivityAndRecord(entity.ProcInstID.Value, WFClosureTool.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, entity.Comments);
                if (_recallSuccess)
                {

                    SaveCommers(entity, ProjectCommentAction.Recall, ProjectCommentStatus.Submit);
                }
            }
            if (!_recallSuccess)
            {
                throw new Exception("Recall失败");
            }

            ProjectInfo.Reset(entity.ProjectId, FlowCode.Closure_ClosureTool, ProjectStatus.Recalled);
            /*
            if (_recallSuccess)
            {
                ModifyProject(entity, ProjectAction.Recall);
            }*/

            return Ok();
        }
    }
}
