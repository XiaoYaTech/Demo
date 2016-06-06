using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/9/2014 3:26:13 PM
 * FileName     :   RenewalEmail
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Entities
{
    public class RenewalEmail
    {
        public static List<EmailSendingResultType> SendPackageApprovalEmail(RenewalInfo info, RenewalPackage entity, ApproveUsers approvers)
        {
            var project = ProjectInfo.Get(entity.ProjectId, FlowCode.Renewal_Package);
            var storeBasic = StoreBasicInfo.GetStorInfo(project.USCode);
            var storeContract = StoreContractInfo.Search(c => c.StoreCode == project.USCode).OrderByDescending(c => c.CreatedTime).FirstOrDefault();
            var assetMgr = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == entity.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetManager);
            var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == entity.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            var assetRep = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == entity.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetRep);
            var results = new List<EmailSendingResultType>();
            using (EmailServiceClient emailClient = new EmailServiceClient())
            {
                List<SimpleEmployee> approveEmps = new List<SimpleEmployee> { 
                    approvers.MarketMgr,
                    approvers.GM,
                    approvers.MDD,
                    approvers.FC,
                    approvers.MCCLAssetDtr
                };
                if (approvers.RegionalMgr != null)
                {
                    approveEmps.Add(approvers.RegionalMgr);
                }
                if (approvers.CDO != null)
                {
                    approveEmps.Add(approvers.CDO);
                }
                if (approvers.ManagingDirector != null)
                {
                    approveEmps.Add(approvers.ManagingDirector);
                }
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", storeBasic.StoreCode);
                bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
                bodyValues.Add("Actor", assetActor.UserNameENUS);////--呈递人
                //bodyValues.Add("WorkflowName", Constants.TempClosure_Package);////--流程名称
                bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
                var viewPage = string.Format("{0}/Renewal/Main#/Package/View/param?projectId={1}",
                    HttpContext.Current.Request.Url.Authority, entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);
                Dictionary<string, string> templateFileds = entity.GetPrintTemplateFields();
                var approveRecords = ProjectComment.GetList("TempClosurePackage", entity.Id, ProjectCommentStatus.Submit).Select(pc => new SubmissionApprovalRecord
                {
                    OperatorID = pc.UserAccount,
                    OperatorName = pc.UserNameENUS,
                    OperatorTitle = pc.TitleNameENUS,
                    OperationDate = pc.CreateTime.HasValue ? pc.CreateTime.Value : DateTime.Now,
                    ActionName = pc.ActivityName,
                    Comments = pc.Content
                }).ToList();
                string imgPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.Renewal, templateFileds, approveRecords);
                approveEmps.ForEach(e =>
                {

                    EmailMessage email = new EmailMessage();
                    List<string> emailAddresses = new List<string>
                    {
                        e.Mail,
                        "Stephen.Wang@nttdata.com",
                        "Poyet.chen@nttdata.com",
                        "Cary.chen@nttdata.com"
                    };
                    if (bodyValues.ContainsKey("ApproverName"))
                    {
                        bodyValues["ApproverName"] = e.NameENUS;
                    }
                    else
                    {
                        bodyValues.Add("ApproverName", e.NameENUS);//--提交人
                    }
                    email.EmailBodyValues = bodyValues;
                    email.To = string.Join(";", emailAddresses);
                    email.Attachments = imgPath;
                    //var result =  emailClient.SendEmail(email);
                    results.Add(new EmailSendingResultType());
                });
                approvers.NoticeUsers.ForEach(e =>
                {
                    EmailMessage email = new EmailMessage();
                    List<string> emailAddresses = new List<string>
                    {
                        e.Mail,
                        "Stephen.Wang@nttdata.com",
                        "Poyet.chen@nttdata.com",
                        "Cary.chen@nttdata.com"
                    };
                    if (bodyValues.ContainsKey("ApproverName"))//--提交人
                    {
                        bodyValues["ApproverName"] = e.NameENUS;
                    }
                    else
                    {
                        bodyValues.Add("ApproverName", e.NameENUS);
                    }
                    email.EmailBodyValues = bodyValues;
                    email.To = string.Join(";", emailAddresses);
                    email.Attachments = imgPath;
                    //var result = emailClient.SendEmail(email);
                    results.Add(new EmailSendingResultType());
                });
            }
            return results;
        }
    }
}