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
    
    public partial class FlowInfo
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string CreateUserAccount { get; set; }
        public string ParentCode { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public string FlowCodePrefix { get; set; }
        public string RoleCode { get; set; }
        public string TableName { get; set; }
        public bool Recallable { get; set; }
        public bool Editable { get; set; }
        public bool NoTaskEditable { get; set; }
        public int LayoutSequence { get; set; }
        public int ExecuteSequence { get; set; }
        public string Percentage { get; set; }
    }
}
