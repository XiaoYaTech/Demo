using System.Collections.Generic;
using NTTMNC.BPM.Fx.K2.Services.Entity;

namespace Mcdonalds.AM.DataAccess.Interfaces
{
    interface IWorkflowExtend
    {
        /// <summary>
        /// Todo:检查Project是否处于冻结状态
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        bool CheckIfFreezeProject(string projectId);

        /// <summary>
        /// Todo:中止Project
        /// </summary>
        /// <param name="projectId"></param>
        void PendingProject(string projectId);

        /// <summary>
        /// Todo:恢复Project
        /// </summary>
        /// <param name="projectId"></param>
        void ResumeProject(string projectId);

        /// <summary>
        /// Todo:变更Project Team成员
        /// </summary>
        //void ChangeProjectTeamMember(string projectId);

        //void UpdateWorkflowDataField(int procInstId);

        void RedirectWorkflowTask(string sn, string taskUserAccount, string redirectUserAccount, List<ProcessDataField> dataFields = null);
    }
}
