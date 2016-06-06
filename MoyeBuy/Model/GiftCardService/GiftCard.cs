using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class GiftCard
    {
        public string GiftCardNoID { get; set; }
        public string GiftCardNo { get; set; }
        public string GifCardPwd { get; set; }
        public string GiftCardAmount { get; set; }
        public string UpdateByUserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsInvalid { get; set; }
        public string GiftCardInvalidID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
