using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mcdonalds.AM.DataAccess.Common.Extensions.Tests
{
    [TestClass()]
    public class StringExtensionTests
    {
        [TestMethod()]
        public void AsTest()
        {
            //var tmp=StringExtension.As<decimal>("223");
            var fileInfo = new FileInfo("www");

        }

        [TestMethod()]
        public void AsStringTest()
        {
            object ds = null;
        }
    }
}
