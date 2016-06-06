using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public static class Utils
    {
        public static string GetYearMonth(string year, string month)
        {
            string rtnStr = string.Empty;
            switch (month)
            {
                case "01":
                    rtnStr = year + " January";
                    break;
                case "02":
                    rtnStr = year + " February";
                    break;
                case "03":
                    rtnStr = year + " March";
                    break;
                case "04":
                    rtnStr = year + " April";
                    break;
                case "05":
                    rtnStr = year + " May";
                    break;
                case "06":
                    rtnStr = year + " June";
                    break;
                case "07":
                    rtnStr = year + " July";
                    break;
                case "08":
                    rtnStr = year + " August";
                    break;
                case "09":
                    rtnStr = year + " September";
                    break;
                case "10":
                    rtnStr = year + " October";
                    break;
                case "11":
                    rtnStr = year + " November";
                    break;
                case "12":
                    rtnStr = year + " December";
                    break;
                default:
                    break;
            }
            return rtnStr;
        }

        public static void GetYearMonth(string yearmonth, ref string year, ref string month)
        {
            if (yearmonth.Split(' ').Length == 2)
            {
                year = yearmonth.Split(' ')[0];
                month = yearmonth.Split(' ')[1];
                switch (month)
                {
                    case "January":
                        month = " 01";
                        break;
                    case "February":
                        month = " 02";
                        break;
                    case "March":
                        month = " 03";
                        break;
                    case "April":
                        month = " 04";
                        break;
                    case "May":
                        month = " 05";
                        break;
                    case "June":
                        month = " 06";
                        break;
                    case "July":
                        month = " 07";
                        break;
                    case "August":
                        month = " 08";
                        break;
                    case "September":
                        month = " 09";
                        break;
                    case "October":
                        month = " 10";
                        break;
                    case "November":
                        month = " 11";
                        break;
                    case "December":
                        month = " 12";
                        break;
                    default:
                        month = "";
                        break;
                }
            }
        }

        public static string GetLatestYear()
        {
            string rtnStr = string.Empty;
            if (DateTime.Now.Month == 1)
            {
                rtnStr = DateTime.Now.AddYears(-1).Year.ToString();
            }
            else
            {
                rtnStr = DateTime.Now.Year.ToString();
            }
            return rtnStr;
        }

        public static string GetLatestMonth()
        {
            string rtnStr = string.Empty;
            if (DateTime.Now.Month == 1)
            {
                rtnStr = "12";
            }
            else
            {
                rtnStr = DateTime.Now.AddMonths(-1).Month.ToString();
                if (rtnStr.Length < 2)
                {
                    rtnStr = "0" + rtnStr;
                }
            }
            return rtnStr;
        }
    }
}