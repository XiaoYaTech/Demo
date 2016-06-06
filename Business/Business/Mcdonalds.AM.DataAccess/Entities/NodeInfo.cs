using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Constants;

namespace Mcdonalds.AM.DataAccess
{
    public partial class NodeInfo : BaseEntity<NodeInfo>
    {
        //public string CurrentNodeCode { get; set; }

        public class NodeInfoEntity
        {
            public System.Guid Id { get; set; }
            public string Code { get; set; }
            public Nullable<double> ProgressRate { get; set; }
            public Nullable<System.DateTime> CreateTime { get; set; }
            public string CreateUserAccount { get; set; }
            public string FlowCode { get; set; }
            public string NameZHCN { get; set; }
            public string NameENUS { get; set; }
            public Nullable<NodeType> Type { get; set; }
            public Nullable<int> Sequence { get; set; }
            /// <summary>
            /// 当前节点Code
            /// </summary>
            public string CurrentNodeCode { get; set; }

            /// <summary>
            /// 当前节点排序号
            /// </summary>
            public int CurrentNodeSequence { get; set; }
        }

        public static NodeInfo GetCurrentNode(string projectId, string flowCode)
        {
            string sql = string.Format(@"SELECT tb_node.* 
            FROM dbo.ProjectInfo tb_project
			Inner JOIN dbo.NodeInfo tb_node
			ON (tb_project.FlowCode = tb_node.FlowCode OR TB_node.Type =2)
	        AND tb_project.NodeCode = tb_node.Code		
			WHERE  ProjectId  ='{0}'
			AND tb_project.FlowCode = '{1}'", projectId, flowCode);

            var datas = SqlQuery<NodeInfo>(sql, null);
            var nodeInfo = datas.AsNoTracking().FirstOrDefault();
            return nodeInfo;
        }

        public static NodeInfo GetNodeInfo(string nodeCode)
        {
            return FirstOrDefault(e => e.Code == nodeCode);
        }

        /// <summary>
        /// 查询检查点
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="flowCode"></param>
        /// <returns></returns>
        public List<CheckPoint> QueryCheckPoints(string projectId, string flowCode)
        {
            string sql = @"SELECT pn.NodeCode,n.NameENUS,n.NameZHCN,pn.Status,n.Sequence
                FROM dbo.ProjectNode pn INNER JOIN dbo.NodeInfo n
                ON n.Code = pn.NodeCode
                WHERE pn.ProjectId = @ProjectId AND pn.FlowCode = @FlowCode
                ORDER BY n.Sequence";

            var datas = SqlQuery<CheckPoint>(sql, new
            {
                ProjectId = projectId,
                FlowCode = flowCode
            });
            var list = datas.ToList();
            return list;
        }
    }
}
