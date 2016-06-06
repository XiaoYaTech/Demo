using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class ReplyToArticle
    {
        public string ReplyID { get; set; }
        public string ArticleID { get; set; }
        public string ReplyBody { get; set; }
        public bool IsValidated { get; set; }
        public string UpdatedByUserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
