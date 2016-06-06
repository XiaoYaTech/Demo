/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/29/2014 10:50:53 AM
 * FileName     :   TopNavigator
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class TopNavigator
    {
        public string ParentCode { get; set; }
        public string Code { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public string Url { get; set; }
        public bool IsSelected { get; set; }
        public bool IsFinished { get; set; }
        public double? ProgressRate { get; set; }
        public bool EditApproverable { get; set; }
        public bool Editable { get; set; }
        public bool Recallable { get; set; }
        public int ExecuteSequence { get; set; }
        public string Percentage { get; set; }

        public ProjectStatus Status { get; set; }

        public bool IsRejected { get; set; }
    }
}