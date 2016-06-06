using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PagedDataSource
    {
        public int TotalItems { get; set; }
        public List<Object> List { get; set; }
        public PagedDataSource(int totalItems, Object[] items)
        {
            TotalItems = totalItems;
            List = items.ToList();
        }
    }
}