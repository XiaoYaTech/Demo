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
    public class StoreContractInfoTests
    {
        [TestMethod()]
        public void MappingProjectContractRevisionTest()
        {
            var revision = StoreContractInfo.MappingProjectContractRevision("1960005");

        }
    }
}
