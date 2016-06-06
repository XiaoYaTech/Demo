using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MoyeBuy.Com.IDAL;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.DALFactory;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.IUtility;
using MoyeBuy.Com.UtilityFactory;

namespace MoyeBuy.Com.BLL
{
    public class Cart
    {
        private static readonly ICart logicCart = UtilityFactory.Cart.CreateCart();
        public IList<ProductInfo> ListProd { get; set; }
        public List<Model.CartItem> ListCartItem { get; set; }
        Dictionary<string, object> item = null; 
        public Dictionary<string, object> Item
        {
            get
            {
                item = new Dictionary<string, object>();
                if (this.ListProd.Count > 0)
                {
                    item.Add("ListProd", this.ListProd);
                }
                if (this.ListCartItem.Count > 0)
                {
                    item.Add("ListCartItem", this.ListCartItem);
                }
                return this.item;
            }
        }
        public Cart()
        {
            string strCartData = "";
            string strProdIDs = "";
            List<Model.CartItem> ListCartItem = new List<Model.CartItem>();
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("CART") && HttpContext.Current.Request.Cookies["CART"]["DATA"] != "")
            {
                strCartData = HttpContext.Current.Request.Cookies["CART"]["DATA"];
                strCartData = MoyeBuyUtility.Encryption.DecryptString(strCartData);
                ListCartItem = MoyeBuyUtility.Gadget.DeserializeCartItems(strCartData);
                this.ListCartItem = ListCartItem;
                //foreach (var item in ListCartItem)
                //{
                //    strProdIDs += item.ProductId + "|||";
                //}
                //if (strProdIDs.Length > 3)
                //    strProdIDs = strProdIDs.Substring(0, strProdIDs.Length - 3);
                //Product pBll = new Product();
                //this.ListProd = pBll.GetProduct(strProdIDs);
            }
        }
        public Dictionary<string, object> GetAllCartItem()
        {
            string strCartData = "";
            string strProdIDs = "";
            List<Model.CartItem> ListCartItem = new List<Model.CartItem>();
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("CART") && HttpContext.Current.Request.Cookies["CART"]["DATA"] != "")
            {
                strCartData = HttpContext.Current.Request.Cookies["CART"]["DATA"];
                strCartData = MoyeBuyUtility.Encryption.DecryptString(strCartData);
                ListCartItem = MoyeBuyUtility.Gadget.DeserializeCartItems(strCartData);
                this.ListCartItem = ListCartItem;
                foreach (var item in ListCartItem)
                {
                    strProdIDs += item.ProductId + "|||";
                }
                if (strProdIDs.Length > 3)
                    strProdIDs = strProdIDs.Substring(0, strProdIDs.Length - 3);
                Product pBll = new Product();
                this.ListProd = pBll.GetProduct(strProdIDs);
            }
            return this.Item;
        }
        public void AddToCart(string strProdId,string strNum,decimal decPrice)
        {
            string strCartData = "";
            List<Model.CartItem> ListCartItem = new List<Model.CartItem>();
            string strProdIDs = "";
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains("CART") && HttpContext.Current.Request.Cookies["CART"]["DATA"]!="")
            {
                strCartData = HttpContext.Current.Request.Cookies["CART"]["DATA"];
                strCartData = MoyeBuyUtility.Encryption.DecryptString(strCartData);
                ListCartItem = MoyeBuyUtility.Gadget.DeserializeCartItems(strCartData);
                bool isExist = false;
                foreach (var item in ListCartItem)
                {
                    if (item.ProductId == strProdId)
                    {
                        isExist = true;
                    }
                }
                if (!string.IsNullOrEmpty(strProdId))
                {
                    if (isExist)
                    {
                        foreach (var item in ListCartItem)
                        {
                            if (item.ProductId == strProdId)
                            {
                                decimal singlePrice = item.Price / item.Count;
                                item.Count = logicCart.GetCartItemProdCount(item.Count + 1);
                                item.Price = logicCart.GetCartItemPrice(singlePrice, item.Count);
                            }
                        }
                    }
                    else
                    {
                        int intNum = Convert.ToInt32(strNum);
                        decimal decSinglePrice = decPrice / intNum;
                        Model.CartItem newitem = new Model.CartItem();
                        newitem.ProductId = strProdId;
                        newitem.Count =logicCart.GetCartItemProdCount(intNum);
                        newitem.Price = logicCart.GetCartItemPrice(decSinglePrice,intNum);
                        ListCartItem.Add(newitem);
                    }
                }

                foreach (var item in ListCartItem)
                {
                    strProdIDs += item.ProductId + "|||";
                }
                if (strProdIDs.Length > 3)
                    strProdIDs = strProdIDs.Substring(0, strProdIDs.Length - 3);
                strCartData = MoyeBuyUtility.Gadget.SerializeCartItems(ListCartItem).OuterXml;
            }
            else
            {
                int intNum = Convert.ToInt32(strNum);
                decimal decSinglePrice = decPrice / intNum;

                Model.CartItem item = new Model.CartItem();
                item.ProductId = strProdId;
                item.Count = logicCart.GetCartItemProdCount(intNum);
                item.Price = logicCart.GetCartItemPrice(decSinglePrice, intNum);

                ListCartItem.Add(item);
                strProdIDs = strProdId;
                strCartData = MoyeBuyUtility.Gadget.SerializeCartItems(ListCartItem).OuterXml;
            }
            if (strProdIDs!="")
            {
                Product pBll = new Product();
                this.ListProd = pBll.GetProduct(strProdIDs);
            }
            strCartData = MoyeBuyUtility.Encryption.EncryptString(strCartData);
            HttpContext.Current.Response.Cookies["CART"]["DATA"] = strCartData;
        }
        public bool DelProd(string strProdId)
        {
            if (string.IsNullOrEmpty(strProdId))
                return false;
            //for (int i = 0; i < this.ListProd.Count; i++)
            //{
            //    Model.ProductInfo item = this.ListProd[i];
            //    if (item.ProductId == strProdId)
            //    {
            //        this.ListProd.Remove(item);
            //    }
            //}
            for (int i = 0; i < this.ListCartItem.Count; i++)
            {
                Model.CartItem item = this.ListCartItem[i];
                if (item.ProductId == strProdId)
                {
                    this.ListCartItem.Remove(item);
                }
            }
            string strCartData = "";
            strCartData = MoyeBuyUtility.Gadget.SerializeCartItems(this.ListCartItem).OuterXml;
            strCartData = MoyeBuyUtility.Encryption.EncryptString(strCartData);
            HttpContext.Current.Response.Cookies["CART"]["DATA"] = strCartData;
            return true;
        }
    }
}
