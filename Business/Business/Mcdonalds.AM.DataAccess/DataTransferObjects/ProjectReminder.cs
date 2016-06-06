/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/12/2014 4:31:21 PM
 * FileName     :   ProjectReminder
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
    public class ProjectReminder
    {
        public int TaskCount { get; set; }
        public List<TaskWork> Tasks { get; set; }

        public int RemindCount { get; set; }
        public List<Remind> Reminds { get; set; }
    }
}
