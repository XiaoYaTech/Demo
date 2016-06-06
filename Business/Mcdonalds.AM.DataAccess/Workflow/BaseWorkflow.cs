/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/28/2014 11:14:35 AM
 * FileName     :   BaseWorkflow
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using Mcdonalds.AM.DataAccess.Constants;
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

namespace Mcdonalds.AM.DataAccess.Workflow
{
    public abstract class BaseWorkflow<T> where T : BaseAbstractEntity, IWorkflowEntity, new()
    {

        #region ---- [Workflow Config Varables ] ----

        public abstract string ProcessName { get; }
        public abstract string ProcessCode { get; }
        public abstract string TableName { get; }
        public abstract string Act_Originator { get; } // 退回至发起人节点名

        public abstract string ProjectFlowCode { get; }

        // 一般审批人节点，不能取消流程
        public abstract string[] NormalActors { get; }

        #endregion
        public T Entity { get; private set; }
        public K2Parameter K2Param { get; protected set; }
        public BaseWorkflow(T entity)
        {
            K2Param = new K2Parameter();
            Entity = entity;
        }

        public void ExecuteProcess(string actionName)
        {
            K2FxContext.Current.ApprovalProcess(K2Param.SerialNumber, K2Param.EmployeeCode, actionName, K2Param.Comment);
        }

        protected int StartProcess()
        {
            return K2FxContext.Current.StartProcess(ProcessCode, ClientCookie.UserCode, K2Param.DataFields);
        }

        public void Submit()
        {
            Entity.ProcInstId = StartProcess();
        }
        public void Approve()
        {
            ExecuteProcess("Approve");
        }

        public void Reject()
        {
            ExecuteProcess("Reject");
        }

        public virtual void Return()
        {
            ExecuteProcess("Return");
        }

        public void Resubmit()
        {
            ExecuteProcess("Resubmit");
        }

        public void ReCall()
        {
            string comments = ClientCookie.UserNameZHCN + "进行了流程撤回操作";
            K2FxContext.Current.GoToActivityAndRecord(
                Entity.ProcInstId.Value,
                Act_Originator,
                ClientCookie.UserCode,
                ProjectAction.Recall,
                comments
            );
            ProjectInfo.Reset(Entity.ProjectId, ProjectFlowCode);
        }
    }
}
