using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class NotificationSearchCondition
    {

        public string Title { get; set; }
        public string FlowCode { get; set; }
        public string SenderZHCN { get; set; }
        public string SenderENUS { get; set; }
        public string SenderCode { get; set; }
        public string ReceiverAccount { get; set; }
        public string ProjectId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageIndex { get; set; }
        public string RefTableName { get; set; }
        public int PageSize { get; set; }
    }

    public class NotificationDTO
    {
        public System.Guid ?RefId { get; set; }
        public string ProjectId { get; set; }
        public string SenderCode { get; set; }
        public string ReceiverCode { get; set; }
        public System.DateTime? CreateTime { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public string CreateUserAccount { get; set; }
        public bool IsSendEmail { get; set; }
        public bool HasRead { get; set; }
        public string FlowCode { get; set; }
        public Nullable<System.Guid> ContentId { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public string PositionENUS { get; set; }
        public string PositionZHCN { get; set; }
        public string SenderZHCN { get; set; }
        public string SenderENUS { get; set; }
        public string ReceiverENUS { get; set; }
        public string ReceiverZHCN { get; set; }
    }
}
