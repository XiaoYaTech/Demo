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
    
    public partial class StoreAssetHandoverAttachment
    {
        public System.Guid Id { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string StoreCode { get; set; }
        public Nullable<int> SequenceNum { get; set; }
        public string AttachmentName { get; set; }
        public Nullable<int> Stauts { get; set; }
        public string Attachment { get; set; }
        public string DOCName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public string CreatePerson { get; set; }
        public Nullable<int> AttachmentType { get; set; }
    }
}
