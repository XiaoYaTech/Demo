/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/14/2014 10:29:32 AM
 * FileName     :   PostConsInfoDTO
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class ConsInfoDTO<T1, T2>
        where T1 : BaseAbstractEntity
        where T2 : BaseAbstractEntity
    {
        public T1 Info { get; set; }
        public T2 Entity { get; set; }

        public ReinvestmentBasicInfo ReinBasicInfo { get; set; }

        public ReinvestmentCost ReinCost { get; set; }

        public WriteOffAmount WriteOff { get; set; }

        public string ProjectComment { get; set; }

        public string SN { get; set; }

        public bool Editable { get; set; }

        public bool Recallable { get; set; }

        public bool Savable { get; set; }
    }
}
