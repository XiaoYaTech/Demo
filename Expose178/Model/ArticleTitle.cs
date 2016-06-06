using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class ArticleTitle
    {
        public string ArticleID { get; set; }
        public string ArticleTile { get; set; }
        public string ArticleSummary { get; set; }
        public DateTime ArticleDate { get; set; }
        public int ReadNum { get; set; }
        public int ReplyNum { get; set; }
    }
}
