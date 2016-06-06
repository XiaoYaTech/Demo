using Mcdonalds.AM.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Mcdonalds.AM.ScheduleService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.WriteLog("===================================================");
            Log.WriteLog("Start");

            var today = DateTime.Now;

            var logs = ScheduleLog.Search(i => i.IsExecuted == false).ToArray();
            foreach (var log in logs)
            {
                if (CanOperate(log, today))
                {
                    ExecuteAction(log);
                }
            }
            ScheduleLog.Update(logs);

            Log.WriteLog("End");
            Log.WriteLog("===================================================");

            this.Dispose();
        }

        /// <summary>
        /// 判断是否到了更新Store状态的时间
        /// </summary>
        /// <param name="info"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool CanOperate(ScheduleLog info, DateTime today)
        {
            if (info.ExecuteDate.HasValue)
            {
                //正常处理
                if (info.ExecuteDate.Value.ToString("yyyy-MM-dd") == today.ToString("yyyy-MM-dd"))
                    return true;
                //因异常未处理的数据
                else if (info.ExecuteDate < today)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 执行定时操作
        /// </summary>
        /// <param name="info"></param>
        private void ExecuteAction(ScheduleLog info)
        {
            switch (info.Action)
            {
                case ScheduleAction.Update:
                    UpdateStore(info);
                    break;
                case ScheduleAction.Generate:
                    GenerateTask(info);
                    break;
            }
        }

        /// <summary>
        /// 更新店面状态
        /// </summary>
        /// <param name="info"></param>
        private void UpdateStore(ScheduleLog info)
        {
            var storeInfo = StoreBasicInfo.GetStorInfo(info.USCode);
            if (storeInfo == null)
                return;

            try
            {
                if (info.ProjectId.ToLower().Contains("closure"))
                {
                    storeInfo.StoreStatus = "suoya301003";
                    storeInfo.statusName = "Closed";
                    storeInfo.CloseDate = info.ExecuteDate;
                    storeInfo.Update();
                }
                else if (info.ProjectId.ToLower().Contains("tpcls"))
                {
                    storeInfo.StoreStatus = "suoya301005";
                    storeInfo.statusName = "TempClosed";
                    //storeInfo.CloseDate = info.UpdateDate;
                    storeInfo.Update();
                }
                else if (info.ProjectId.ToLower().Contains("reimage"))
                {
                    storeInfo.StoreStatus = "suoya301001";
                    storeInfo.statusName = "Reimaging";
                    storeInfo.ReImageDate = info.ExecuteDate;
                    storeInfo.Update();
                }
                else if (info.ProjectId.ToLower().Contains("renewal"))
                {
                    storeInfo.StoreStatus = "suoya301002";
                    storeInfo.statusName = "Renewal";
                    storeInfo.Update();
                }
                info.IsExecuted = true;
                Log.WriteLog(storeInfo.NameZHCN + "(" + info.USCode + ")状态更新成功");
            }
            catch (Exception ex)
            {
                Log.WriteErrorMessage(ex);
                Log.WriteLog(storeInfo.NameZHCN + "(" + info.USCode + ")状态更新失败");
            }
        }


        /// <summary>
        /// 生成任务
        /// </summary>
        /// <param name="info"></param>
        private void GenerateTask(ScheduleLog info)
        {
            if (!string.IsNullOrEmpty(info.Info))
            {
                try
                {
                    var task = JsonConvert.DeserializeObject<TaskWork>(info.Info);
                    task.Add();
                    info.IsExecuted = true;
                    Log.WriteLog(info.ProjectId + "创建" + info.FlowCode + "任务成功");
                }
                catch (Exception ex)
                {
                    Log.WriteErrorMessage(ex);
                    Log.WriteLog(info.ProjectId + "创建" + info.FlowCode + "任务失败");
                }
            }
        }
    }
}
