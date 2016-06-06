/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/23/2014 3:11:24 PM
 * FileName     :   ProcedureManager
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public class ProcedureManager
    {
        public static List<Proc_GetProjectProcess_Result> Proc_GetProjectProcess(string projectId)
        {
            var db = new McdAMEntities();
            var result = db.Proc_GetProjectProcess(projectId).ToList();
            return result;
        }

        public static Proc_PrepareClosureMemo_Result Proc_PrepareClosureMemo(string projectId)
        {
            
            var db = new McdAMEntities();
            var result = db.Proc_PrepareClosureMemo(projectId).FirstOrDefault();
            
            return result;
        }

        public static List<Proc_ProjectHistory_Result> Proc_GetProjectHistory(string projectId, string tableName,bool hasTemplate)
        {
            var db = new McdAMEntities();
            var result = db.Proc_GetProjectHistory(projectId, tableName, hasTemplate).ToList();

            return result;
        }

    }
}
