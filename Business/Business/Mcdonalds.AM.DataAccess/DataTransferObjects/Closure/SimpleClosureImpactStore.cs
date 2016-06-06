/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/1/2014 1:20:28 AM
 * FileName     :   SimpleClosureImpactStore
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class SimpleClosureImpactStore
    {
        public string StoreCode { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public decimal? ImpactSaltes { get; set; }
        public bool IsSelected { get; set; }
    }
}