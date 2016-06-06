using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class CommentInfo
    {
        public string CommentID { get; set; }
        public string UserID { get; set; }
        public string CommentCatgoryID{get;set;}//评论 或者 咨询
        public string CommentDesc { get; set; }
        public string CommentState { get; set; }//是否通过审核
        public string CommentAttitude { get; set; }//好中差评
        public Nullable<bool> IsAgree { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public CommentInfo ParentComment { get; set; }
    }
}
