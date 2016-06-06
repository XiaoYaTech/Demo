using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class NotificationTests
    {
        [TestMethod()]
        public void SendTest()
        {
            //var notificationMsg = new NotificationMsg()
            //{
            //    FlowCode = FlowCode.Reimage_ConsInfo,
            //    ProjectId = "Reimage14111201",
            //    SenderCode = "E5004654",
            //    RefId = Guid.NewGuid(),
            //    IsSendEmail = false
            //};

            var notificationMsg = new NotificationMsg()
            {
                FlowCode = FlowCode.Closure_LegalReview,
                ProjectId = "Closure14111103",
                SenderCode = "E5016672",
                RefId = Guid.NewGuid(),
                IsSendEmail = false
            };

            notificationMsg.ReceiverCodeList = new List<string>() { "E5001301" };

            Notification.Send(notificationMsg);
        }
    }
}
