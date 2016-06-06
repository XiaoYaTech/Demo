/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/9/2014 2:25:06 PM
 * FileName     :   WFTempClosureLegalReview
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess.Workflow.TempClosure
{
    public class WFTempClosureLegalReview:BaseWorkflow<TempClosureLegalReview>
    {
        public WFTempClosureLegalReview(TempClosureLegalReview entity)
            : base(entity)
        {
            var legal = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == entity.ProjectId && pu.RoleCode == ProjectUserRoleCode.Legal);
            var task = TaskWork.GetTaskWork(entity.ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.TempClosure, FlowCode.TempClosure_LegalReview);
            K2Param.Task = task;
            K2Param.AddDataField("dest_Creator",Entity.CreateUserAccount);
            K2Param.AddDataField("dest_Legal", legal.UserAccount);
            K2Param.AddDataField("ProcessCode",ProcessCode);
            K2Param.AddDataField("ProjectTaskInfo",JsonConvert.SerializeObject(task));
        }

        public override string ProcessName
        {
            get { return @"MCDAMK2Project\TempClosureLegalReview"; }
        }

        public override string ProcessCode
        {
            get { return @"MCD_AM_TempClosure_LR"; }
        }

        public override string TableName
        {
            get { return "TempClosureLegalReview"; }
        }

        public override string Act_Originator
        {
            get { return "Originator"; }
        }

        public override string ProjectFlowCode
        {
            get { return FlowCode.TempClosure_LegalReview; }
        }

        public override string[] NormalActors
        {
            get { return new string[] { }; }
        }
    }
}
