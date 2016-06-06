using System;
using System.Collections.Generic;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class NotificationMsg
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string FlowCode { get; set; }
        public string ProjectId { get; set; }
        public string SenderCode { get; set; }
        public bool IsSendEmail { get; set; }
        public List<string> ReceiverCodeList { get; set; }
        public string UsCode { get; set; }
        public Guid RefId { get; set; }
    }
}
