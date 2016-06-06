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
//  Filename: ModNoticeReceivers.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/13 23:30:58. 
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
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace Mcdonalds.AM.DataAccess
{
    /// <summary>
    /// ModNoticeReceivers
    /// </summary>
    public partial class ModNoticeReceivers : BaseEntity<ModNoticeReceivers>
    {
        /// <summary>
        /// Update the list.
        /// </summary>
        /// <param name="theList">The list.</param>
        /// <returns>System.Int32.</returns>
        public static int UpdateList( List<ModNoticeReceivers> theList )
        {
            var _db = PrepareDb();
            
            foreach (var _item in theList)
            {
                _db.ModNoticeReceivers.Attach(_item);
                _db.Entry(_item).State = EntityState.Modified;
            }
            int result = 0;
            try
            {
                result = _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

            }

            return result;
        }
    }
}
