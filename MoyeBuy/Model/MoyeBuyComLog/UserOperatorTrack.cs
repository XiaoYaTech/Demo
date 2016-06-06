using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class UserOperatorTrack
    {
        public string UserOperatorTrackID { get; set; }
        public string ParentUserOperatorTrackID { get; set; }
        public string UserID { get; set; }
        public string FromURL { get; set; }
        public string PageName { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public UserOperatorTrack Track { get; set; }
    }
}
