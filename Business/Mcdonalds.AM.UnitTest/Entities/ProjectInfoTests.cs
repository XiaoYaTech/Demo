using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Tests
{
    [TestClass()]
    public class ProjectInfoTests
    {
        [TestMethod()]
        public void UpdateProjectStatusTest()
        {
            ProjectInfo.UpdateProjectStatus("MajorLease14090106", FlowCode.MajorLease_LegalReview, ProjectStatus.UnFinish);
        }
    }
}
