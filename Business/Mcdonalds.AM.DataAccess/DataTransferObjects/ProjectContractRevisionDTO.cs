using System;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class ProjectContractRevisionDTO
    {
        public Nullable<System.DateTime> LeaseChangeExpiryOld { get; set; }

        public string RedlineAreaOld { get; set; }
        public string LandlordOld { get; set; }
        public string RentStructureOld { get; set; }

        public Nullable<decimal> OldChangeRedLineRedLineArea
        {
            get
            {
                Nullable<decimal> oldChangeRedLineRedLineArea = null;
                if (!string.IsNullOrEmpty(RedlineAreaOld))
                {
                    oldChangeRedLineRedLineArea = Convert.ToDecimal(RedlineAreaOld);
                }

                return oldChangeRedLineRedLineArea;
            }
        }
    }
}
