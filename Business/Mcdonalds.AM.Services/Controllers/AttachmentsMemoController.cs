using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using System.Web;
using System.IO;
using Mcdonalds.AM.Services.Infrastructure;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Enums;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class AttachmentsMemoController : ApiController
    {
        [HttpGet]
        [Route("api/AttachmentsMemo/GetMemoList")]
        public IHttpActionResult GetMemo(string flowCode,string projectId)
        {
            var memoList = AttachmentsMemo.GetAttachmentsMemoList(flowCode);
            if (memoList == null || memoList.Count == 0)
            {
                return Ok();
            }
            var navs = memoList.Select(e =>
            {
                string url = "";
                DateTime ?date = null;
                ProjectUsers user = null;
                switch (e.MemoCode)
                {
                    case "GBMemo":
                        user = ProjectUsers.GetProjectUser(projectId, ProjectUserRoleCode.PM);
                        var gbEntity = AttachmentsMemoProcessInfo.Get(projectId, FlowCode.GBMemo);
                        if (gbEntity!=null)
                            date = gbEntity.NotifyDate;
                        break;
                    case "ClosureMemo":
                        user = ProjectUsers.GetProjectUser(projectId, ProjectUserRoleCode.AssetActor);
                        var tmpEntity = AttachmentsMemoProcessInfo.Get(projectId,FlowCode.ClosureMemo);
                        if(tmpEntity!=null)
                            date = tmpEntity.NotifyDate;
                        break;
                    case "ReopenMemo":
                        user = ProjectUsers.GetProjectUser(projectId, ProjectUserRoleCode.AssetActor);
                        var entity = AttachmentsMemoProcessInfo.Get(projectId, FlowCode.ReopenMemo);
                        date = entity.NotifyDate;
                        break;
                }
                if (e.MemoCode == "GBMemo")
                {
                    var flowcode = e.FlowCode+"_"+e.MemoCode;
                    var task = TaskWork.Search(t => t.RefID == projectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == flowcode).FirstOrDefault();
                    if (task != null)
                    {
                        if ((task.Url.IndexOf("Approval", StringComparison.OrdinalIgnoreCase) != -1
                            || task.Url.IndexOf("Resubmit", StringComparison.OrdinalIgnoreCase) != -1
                            || task.Url.IndexOf("Notify", StringComparison.OrdinalIgnoreCase) != -1
                            || task.Url.IndexOf("View", StringComparison.OrdinalIgnoreCase) == -1)
                            && task.ReceiverAccount == ClientCookie.UserCode)
                        {
                            if (task.Url.IndexOf(task.RefID, StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                if (task.Url.IndexOf("?") >= 0)
                                {
                                    task.Url += "&";
                                }
                                else
                                {
                                    task.Url += "?";
                                }
                                task.Url += "projectId=" + task.RefID;
                            }
                            url = task.Url;
                        }
                        else
                            url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCode, e.MemoCode, projectId);
                    }
                    else
                    {
                        if (user.UserAccount == ClientCookie.UserCode && date==null)
                            url = string.Format("/{0}/Main#/{1}?projectId={2}", flowCode, e.MemoCode, projectId);
                        else
                            url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCode, e.MemoCode, projectId);
                    }
                }
                else
                {
                    if (user.UserAccount == ClientCookie.UserCode)
                        url = string.Format("/{0}/Main#/{1}?projectId={2}", flowCode, e.MemoCode, projectId);
                    else
                        url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCode, e.MemoCode, projectId);
                }
                

                return new
                {
                    NotifyDate = date,
                    Name = e.MemoNameENUS,
                    Url = url
                };
            }).ToList();
            return Ok(navs);
        }
    }
}
