using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using MoyeBuy.Com.IDAL;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.DALFactory;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.BLL
{
    public class Ad
    {
        private static readonly MoyeBuy.Com.IDAL.IAds dal = DataAcess.CreateAds();

        public IList<MoyeBuy.Com.Model.Ad> GetAds()
        {
            return dal.GetAds();
        }
    }
}
