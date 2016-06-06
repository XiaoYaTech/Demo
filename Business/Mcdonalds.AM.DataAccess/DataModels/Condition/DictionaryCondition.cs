using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataModels.Condition
{
    public class DictionaryCondition 
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string Ids { get; set; }

        public int Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public bool IsDirectory { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateUserAccount { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public int Sequence { get; set; }
        public string ExtendField0 { get; set; }
        public string ExtendField1 { get; set; }
        public string ExtendField2 { get; set; }
        public string ExtendField3 { get; set; }
        public string ExtendField4 { get; set; }
        public string ExtendField5 { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
    }
}
