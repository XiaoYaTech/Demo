using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectStatusChangeLog : BaseEntity<ProjectStatusChangeLog>
    {
        public string PrevStatusName
        {
            get
            {
                var result = default(string);
                switch (PrevStatus)
                {
                    case ProjectStatus.UnFinish:
                        result = "Active";
                        break;
                    default:
                        result = PrevStatus.ToString();
                        break;
                }
                return result;
            }
        }

        public string CurrStatusName
        {
            get
            {
                var result = default(string);
                switch (CurrStatus)
                {
                    case ProjectStatus.UnFinish:
                        result = "Active";
                        break;
                    default:
                        result = CurrStatus.ToString();
                        break;
                }
                return result;
            }
        }

        public string CreatorNameENUS
        {
            get
            {
                var emp = Employee.GetEmployeeByCode(CreateBy);
                if (emp != null)
                    return emp.NameENUS;
                return "";
            }
        }
    }
}
