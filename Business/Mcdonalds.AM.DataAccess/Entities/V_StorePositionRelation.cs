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
//  Filename: V_StorePostionRelation.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/22 14:47:01. 
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

namespace Mcdonalds.AM.DataAccess
{
    /// <summary>
    /// V_StorePostionRelation
    /// </summary>
    public partial class V_StorePostionRelation : BaseEntity<V_StorePostionRelation>
    {

        /// <summary>
        /// Query the store position by project identifier.
        /// </summary>
        /// <param name="projectID">The project identifier.</param>
        /// <param name="employeeAccount">The employee account.default - null</param>
        /// <returns>IQueryable&lt;V_StorePostionRelation&gt;.</returns>
        public IQueryable<V_StorePostionRelation> QueryStorePositionByProjectID( string projectID, string employeeAccount = null )
        {
            //IQueryable<V_StorePostionRelation> itemResult;

            //var context = GetDb();

            //if (string.IsNullOrEmpty(employeeAccount))
            //{
            //    itemResult = (from main in context.ProjectInfo
            //                  join child in context.V_StorePostionRelation on main.USCode equals child.Code
            //                  where main.ProjectId == projectID

            //                  select child).Distinct();
            //}
            //else
            //{
            //    itemResult = (from main in context.ProjectInfo
            //                  join child in context.V_StorePostionRelation on main.USCode equals child.Code
            //                  where main.ProjectId == projectID && child.EmployeeAccount == employeeAccount

            //                  select child).Distinct();
            //}

            return null;
        }
    }
}
