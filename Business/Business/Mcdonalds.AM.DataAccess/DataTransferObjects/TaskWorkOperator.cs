/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   2/4/2015 5:14:21 PM
 * FileName     :   TaskWorkOperator
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class TaskWorkOperator
    {
        public string TemplateENUS { get; set; }
        public string TemplateZHCN { get; set; }
        public string Code { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public string OperateMsgENUS
        {
            get
            {
                if (string.IsNullOrEmpty(TemplateENUS))
                {
                    return string.Format("Waiting for {0} to deal with", NameENUS);
                }
                else
                {
                    return string.Format(TemplateENUS, NameENUS);
                }
            }
        }
        public string OperateMsgZHCN
        {
            get
            {
                if (string.IsNullOrEmpty(TemplateENUS))
                {
                    return string.Format("等待{0}处理", NameENUS);
                }
                else
                {
                    return string.Format(TemplateZHCN, NameENUS);
                }
            }
        }

        public string OperateMsgDisp
        {
            get
            {
                if (ClientCookie.Language == SystemLanguage.ENUS)
                    return OperateMsgENUS;
                else
                    return OperateMsgZHCN;
            }
        }
    }
}
