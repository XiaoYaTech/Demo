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
    
    public partial class SysRole
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public int Type { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<int> IsInternal { get; set; }
        public Nullable<int> IsEffective { get; set; }
        public bool UseRange { get; set; }
    }
}
