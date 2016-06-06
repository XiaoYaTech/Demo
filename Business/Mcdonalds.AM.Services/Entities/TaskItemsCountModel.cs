using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Entities
{
    /// <summary>
    /// AM任务数量-为PMT提供任务数据量的接口对象
    /// </summary>
    [Serializable]
    public class TaskItemsCountModel
    {
        /// <summary>
        /// 代办任务数量
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// Notice 数量
        /// </summary>
        public int NoticeCount { get; set; }

        /// <summary>
        /// Reminder 数量
        /// </summary>
        public int ReminderCount { get; set; }

        /// <summary>
        /// Total = 代办任务数量 + Notice 数量 + Reminder 数量
        /// </summary>
        public int Total 
        {
            get
            {
                return TaskCount + NoticeCount + ReminderCount;
            }
        }

        /// <summary>
        /// 是否调用成功
        /// </summary>
        public bool Successfull { get; set; }

        /// <summary>
        /// 如果调用失败，则记录失败的详细信息。
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// PMT任务数量数据模型
    /// </summary>
    [Serializable]
    public class PMTTaskCountModel
    {
        /// <summary>
        /// 待处理
        /// </summary>
        public int DealCount { get; set; }

        /// <summary>
        /// 待批复
        /// </summary>
        public int ApproveCount { get; set; }

        /// <summary>
        /// 通知
        /// </summary>
        public int NotifyCount { get; set; }

        /// <summary>
        /// 提醒
        /// </summary>
        public int RemindCount { get; set; }

        /// <summary>
        /// 从AM到PMT的跳转链接
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 是否调用成功
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// 如果调用失败，则记录失败的详细信息。
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Task Count Model
    /// </summary>
    public class TaskCountModel
    {
        /// <summary>
        /// 代办任务数量
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// Notice 数量
        /// </summary>
        public int NoticeCount { get; set; }

        /// <summary>
        /// Reminder 数量
        /// </summary>
        public int ReminderCount { get; set; }

        /// <summary>
        /// 待处理
        /// </summary>
        public int PMT_DealCount { get; set; }

        /// <summary>
        /// 待批复
        /// </summary>
        public int PMT_ApproveCount { get; set; }

        /// <summary>
        /// 通知
        /// </summary>
        public int PMT_NotifyCount { get; set; }

        /// <summary>
        /// 提醒
        /// </summary>
        public int PMT_RemindCount { get; set; }

        /// <summary>
        /// 从AM到PMT的跳转链接
        /// </summary>
        public string PMT_URL { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public int PMT_Total { get; set; }
    }
}