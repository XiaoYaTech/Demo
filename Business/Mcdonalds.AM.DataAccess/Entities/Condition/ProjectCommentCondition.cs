using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Entities.Condition
{
    public class ProjectCommentCondition
    {
        public string SourceCode { get; set; }
        public string SourceNameENUS { get; set; }
        public string SourceNameZHCN { get; set; }
        public string RefTableName { get; set; }
        public Guid RefTableId { get; set; }
        public string UserAccount { get; set; }
        public string UserNameENUS { get; set; }
        public string UserNameZHCN { get; set; }
        public string TitleNameZHCN { get; set; }
        public string TitleNameENUS { get; set; }
        public string TitleCode { get; set; }
        public string Content { get; set; }
        public string Action { get; set; }
        private ProjectCommentStatus _status = ProjectCommentStatus.None;
        public ProjectCommentStatus Status
        {
            get { return _status; }
            set { _status = value; }

        }
    }
}
