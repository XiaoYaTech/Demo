#region ---- [ Copyright ] ----
//================================================================= 
//  Copyright (C) 2014 NTT DATA Inc All rights reserved. 
//     
//  The information contained herein is confidential, proprietary 
//  to NTT DATA Inc. Use of this information by anyone other than 
//  authorized employees of NTT DATA Inc is granted only under a 
//  written non-disclosure agreement, expressly prescribing the 
//  scope and manner of such use. 
//================================================================= 
//  Filename: ModNotices.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/13 23:30:13. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.DataModels.Condition;

namespace Mcdonalds.AM.DataAccess
{
    /// <summary>
    /// ModNotices
    /// </summary>
    public partial class ModNotices : BaseEntity<ModNotices>
    {

        /// <summary>
        /// Query the notices by conditions.通过查询条件组来查询notice
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>IQueryable&lt;ModNotices&gt;.</returns>
        public IQueryable<ModNotices> QueryNoticesByConditions( SearchNoticeCondition condition )
        {
            string receiver, storeCode;
            receiver = condition.Receiver;
            storeCode = condition.StoreCode;

            //IQueryable<ModNotices> itemResult;

            var context = GetDb();


            //{
            //    itemResult = (from child in context.ModNoticeReceivers
            //                  join main in context.ModNotices on child.NoticeId equals main.Id
            //                  where child.Receiver == receiver
            //                  && (string.IsNullOrEmpty(condition.ProcessId) || main.ProcessId.Contains( condition.ProcessId) )
            //                  && (string.IsNullOrEmpty(condition.Title) || main.Title.Contains(condition.Title))
            //                  && (string.IsNullOrEmpty(condition.SenderName) || main.SenderNameENUS.Contains(condition.SenderName) || main.SenderNameZHCN.Contains(condition.SenderName))
            //                  && (!condition.DateFrom.HasValue || main.CreatedTime >= condition.DateFrom )
            //                  && (!condition.DateTo.HasValue || main.CreatedTime <= condition.DateTo )

            //                  || (from role in context.V_RoleEmployeeRelation where role.Code == receiver select role.Code).Contains(child.Receiver)

            //                  || (from posit in context.V_StorePostionRelation
            //                      where (!string.IsNullOrEmpty(storeCode) && posit.Code == storeCode && posit.EmployeeAccount == receiver)
            //                        || (string.IsNullOrEmpty(storeCode) && posit.EmployeeAccount == receiver)
            //                      select posit.PositionCode
            //                      ).Contains(child.Receiver)

            //                  select main).Distinct();
            //}

            return null;

        }

        /// <summary>
        /// Queries the notices by receivers.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="storeCode">The store code.</param>
        /// <returns>IQueryable&lt;ModNotices&gt;.</returns>
        public IQueryable<ModNotices> QueryNoticesByReceivers(string receiver, string storeCode = null)
        {
            //IQueryable<ModNotices> itemResult;

            //var context = GetDb();
            

            //{
            //    itemResult = (from child in context.ModNoticeReceivers
            //                  join main in context.ModNotices on child.NoticeId equals main.Id
            //                  where child.Receiver == receiver

            //                  || (from role in context.V_RoleEmployeeRelation where role.Code == receiver select role.Code).Contains(child.Receiver)

            //                  || (from posit in context.V_StorePostionRelation
            //                      where (!string.IsNullOrEmpty(storeCode) && posit.Code == storeCode && posit.EmployeeAccount == receiver)
            //                        || (string.IsNullOrEmpty(storeCode) && posit.EmployeeAccount == receiver)
            //                      select posit.PositionCode
            //                      ).Contains(child.Receiver)

            //                  select main).Distinct();
            //}
            
            return null;
        }

        /// <summary>
        /// Get the notices by receivers.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        /// <param name="storeCode">The store code.</param>
        /// <returns>List&lt;ModNotices&gt;.</returns>
        public List<ModNotices> GetNoticesByReceivers( string receiver, string storeCode = null )
        {
            List<ModNotices> itemsList;
            var result = QueryNoticesByReceivers(receiver, storeCode).AsEnumerable();
            itemsList = result.ToList();
            
            return itemsList;
        }

        /// <summary>
        /// 获取通知的数量
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public static int GetNotifyCountByEid(string eid)
        {
            var count = Search(n => n.SenderAccount == eid).Count();
            return count;
        }

    }
}
