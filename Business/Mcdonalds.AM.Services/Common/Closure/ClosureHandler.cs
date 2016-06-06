using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common.Closure
{
    public class ClosureHandler
    {
        public EmailSendingResultType SendEmail(Guid id, string[] emailToEids, string workflowName, string projectID, string tableName)
        {
            var closureInfo = ClosureInfo.GetByProjectId(projectID);
            var store = StoreBasicInfo.GetStore(closureInfo.USCode);
            var storeBasic = store.StoreBasicInfo;
            EmailServiceClient emailClient = new EmailServiceClient();
            var bllProjectComment = new ProjectComment();
            EmailMessage email = new EmailMessage();
            //邮件模板中的数据
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            //邮件内容中的键值对
            bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
            bodyValues.Add("WorkflowName", workflowName);
            bodyValues.Add("StoreCode", storeBasic.StoreCode);
            bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
            bodyValues.Add("Actor", closureInfo.AssetActorNameENUS);////--呈递人
            var viewPage = string.Format("{0}/TempClosure/Main#/ClosurePackage/View/param?projectId={1}",
                HttpContext.Current.Request.Url.Authority, projectID);
            bodyValues.Add("FormUrl", viewPage);
            email.EmailBodyValues = bodyValues;
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            templateFileds.Add("WorkflowName", SystemCode.Instance.GetCodeName(FlowCode.TempClosure, ClientCookie.Language));
            templateFileds.Add("ProjectID", projectID);
            templateFileds.Add("USCode", storeBasic.StoreCode);
            templateFileds.Add("Region", storeBasic.Region);
            templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
            templateFileds.Add("Market", storeBasic.Market);
            templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
            templateFileds.Add("City", storeBasic.CityZHCN);
            templateFileds.Add("StoreAge", "");
            templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
            var currentLeaseENDYear = store.CurrentYear - int.Parse(store.StoreContractInfo.EndYear);
            templateFileds.Add("CurrentLeaseENDYear", currentLeaseENDYear.ToString());
            templateFileds.Add("AssetsManager", closureInfo.AssetManagerNameENUS);
            templateFileds.Add("AssetsActor", closureInfo.AssetActorNameENUS);
            templateFileds.Add("AssetsRep", closureInfo.AssetRepNameENUS);
            if (closureInfo.ActualCloseDate.HasValue && closureInfo.ActualCloseDate.Value.Year == 1900)
            {
                templateFileds.Add("ClosureDate", string.Empty);
            }
            else
            {
                templateFileds.Add("ClosureDate", closureInfo.ActualCloseDate.HasValue ? closureInfo.ActualCloseDate.Value.ToString("yyyy-MM-dd") : "");
            }
            templateFileds.Add("ClosureType", closureInfo.ClosureTypeNameENUS);
            templateFileds.Add("LandlordName", "");
            templateFileds.Add("LeaseExpireDate", "");
            templateFileds.Add("OriginalCFNPV","" );

            List<SubmissionApprovalRecord> approveRecords = new List<SubmissionApprovalRecord>();
            if (id != new Guid())
            {
                approveRecords = ProjectComment.GetList(tableName, id, ProjectCommentStatus.Submit).Select(pc => new SubmissionApprovalRecord
                {
                    OperatorID = pc.UserAccount,
                    OperatorName = pc.UserNameENUS,
                    OperatorTitle = pc.TitleNameENUS,
                    OperationDate = pc.CreateTime.HasValue ? pc.CreateTime.Value : DateTime.Now,
                    ActionName = pc.ActivityName,
                    Comments = pc.Content
                }).ToList();
            }
            string imgPath = HtmlConversionUtility.ConvertToImage(HtmlTempalteType.Default, templateFileds, approveRecords);
            List<string> emailAddresses = Employee.Search(e => emailToEids.Contains(e.Code)).Select(e => e.Mail).ToList();
            emailAddresses.Add("Stephen.Wang@nttdata.com");
            emailAddresses.Add("Poyet.chen@nttdata.com");
            emailAddresses.Add("Cary.chen@nttdata.com");
            //emailAddresses.Add("jiejiemilesaway@163.com");
            email.To = string.Join(";", emailAddresses);
            email.Attachments = imgPath;
            //EmailSendingResultType result = emailClient.SendEmail(email);
            return new EmailSendingResultType();
        }
    }
}