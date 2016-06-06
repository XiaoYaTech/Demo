using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class MajorLeaseConsInvtCheckingTests
    {
        [TestMethod()]
        public void GetApprovalTypeTest()
        {
            var conInvstChecking = MajorLeaseConsInvtChecking.Get(new Guid("42F97D24-C397-4FF6-9939-9913A4BF6625"));
            conInvstChecking.GetApprovalType();
        }
    }
}
