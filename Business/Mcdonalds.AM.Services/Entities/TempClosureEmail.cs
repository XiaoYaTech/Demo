using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/12/2014 3:49:59 PM
 * FileName     :   TempClosurePackageEmail
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Entities.TempClosure
{
    public class TempClosureEmail
    {
        public static EmailSendingResultType SendLegalReviewEmail(TempClosureLegalReview entity)
        {
            var project = ProjectInfo.Get(entity.ProjectId, FlowCode.TempClosure_ClosurePackage);
            var storeBasic = StoreBasicInfo.GetStorInfo(project.USCode);
            var legal = ProjectUsers.GetProjectUser(entity.ProjectId, ProjectUserRoleCode.Legal);
            var actor = ProjectUsers.GetProjectUser(entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (EmailServiceClient emailClient = new EmailServiceClient())
            {
                EmailMessage email = new EmailMessage();
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApproverName", legal.UserNameENUS);
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", storeBasic.StoreCode);
                bodyValues.Add("StoreName", storeBasic.NameENUS + @" \ " + storeBasic.NameZHCN);
                bodyValues.Add("Actor", actor.UserNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", Constants.TempClosure_LegalReview);////--流程名称
                bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
                var viewPage = string.Format("{0}/TempClosure/Main#/ClosurePackage/View/param?projectId={1}",
                    HttpContext.Current.Request.Url.AbsolutePath, entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);
                email.EmailBodyValues = bodyValues;
                List<string> emailAddresses = Employee.Search(e => e.Code == legal.UserAccount).Select(e => e.Mail).ToList();
                emailAddresses.Add("Stephen.Wang@nttdata.com");
                emailAddresses.Add("Poyet.chen@nttdata.com");
                emailAddresses.Add("Cary.chen@nttdata.com");
                email.To = string.Join(";", emailAddresses);
                //return emailClient.SendNotificationEmail(email);
                return new EmailSendingResultType();
            }
        }
        public static List<EmailSendingResultType> SendPackageApprovalEmail(TempClosureInfo tempClosureInfo, TempClosurePackage entity, ApproveUsers approvers)
        {
            var project = ProjectInfo.Get(entity.ProjectId, FlowCode.TempClosure_ClosurePackage);
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
                    approvers.VPGM
                    //approvers.MCCLAssetMgr,
                    //approvers.MCCLAssetDtr
                };
                if (approvers.RegionalMgr != null)
                {
                    approveEmps.Add(approvers.RegionalMgr);
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
                var viewPage = string.Format("{0}/TempClosure/Main#/ClosurePackage/View/param?projectId={1}",
                    HttpContext.Current.Request.Url.Authority, entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);
                Dictionary<string, string> templateFileds = new Dictionary<string, string>();
                var flowInfo = FlowInfo.Get(FlowCode.TempClosure);
                templateFileds.Add("WorkflowName", flowInfo.NameENUS);
                templateFileds.Add("ProjectID", entity.ProjectId);
                templateFileds.Add("USCode", storeBasic.StoreCode);
                templateFileds.Add("City", storeBasic.CityZHCN);
                templateFileds.Add("Region", storeBasic.Region);
                templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
                templateFileds.Add("Market", storeBasic.Market);
                templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
                templateFileds.Add("StoreAge", Math.Floor((DateTime.Now - storeBasic.OpenDate).TotalDays / 365D).ToString());
                templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
                var storeInfo = StoreBasicInfo.GetStore(project.USCode);

                if (!string.IsNullOrEmpty(storeInfo.StoreContractInfo.EndYear))
                    templateFileds.Add("CurrentLeaseENDYear", (int.Parse(storeInfo.StoreContractInfo.EndYear) - storeInfo.CurrentYear).ToString());
                else
                    templateFileds.Add("CurrentLeaseENDYear", "");

                if (assetMgr != null)
                    templateFileds.Add("AssetsManager", assetMgr.UserNameENUS);
                templateFileds.Add("AssetsActor", assetActor.UserNameENUS);
                templateFileds.Add("AssetsRep", assetRep.UserNameENUS);
                templateFileds.Add("ClosureDate", tempClosureInfo.ActualTempClosureDate.ToString("yyyy-MM-dd"));
                templateFileds.Add("LeaseExpireDate", tempClosureInfo.LeaseExpireDate.HasValue ? tempClosureInfo.LeaseExpireDate.Value.ToString("yyyy-MM-dd") : "");
                templateFileds.Add("ReOpenDate", tempClosureInfo.ActualReopenDate.ToString("yyyy-MM-dd"));
                templateFileds.Add("RentFreeTerm", string.IsNullOrEmpty(entity.RentReliefClause) ? "否" : "是");
                templateFileds.Add("RentFreeStartDate", entity.RentReliefStartDate.HasValue ? entity.RentReliefStartDate.Value.ToString("yyyy-MM-dd") : "");
                templateFileds.Add("RentFreeEndDate", entity.RentReliefEndDate.HasValue ? entity.RentReliefEndDate.Value.ToString("yyyy-MM-dd") : "");
                templateFileds.Add("FreeRentTerm", entity.RentReliefClause);
                var approveRecords = ProjectComment.GetList("TempClosurePackage", entity.Id, ProjectCommentStatus.Submit).Select(pc => new SubmissionApprovalRecord
                {
                    OperatorID = pc.UserAccount,
                    OperatorName = pc.UserNameENUS,
                    OperatorTitle = pc.TitleNameENUS,
                    OperationDate = pc.CreateTime.HasValue ? pc.CreateTime.Value : DateTime.Now,
                    ActionName = pc.ActivityName,
                    Comments = pc.Content
                }).ToList();
                string imgPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.TempClosure, templateFileds, approveRecords);
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