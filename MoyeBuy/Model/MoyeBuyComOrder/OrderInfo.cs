using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class OrderInfo
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string UserID { get; set; }
        public AddressInfo Address { get; set; }
        public decimal OrderTotal { get; set; }
        public OrderStateInfo OrderState { get; set; }//订单状态
        public ShippingModeInfo ShippingMode { get; set; }
        public DateTime ShippingTime { get; set; }
        public string PayBank { get; set; }
        public IList<ProductInfo> Products { get; set; }
        public Nullable<int> authorizationNumber { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
