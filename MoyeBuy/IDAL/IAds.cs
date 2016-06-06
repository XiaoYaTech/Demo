using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface IAds
    {
        IList<MoyeBuy.Com.Model.Ad> GetAds();
        bool InsertAds(IList<Model.Ad> listAd);
    }
}
