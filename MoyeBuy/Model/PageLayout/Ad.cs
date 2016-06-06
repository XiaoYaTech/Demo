using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class Ad
    {
        public int AdID { get; set; }
        public string AdTitle { get; set; }
        public string AdType { get; set; }
        public IList<string> AdImgs { get; set; }
        public IList<string> AdImgAltTitle { get; set; }
        public IList<string> AdImgDisq { get; set; }
        public IList<string> AdImigDesc { get; set; }
        public IList<string> AdUrl { get; set; }
        public IList<string> AdTarget { get; set; }
        public IList<string> AdClassName { get; set; }
        public IList<string> AdControlID { get; set; }
    }
}
