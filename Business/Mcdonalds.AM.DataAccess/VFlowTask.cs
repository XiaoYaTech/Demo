//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mcdonalds.AM.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class VFlowTask
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string NameENUS { get; set; }
        public string NameZHCN { get; set; }
        public Nullable<int> Status { get; set; }
        public string RefID { get; set; }
        public string ReceiverAccount { get; set; }
        public string ReceiverNameENUS { get; set; }
        public string ReceiverNameZHCN { get; set; }
        public string RoleCode { get; set; }
    }
}
