using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class CommentCatgory
    {
        public string CommentCatgoryID { get; set; }
        public string CommentCatgoryName { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
