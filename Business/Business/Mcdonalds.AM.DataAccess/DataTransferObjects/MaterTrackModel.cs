using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class MaterTrackRep : MaterTrackReply
    {
        public List<MaterTrackReply> Replies { get; set; }
    }

    public class MaterTrackReply
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsFinish { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid Creator { get; set; }
        public string CreatorZHCN { get; set; }
        public string CreatorENUS { get; set; }
        public MaterTrackType TrackType { get; set; }
    }


}