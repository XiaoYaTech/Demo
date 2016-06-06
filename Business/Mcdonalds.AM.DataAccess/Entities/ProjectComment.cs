using System.CodeDom;
using System.Data.Entity;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using System;
using System.Collections.Generic;
using System.Linq;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Constants;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectComment : BaseEntity<ProjectComment>
    {
        public string TitleName
        {
            get
            {
                return I18N.GetValue(this, "TitleName");
            }
        }

        public static void AddComment(string action, string content, Guid refTableId, string refTableName, string sourceCode, int? procInstID = null, ProjectCommentStatus? status = null)
        {
            var source = FlowInfo.Get(sourceCode);
            var comment = new ProjectComment
            {
                Action = action,
                Content = content,
                CreateTime = DateTime.Now,
                CreateUserAccount = ClientCookie.UserCode,
                CreateUserNameENUS = ClientCookie.UserNameENUS,
                CreateUserNameZHCN = ClientCookie.UserNameZHCN,
                Id = Guid.NewGuid(),
                ProcInstID = procInstID,
                RefTableId = refTableId,
                RefTableName = refTableName,
                SourceCode = source.Code,
                SourceNameENUS = source.NameENUS,
                SourceNameZHCN = source.NameZHCN,
                Status = status,
                TitleNameENUS = ClientCookie.TitleENUS,
                TitleNameZHCN = ClientCookie.TitleENUS,
                UserAccount = ClientCookie.UserCode,
                UserNameENUS = ClientCookie.UserNameENUS,
                UserNameZHCN = ClientCookie.UserNameZHCN
            };
            comment.Add();
        }

        public void ParseActionDescription()
        {

            switch (Action)
            {
                case ProjectCommentAction.Decline:
                    ActionDesc = "Reject";
                    break;
                default:
                    ActionDesc = Action;
                    break;
            }

        }

        public void ParseCommentStatus()
        {
            switch (Action)
            {
                case "Confirm":
                    Status = ProjectCommentStatus.Save;
                    break;
            }
        }

        public override int Add()
        {
            ParseActionDescription();
            ParseCommentStatus();
            return base.Add();
        }

        public override int Update()
        {
            ParseActionDescription();
            ParseCommentStatus();
            return base.Update();
        }

        public static ProjectComment GetSavedComment(Guid refTableId, string refTableName, string creator)
        {
            return FirstOrDefault(pc => pc.RefTableId == refTableId && pc.UserAccount == creator && pc.RefTableName == refTableName && pc.Status == ProjectCommentStatus.Save);
        }

        public static List<ProjectComment> GetList(string refTableName, Guid refTableId, ProjectCommentStatus? status = null)
        {
            return Search(pc => pc.RefTableName == refTableName && pc.RefTableId == refTableId && (status == null || pc.Status == status)).OrderByDescending(pc => pc.Num).ToList();
        }

        public static List<ProjectComment> SearchList(ProjectCommentCondition condition)
        {
            var _db = PrepareDb();
            IQueryable<ProjectComment> list = _db.ProjectComment.Where(d => true);
            if (!string.IsNullOrEmpty(condition.SourceCode))
            {
                list = _db.ProjectComment
              .Where(d => d.SourceCode == condition.SourceCode);
            }
            if (condition.RefTableId != new Guid())
            {
                list = list.Where(d => d.RefTableId == condition.RefTableId);
            }

            if (!string.IsNullOrEmpty(condition.Action))
            {
                list = list.Where(d => d.Action == condition.Action);
            }
            if (!string.IsNullOrEmpty(condition.UserAccount))
            {
                list = list.Where(d => d.UserAccount == condition.UserAccount);
            }

            if (!string.IsNullOrEmpty(condition.Content))
            {
                list = list.Where(d => d.Content.Contains(condition.Content));
            }
            if (condition.Status != ProjectCommentStatus.None)
            {
                list = list.Where(d => d.Status == condition.Status);
            }

            List<ProjectComment> resultList = list.ToList();


            return resultList;

        }


    }
}
