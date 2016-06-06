using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public class Constants
    {
        public const string DefaultHtmlTemplate = @"HtmlTemplates\DefaultTemplate.html";
        public const string DefaultHtmlTemplateAdding = @"HtmlTemplates\DefaultTemplate2.html";
        public const string DefaultHtmlTemplateWithComments = @"HtmlTemplates\DefaultTemplate3.html";
        public const string ClosureMemoHtmlTemplate = @"HtmlTemplates\ClosureMemo.html";
        public const string MajorLeaseHtmlTemplate = @"HtmlTemplates\MajorLeaseTemplate.html";
        public const string TempClosureHtmlTemplate = @"HtmlTemplates\TempClosureTemplate.html";
        public const string TempClosureReopenMemoHtmlTemplate = @"HtmlTemplates\TempClosureReopenMemo.html";
        public const string ReimageHtmlTemplate = @"HtmlTemplates\ReimageTemplate.html";
        public const string GBMemoHtmlTemplate = @"HtmlTemplates\GBMemoTemplate.html";
        public const string ReopenMemoHtmlTemplate = @"HtmlTemplates\ReopenMemoTemplate.html";
        public const string RenewalHtmlTemplate = @"HtmlTemplates\RenewalTemplate.html";
        public const string RenewalLegalApprovalTemplate = @"HtmlTemplates\RenewalLegalApprovalTemplate.html";
        public const string RebuildPackageHtmlTemplate = @"HtmlTemplates\RebuildPackageTemplate.html";
        public const string EmptyComments = "......";

        //流程名称: Project Name + K2 workflow name
        //Renewal 流程
        public const string Renewal_ReopenMemo = "Renewal_ReopenMemo";
        public const string Renewal = "Renewal";
        //Closure 流程:
        public const string Closure = "Closure";
        public const string Closure_Package = "Closure Package";
        public const string Closure_Memo = "Closure Memo";
        public const string Closure_ReopenMemo = "Closure Reopen Memo";
        //Tempclosure 流程：
        public const string TempClosure = "Temp Closure";
        public const string TempClosure_Memo = "Temp Closure Memo";
        public const string TempClosure_ReopenMemo = "Temp Closure Reopen Memo";
        public const string TempClosure_Package = "Temp Closure Package";
        public const string TempClosure_LegalReview = "Legal Review";
        //Major Lease 流程：
        public const string MajorLease = "Major Lease";
        public const string MajorLease_ReopenMemo = "Major Lease_ReopenMemo";
        public const string MajorLease_Package = "Major Lease Package";
        public const string MajorLease_ConsInvtChecking = "Cons Invt Checking";
        //Rebuild 流程：
        public const string Rebuild = "Rebuild";
        public const string Rebuild_Package = "Rebuild Package";
        public const string Rebuild_ConsInvtChecking = "Cons Invt Checking";
        public const string Rebuild_ReopenMemo = "Rebuild Reopen Memo";
        public const string Rebuild_GBMemo = "Rebuild GB Memo";
        //Reimage 流程：
        public const string Reimage = "Reimage";
        public const string Reimage_Package = "Reimage Package";
        public const string Reimage_ConsInvtChecking = "Cons Invt Checking";
        public const string Reimage_ReopenMemo = "Reimage Reopen Memo";
        public const string Reimage_GBMemo = "Reimage GB Memo";

        //Closure Package流程的Activity
        public const string Closure_Package_FirstApproveStepName = "Market Manager;Regional Manager";
        public const string Closure_Package_RejectStepName = "VPGM;CDO;CFO;Managing Director";

        //Temp Closure Package流程的Activity
        public const string TempClosure_Package_FirstApproveStepName = "M&R Manager;Regional Manager";
        public const string TempClosure_Package_RejectStepName = "VPGM";

        //MajorLease Package流程的Activity
        public const string MajorLease_Package_FirstApproveStepName = "Market Manager;Regional Manager";
        public const string MajorLease_Package_RejectStepName = "VPGM;Dev VP;CDO;CFO;Managing Director";

        //Rebuild Package流程的Activity
        public const string Rebuild_Package_FirstApproveStepName = "Market Manager;Regional Manager";
        public const string Rebuild_Package_RejectStepName = "VPGM;Dev VP;CDO;CFO;Managing Director";

        //Reimage Package流程的Activity
        public const string Reimage_Package_FirstApproveStepName = "Market Manager;Regional Manager";
        public const string Reimage_Package_RejectStepName = "VPGM;CDO;CFO;Managing Director";

        //Renewal Package流程的Activity
        public const string Renewal_Package_FirstApproveStepName = "Market Manager;Regional Manager";
        public const string Renewal_Package_RejectStepName = "VPGM;MCCL Asset Dir;CDO;Managing Director";

        //PMT Task/redirect URLs
        public static bool GetPMTTask = bool.Parse(ConfigurationManager.AppSettings["GetPMTTask"]);
        public static string Get_PMT_TaskUrl = ConfigurationManager.AppSettings["Get_PMT_TaskUrl"];
        public static string AM_To_PMT_RedirectUrl = ConfigurationManager.AppSettings["AM_To_PMT_RedirectUrl"];

    }
}