using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class Ads:DALBase,IDAL.IAds
    {
        public IList<Model.Ad> GetAds()
        {
            IList<MoyeBuy.Com.Model.Ad> ads = null;
            DataSet dsAds = GetDataSetAds();
            if (Gadget.DatatSetIsNotNullOrEmpty(dsAds))
            {
                ads = new List<MoyeBuy.Com.Model.Ad>();
                foreach (DataRow dr in dsAds.Tables[0].Rows)
                {
                    MoyeBuy.Com.Model.Ad ad = new Model.Ad();
                    ad.AdID = Gadget.GetDataRowIntValue(dr, "AdID");
                    ad.AdTitle = Gadget.GetDataRowStringValue(dr, "AdTitle");
                    ad.AdType = Gadget.GetDataRowStringValue(dr, "AdType");
                    ad.AdImgs = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdImgs"), "|");
                    ad.AdImgAltTitle = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdTitle"), "|");
                    ad.AdImigDesc = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdImigDesc"), "|");
                    ad.AdImgDisq = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdImgDisq"), "|");
                    ad.AdUrl = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdUrl"), "|");
                    ad.AdTarget = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdTarget"), "|");
                    ad.AdClassName = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdClassName"), "|");
                    ad.AdControlID = Gadget.Split<string>(Gadget.GetDataRowStringValue(dr, "AdControlID"), "|");
                }
            }
            return ads;
        }

        public bool InsertAds(IList<Model.Ad> listAd)
        {
            throw new NotImplementedException();
        }

        private DataSet GetDataSetAds()
        {
            DataSet dsAds = null;
            Hashtable hshParam = new Hashtable();
            dsAds = dbOperator.ProcessData("usp_GetMoyeBuyAds", hshParam, strDSN);
            return dsAds;
        }
    }
}
