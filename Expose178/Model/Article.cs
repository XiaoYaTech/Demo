using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class Article
    {
        public string ArticleID { get;set;}
        public string UID { get; set; }
        public string AritcleTypeCode { get; set; }
        public string ReadRoleTypeCode { get; set; }
        public string ArticleTile { get; set; }
        public string ArticleBody { get; set; }
        public DateTime ArticleDate { get; set; }
        public string BackgroundImgUrl { get; set; }
        public bool IsValidated { get; set; }
        public bool IsDraft { get; set; }
        public AritcleType AritcleType { get; set; }
        public ReadRoleType ReadRoleType { get; set; }
        public User User { get; set; }
        public IList<ReplyToArticle> ListReply { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
