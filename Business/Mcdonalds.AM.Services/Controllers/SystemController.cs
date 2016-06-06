using Mcdonalds.AM.ApiCaller;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class SystemController : ApiController
    {
        [Route("api/system/remindercount")]
        public IHttpActionResult GetTaskReminders()
        {
            TaskCountModel result = new TaskCountModel();
            result.TaskCount = TaskWork.GetUseableTaskWork().Count(c => c.ReceiverAccount == ClientCookie.UserCode && c.Status == TaskWorkStatus.UnFinish);
            result.ReminderCount = Remind.Count(c => c.ReceiverAccount == ClientCookie.UserCode
                && !c.IsReaded);
            result.NoticeCount = Notification.Count(ClientCookie.UserCode);
            //获取PMT任务数量
            string encryptUserCode = Cryptography.Encrypt(ClientCookie.UserCode, DateTime.Now.ToString("yyyyMMdd"), "oms");
            string redirectUrl = string.Format(Constants.AM_To_PMT_RedirectUrl, "&eid=" + encryptUserCode);
            string callPMTTaskUrl = string.Format(Constants.Get_PMT_TaskUrl, encryptUserCode);
            if (Constants.GetPMTTask)
            {
                var pmtTaskResult = ApiProxy.Call<PMTTaskCountModel>(callPMTTaskUrl);
                if (pmtTaskResult != null)
                {
                    result.PMT_ApproveCount = pmtTaskResult.ApproveCount;
                    result.PMT_DealCount = pmtTaskResult.DealCount;
                    result.PMT_NotifyCount = pmtTaskResult.NotifyCount;
                    result.PMT_RemindCount = pmtTaskResult.RemindCount;
                    result.PMT_Total = pmtTaskResult.Total;
                }
            }
            result.PMT_URL = redirectUrl;
            return Ok(result);
        }

        [Route("api/system/reminders")]
        public IHttpActionResult GetTaskReminders(int pageSize = 5)
        {
            ProjectReminder reminder = new ProjectReminder();
            reminder.TaskCount = TaskWork.GetUseableTaskWork().Count(c => c.ReceiverAccount == ClientCookie.UserCode && c.Status == 0 && c.ActionName != ProjectAction.Pending);
            reminder.Tasks = TaskWork.GetUseableTaskWork().Where(c => c.ReceiverAccount == ClientCookie.UserCode && c.Status == TaskWorkStatus.UnFinish)
                .OrderByDescending(c => c.CreateTime).Take(pageSize).ToList();
            reminder.RemindCount = Remind.Count(c => c.ReceiverAccount == ClientCookie.UserCode && !c.IsReaded);
            reminder.Reminds = Remind.Search(c => c.ReceiverAccount == ClientCookie.UserCode && !c.IsReaded)
                .OrderByDescending(c => c.CreateTime).Take(pageSize).ToList();
            return Ok(reminder);
        }
    }
}
