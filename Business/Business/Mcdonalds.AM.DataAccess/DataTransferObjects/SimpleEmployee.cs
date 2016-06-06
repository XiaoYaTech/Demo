using Mcdonalds.AM.DataAccess.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/26/2014 10:11:54 AM
 * FileName     :   SimpleEmployee
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class SimpleEmployee
    {
        public string Code { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public string PositionCode { get; set; }
        public string PositionZHCN { get; set; }
        public string PositionENUS { get; set; }
        public string TitleCode { get; set; }
        public string TitleZHCN { get; set; }
        public string TitleENUS { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

        public string Name
        {
            get
            {
                return I18N.GetValue(this, "Name");
            }
        }

        public bool Equals(SimpleEmployee other)
        {
            return Code.Equals(other.Code, StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is SimpleEmployee) return Equals((SimpleEmployee)obj);
            return base.Equals(obj);
        }
    }
}
