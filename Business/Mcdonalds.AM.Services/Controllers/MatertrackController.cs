using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    /// <summary>
    /// 事项追踪API
    /// Author: Stephen.Wang
    /// Date:2014-07-07
    /// </summary>
    public class MatertrackController : ApiController
    {
        [Route("api/matertrack")]
        [HttpGet]
        public IHttpActionResult GetMaterTrackReps(string workflowId, Guid nodeId)
        {
            var tracks = MaterTrack.Search(m => m.WorkflowId == workflowId && m.NodeId == nodeId).ToList();
            var reps = tracks.Where(t => t.TrackType == MaterTrackType.Rep).OrderBy(t => t.CreateTime).Select(t =>
            {
                var rep = new MaterTrackRep();
                rep.Id = t.Id;
                rep.Content = t.Content;
                rep.CreateTime = t.CreateTime;
                rep.Creator = t.Creator;
                rep.CreatorZHCN = t.CreatorZHCN;
                rep.CreatorENUS = t.CreatorENUS;
                rep.TrackType = (MaterTrackType)t.TrackType;
                if (tracks.Count(t2 => t2.TrackType == MaterTrackType.Rep && t2.CreateTime > t.CreateTime) > 0)
                    rep.IsFinish = true;
                else
                {
                    if (TaskWork.Count(t2 => t2.RefID == workflowId && t2.Status == TaskWorkStatus.UnFinish &&
                        t2.ActivityName.Contains("Start_MaterTrack")) > 0)
                        rep.IsFinish = false;
                    else
                        rep.IsFinish = true;
                }
                rep.Replies = tracks.Where(f => f.ParentId == t.Id).OrderBy(f => f.CreateTime).Select(f =>
                {
                    var reply = new MaterTrackReply();
                    reply.Id = f.Id;
                    reply.Content = f.Content;
                    reply.CreateTime = f.CreateTime;
                    reply.Creator = f.Creator;
                    reply.CreatorZHCN = f.CreatorZHCN;
                    reply.CreatorENUS = f.CreatorENUS;
                    reply.TrackType = (MaterTrackType)f.TrackType;
                    return reply;
                }).ToList();
                return rep;
            }).ToList();
            return Ok(new
            {
                Reps = reps,
                IsLegal = ProjectUsers.IsRole(workflowId, ClientCookie.UserCode, ProjectUserRoleCode.Legal),
                IsAssetActor = ProjectUsers.IsRole(workflowId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor)
            });
        }

        [Route("api/matertrack/add")]
        [HttpPost]
        public bool Add(MaterTrack materTrack)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                materTrack.Id = Guid.NewGuid();
                materTrack.CreateTime = DateTime.Now;
                MaterTrack.Add(materTrack);
                var project = ProjectInfo.Get(materTrack.WorkflowId, materTrack.WorkflowType);
                var store = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == project.USCode);
                var Codes = materTrack.NodeType.Split('_');
                var url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", Codes[0], Codes[1], materTrack.WorkflowId);
                //if (Codes[0] == "Closure") //临时改动 只针对Closure,因为Closure 用的还是老的URL
                //{
                //    url = string.Format("/{0}/Main#/{0}/{1}/Process/View?projectId={2}", Codes[0], Codes[1], materTrack.WorkflowId);
                //}
                var title = TaskWork.BuildTitle(materTrack.WorkflowId, store.NameZHCN, store.NameENUS);
                switch (materTrack.TrackType)
                {
                    case MaterTrackType.Rep:
                        {
                            var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == materTrack.WorkflowId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                            TaskWork.SendTask(materTrack.WorkflowId, title, project.USCode, url, actor, materTrack.WorkflowType, materTrack.NodeType, "Start_MaterTrack_Rep");
                        }
                        break;
                    case MaterTrackType.Feedback:
                        {
                            var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == materTrack.WorkflowId && pu.RoleCode == ProjectUserRoleCode.Legal);
                            TaskWork.Finish(t => t.RefID == materTrack.WorkflowId && t.SourceCode == materTrack.WorkflowType && t.TypeCode == materTrack.NodeType && t.ActivityName == "Start_MaterTrack_Rep" && t.Status == TaskWorkStatus.UnFinish);
                            if (TaskWork.Count(t => t.RefID == materTrack.WorkflowId && t.SourceCode == materTrack.WorkflowType && t.TypeCode == materTrack.NodeType && t.ActivityName == "Start_MaterTrack_Feedback" && t.Status == TaskWorkStatus.UnFinish) == 0)
                                TaskWork.SendTask(materTrack.WorkflowId, title, project.USCode, url, legal, materTrack.WorkflowType, materTrack.NodeType, "Start_MaterTrack_Feedback");
                        }
                        break;
                    case MaterTrackType.Reply:
                        {
                            var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == materTrack.WorkflowId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                            TaskWork.Finish(t => t.RefID == materTrack.WorkflowId && t.SourceCode == materTrack.WorkflowType && t.TypeCode == materTrack.NodeType && t.ActivityName == "Start_MaterTrack_Feedback" && t.Status == TaskWorkStatus.UnFinish);
                            if (TaskWork.Count(t => t.RefID == materTrack.WorkflowId && t.SourceCode == materTrack.WorkflowType && t.TypeCode == materTrack.NodeType && t.ActivityName == "Start_MaterTrack_Rep" && t.Status == TaskWorkStatus.UnFinish) == 0)
                                TaskWork.SendTask(materTrack.WorkflowId, title, project.USCode, url, actor, materTrack.WorkflowType, materTrack.NodeType, "Start_MaterTrack_Rep");
                        }
                        break;
                }
                tranScope.Complete();
                return true;
            }
        }

        [Route("api/matertrack/finish")]
        [HttpGet]
        public bool Finish(string WorkFlowId, string WorkFlowType, string NodeType)
        {
            TaskWork.Finish(t => t.RefID == WorkFlowId && t.SourceCode == WorkFlowType && t.TypeCode == NodeType
                && t.ActivityName.Contains("Start_MaterTrack") && t.Status == TaskWorkStatus.UnFinish);
            return true;
        }


    }
}
