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
    
    public partial class RenewalLLNegotiationRecord
    {
        public System.Guid Id { get; set; }
        public System.Guid RenewalLLNegotiationId { get; set; }
        public string McdParticipants { get; set; }
        public string Content { get; set; }
        public string LLParticipants { get; set; }
        public string Topic { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CreateUserAccount { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string LastUpdateUserAccount { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public bool Valid { get; set; }
    }
}
