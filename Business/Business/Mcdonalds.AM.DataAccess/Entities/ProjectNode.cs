using Mcdonalds.AM.DataAccess.Constants;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   2/4/2015 1:36:54 PM
 * FileName     :   ProjectNode
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeCodeDef = Mcdonalds.AM.DataAccess.Constants.NodeCode;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectNode : BaseEntity<ProjectNode>
    {
        public static void GenerateOnCreate(string mainFlowCode, string projectId)
        {
            var flows = FlowInfo.Search(e => e.ParentCode == mainFlowCode).ToList();
            var projectNodes = new List<ProjectNode>();
            flows.ForEach(f =>
            {
                NodeInfo.Search(e => e.FlowCode == f.Code).ToList().ForEach(e =>
                {
                    projectNodes.Add(new ProjectNode
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        FlowCode = f.Code,
                        NodeCode = e.Code,
                        IsHistory = false,
                        Status = ProjectNodeStatus.UnFinish
                    });
                });
            });
            ProjectNode.Add(projectNodes.ToArray());
            if (mainFlowCode == Constants.FlowCode.Renewal)
            {
                var renewalNods = new List<ProjectNode>();
                renewalNods.Add(new ProjectNode
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FlowCode = "Renewal_GBMemo",
                    NodeCode = "Renewal_GBMemo_Input",
                    IsHistory = false,
                    Status = ProjectNodeStatus.UnFinish
                });
                renewalNods.Add(new ProjectNode
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FlowCode = "Renewal_GBMemo",
                    NodeCode = "Renewal_GBMemo_Send",
                    IsHistory = false,
                    Status = ProjectNodeStatus.UnFinish
                });
                ProjectNode.Add(renewalNods.ToArray());
            }
            if (mainFlowCode == Constants.FlowCode.MajorLease)
            {
                var majorleaseNods = new List<ProjectNode>();
                majorleaseNods.Add(new ProjectNode
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FlowCode = "MajorLease_GBMemo",
                    NodeCode = "MajorLease_GBMemo_Input",
                    IsHistory = false,
                    Status = ProjectNodeStatus.UnFinish
                });
                majorleaseNods.Add(new ProjectNode
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FlowCode = "MajorLease_GBMemo",
                    NodeCode = "MajorLease_GBMemo_Send",
                    IsHistory = false,
                    Status = ProjectNodeStatus.UnFinish
                });
                ProjectNode.Add(majorleaseNods.ToArray());
            }
        }

        public static ProjectNode FinishProjectNode(string projectId, string flowCode, string nodeCode)
        {
            var maxSeq = NodeInfo.Search(e => e.FlowCode == flowCode).Max(e => e.Sequence);
            var maxNodeCode = NodeInfo.FirstOrDefault(e => e.FlowCode == flowCode && e.Sequence == maxSeq).Code;
            if (nodeCode == NodeCodeDef.Finish)
            {
                nodeCode = maxNodeCode;
            }
            var projectNode = Search(e => e.ProjectId == projectId && e.FlowCode == flowCode && e.NodeCode == nodeCode && e.IsHistory == false).FirstOrDefault();
            ProjectNode nextNode = projectNode.GetNextNode(), prevNode = projectNode.GetPrevNode(), currentNode = projectNode;
            if (prevNode != null)
            {
                if (prevNode.Status == ProjectNodeStatus.Finish)
                {
                    projectNode.Status = ProjectNodeStatus.Finish;
                }
                else
                {
                    projectNode.Status = ProjectNodeStatus.Pending;
                    while (prevNode != null)
                    {
                        if (prevNode.Status == ProjectNodeStatus.Finish)
                        {
                            currentNode = prevNode;
                            break;
                        }
                        prevNode = prevNode.GetPrevNode();
                    }
                }
            }
            else
            {
                projectNode.Status = ProjectNodeStatus.Finish;
            }
            if (projectNode.Status == ProjectNodeStatus.Finish)
            {
                while (nextNode != null && nextNode.Status == ProjectNodeStatus.Pending)
                {
                    nextNode.Status = ProjectNodeStatus.Finish;
                    nextNode.Update();
                    currentNode = nextNode;
                    nextNode = nextNode.GetNextNode();
                }
            }
            projectNode.Update();
            if (currentNode.NodeCode == maxNodeCode)
            {
                currentNode = ProjectNode.FinishNode;
            }
            return currentNode;
        }
        public static ProjectNode UnFinishProjectNode(string projectId, string flowCode, string nodeCode)
        {
            if (nodeCode == NodeCodeDef.Start)
            {
                throw new ArgumentException("Can't cancel Start Node", "nodeCode");
            }
            ProjectNode pNode = FirstOrDefault(e=>e.ProjectId == projectId && e.FlowCode == flowCode && e.NodeCode == nodeCode);
            pNode.Status = ProjectNodeStatus.UnFinish;
            pNode.Update();
            ProjectNode nextNode = pNode.GetNextNode(), prevNode = pNode.GetPrevNode();
            while (nextNode != null && nextNode.Status == ProjectNodeStatus.Finish)
            {
                nextNode.Status = ProjectNodeStatus.Pending;
                nextNode.Update();
                nextNode = pNode.GetNextNode();
            }
            return prevNode != null ? prevNode : StartNode;
        }

        public ProjectNode GetNextNode()
        {
            var db = GetDb();
            return (from n in db.NodeInfo
                    from nextN in db.NodeInfo
                    from nextPn in db.ProjectNode
                    where nextPn.NodeCode == nextN.Code
                        && n.Code == this.NodeCode && nextN.Sequence == n.Sequence + 1
                        && nextN.FlowCode == n.FlowCode && nextPn.ProjectId == ProjectId
                    select nextPn).FirstOrDefault();
        }

        public ProjectNode GetPrevNode()
        {
            var db = GetDb();
            return (from n in db.NodeInfo
                    from prevN in db.NodeInfo
                    from prevPn in db.ProjectNode
                    where prevPn.NodeCode == prevN.Code
                        && n.Code == this.NodeCode && prevN.Sequence == n.Sequence - 1
                        && prevN.FlowCode == n.FlowCode && prevPn.ProjectId == ProjectId
                    select prevPn).FirstOrDefault();
        }
        public static ProjectNode StartNode
        {
            get
            {
                return new ProjectNode
                {
                    NodeCode = NodeCodeDef.Start
                };
            }
        }

        public static ProjectNode FinishNode
        {
            get
            {
                return new ProjectNode
                {
                    NodeCode = NodeCodeDef.Finish
                };
            }
        }
    }
}
