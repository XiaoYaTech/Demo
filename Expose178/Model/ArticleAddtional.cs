using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class ArticleAddtional
    {
        public string AdditionalID { get; set; }
        public string ArticleID { get; set; }
        public int ReadNum { get; set; }
        public int ReplyNum { get; set; }
        public string UpdatedByUserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
