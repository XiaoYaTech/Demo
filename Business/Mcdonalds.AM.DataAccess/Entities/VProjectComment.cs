using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class VProjectComment : BaseEntity<VProjectComment>
    {
        public string TitleName
        {
            get
            {
                return I18N.GetValue(this, "TitleName");
            }
        }

        public string PositionName
        {
            get
            {
                return I18N.GetValue(this, "Position");
            }
        }

        public static List<VProjectComment> SearchVList(ProjectCommentCondition condition)
        {
            var _db = PrepareDb();
            IQueryable<VProjectComment> list = _db.VProjectComment.Where(d => true);
            if (!string.IsNullOrEmpty(condition.SourceCode))
            {
                list = _db.VProjectComment
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
                switch (condition.Status) 
                {
                    case ProjectCommentStatus.Save:
                        list = list.Where(d => d.Status == 0);
                        break;
                    case ProjectCommentStatus.Submit:
                        list = list.Where(d => d.Status == 1);
                        break;
                }
            }

            List<VProjectComment> resultList = list.ToList();


            return resultList;

        }

        /// <summary>
        /// Add By Kevin.Yao
        /// 2014-10-22
        /// ClosurePackage-Package Attachment-For Print PDF
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<VProjectComment> SearchVListForPDF(ProjectCommentCondition condition)
        {
            var commentList = SearchVList(condition);

            //如果Package流程有Return/Recall操作，只取Return/Recall之后的意见
            var undoComment = commentList.Where(i => i.Action == ProjectCommentAction.Return || i.Action == ProjectCommentAction.Recall).OrderByDescending(i => i.CreateTime);
            if (undoComment.Count() > 0)
            {
                var lastReturnTime = undoComment.ToArray()[0].CreateTime;
                commentList = commentList.Where(i => i.CreateTime > lastReturnTime).ToList();
            }

            return commentList.Where(i => i.Action == ProjectCommentAction.Approve || i.Action == ProjectCommentAction.Submit || i.Action == ProjectCommentAction.ReSubmit).ToList();
        }

    }
}
