using System;
using System.Collections.Generic;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PackageHoldingDto
    {
        public string ProjectId { get; set; }

        public string FlowCode { get; set; }

        public string HoldingPackageCode
        {
            get
            {
                var holdingPackageCode = string.Empty;
                switch (FlowCode)
                {
                    case Constants.FlowCode.Reimage:
                        holdingPackageCode = Constants.FlowCode.Reimage_Package;
                        break;
                }

                return holdingPackageCode;
            }
        }

        public HoldingStatus Status { get; set; }

        public string ErrorMsg { get; set; }

        public bool HasRight { get; set; }

        public List<SelectItem> HoldingStatusList
        {
            get
            {
                return new List<SelectItem>()
                {
                    new SelectItem(){Name = "Yes",IntValue = 1},
                    new SelectItem(){Name = "No",IntValue = 2}
                };
            }
        }

    }

    
}
