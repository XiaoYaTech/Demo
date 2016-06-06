using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostSimpleWorkflowData
    {
        public string ProjectId { get; set; }
        public string SN { get; set; }
        public string ProjectComment { get; set; }
        public string FlowCode { get; set; }
    }
}
