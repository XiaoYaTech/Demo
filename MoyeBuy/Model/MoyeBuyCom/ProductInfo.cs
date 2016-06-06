using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class ProductInfo
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string ProductSpec { get; set; }//规格
        public string ProductImgs { get; set; }
        public Nullable<decimal> MoyeBuyPrice { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public bool IsSellHot { get; set; }
        public bool IsOnSell { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public IList<CommentInfo> Comment { get; set; }//评论
        public ProductCategory Category { get; set; }
        public ProductStore PStore { get; set; }//库存
        public string CategoryId { get; set; }
        public string ProductCount { get; set; }
        public string SupplierID { get; set; }
    }
}
