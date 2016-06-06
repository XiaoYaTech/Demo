using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;

namespace Expose178.Com.GadgetScripts
{
    public class FormatInfo
    {
        private DateTimeFormatInfo _dfi;
        private CultureInfo _nfCulture;
        private CultureInfo _dfCulture;

        public FormatInfo()
        {
            _nfCulture = new CultureInfo(1033);
            _dfCulture = new CultureInfo(1033);
        }
        public void SetNumberFormatCultureCode(int intLCID)
        {
            _nfCulture = new CultureInfo(intLCID);
        }

        public void SetDateTimeFormatCultureCode(int intLCID)
        {
            _dfCulture = new CultureInfo(intLCID);
        }

        public string FormatPercent(double dblExpression, int intNumDigitsAfterDecimal,
                                    bool bolUseParensForNegativeNumbers, bool bolGroupDigits)
        {
            string strExpression = null;
            NumberFormatInfo _nfi;
            _nfi = _nfCulture.NumberFormat;

            _nfi.PercentDecimalDigits = intNumDigitsAfterDecimal;
            if (!bolGroupDigits)
                _nfi.PercentGroupSeparator = "";

            if (bolUseParensForNegativeNumbers)
                _nfi.PercentNegativePattern = 1;

            _nfi.PercentPositivePattern = 1;

            strExpression = dblExpression.ToString("P", _nfi);

            return strExpression;
        }

        public string FormatDateTime(string strDateTime, int intFormat, params string[] strDateTimeFormat)
        {
            int intLocaleID = 1033;

            string strExpression = null;
            try
            {
                if (strDateTime != "" && strDateTime != null)
                {
                    string strFormat = null;
                    SetDateTimeFormatCultureCode(intLocaleID);
                    _dfi = _dfCulture.DateTimeFormat;
                    //dDateTime = (DateTime) dDateTime;
                    DateTime dtmDateTime;
                    dtmDateTime = Convert.ToDateTime(strDateTime);
                    switch (intFormat)
                    {
                        case 99: //Custom Date Format
                            strFormat = strDateTimeFormat.Length == 0 ? "d" : strDateTimeFormat[0];
                            break;
                        case 0:	//General (short date && long time) - Ex: 6/9/2006 4:47:07 PM 
                            strFormat = "G";
                            break;
                        case 1:	//LongDatePattern - Ex: Friday, June 09, 2006 
                            strFormat = "D";
                            break;
                        case 2:	//ShortDatePattern - Ex: 6/9/2006 
                            strFormat = "d";
                            break;
                        case 3:	//LongTimePattern - Ex: 4:48:17 PM
                            strFormat = "T";
                            break;
                        case 4:	//ShortTimePattern - Ex: 4:48 PM
                            strFormat = "t";
                            break;
                        case 5:	//General (short date && short time) - Ex: 6/9/2006 4:48 PM
                            strFormat = "g";
                            break;
                        case 6:	//FullDateTimePattern (long date && long time) - Ex: Friday, June 09, 2006 4:49:26 PM
                            strFormat = "F";
                            break;
                        case 7:	//Full date && time (long date && short time) - Ex: Friday, June 09, 2006 4:49 PM
                            strFormat = "f";
                            break;
                        case 8:	//UniversalSortableDateTimePattern using the format for universal time display - Ex: 2006-06-09 16:52:32Z
                            strFormat = "u";
                            break;
                        case 9:	//Full date && time (long date && long time) using universal time - Ex: Friday, June 09, 2006 8:52:51 PM
                            strFormat = "U";
                            break;
                    }
                    strExpression = dtmDateTime.ToString(strFormat, _dfi);

                    //changed by Young, I don't know why we wrote this logic here, actually, if you set Culture be 1033 the format will be "mm/dd/yy" automatically 
                    //if (intFormat == 2)
                    //{
                    //    strExpression = String.Format("{0}/{1}/{2}", dtmDateTime.Month.ToString("00"), dtmDateTime.Day.ToString("00"), dtmDateTime.Year); //NoRemoveXX dtmDateTime.Month,Day,Year. They are int.
                    //}
                }
                else
                    strExpression = "";

            }
            catch
            {
                strExpression = strDateTime;
            }
            return strExpression;
        }

        public static bool SalIsNumeric(string strNumber)
        {
            try
            {
                Convert.ToDouble(strNumber);
                return true;
            }
            catch (Exception expError)
            {
                return false;
            }

        }

        public static bool SalIsDate(string strDate)
        {
            try
            {
                Convert.ToDateTime(strDate);
                return true;
            }
            catch (Exception expError)
            {
                return false;
            }
        }

        public static bool SalIsNull(object obj)
        {
            if (obj != null)
                return false;
            else
                return true;
        }

        //' check if the first char in string is a number && follow by a char
        public static bool SalCheckIsOneNumber(string strValue)
        {
            bool bolReturnValue;
            bolReturnValue = false;
            if (strValue == null)
                strValue = "";

            if (String.Compare(strValue, "", true) != 0)
            {
                if (strValue.Length > 1)
                    if (SalIsNumeric(strValue.Substring(1, 1)) == true && SalIsNumeric(strValue.Substring(2, 1)) == false)
                        bolReturnValue = true;
                    else
                        if (SalIsNumeric(strValue))
                            bolReturnValue = true;
            }
            return bolReturnValue;

        }

        public static string SalFormatString(string strValue)
        {
            string strNewFormat;
            strNewFormat = "";
            if (strValue != null && String.Compare(strValue, "", true) != 0)
            {
                strNewFormat = Convert.ToString(strValue).Trim();
                if (strNewFormat.IndexOf("\"\"") > 0)
                    strNewFormat = strNewFormat.Replace("\"\"", "\"\"\"\"");

                if (strNewFormat.IndexOf(",") > 0 || strNewFormat.Substring(1, 1) == "0")
                {
                    if (strNewFormat.Substring(1, 1) == "0")
                        strNewFormat = "=" + "\"\"" + strNewFormat + "\"\"";
                    else
                        strNewFormat = "\"\"" + strNewFormat + "\"\"";

                }
            }
            return strNewFormat;
        }
        public static double SalRound(double objInputValue, int intDigs)
        {
            double dblInputValue = 0;

            dblInputValue = Convert.ToDouble(GetRound(objInputValue, intDigs));
            return dblInputValue;
        }
        public static decimal SalRound(object objInputValue, int intDigs)
        {
            decimal decInputValue = 0;
            decInputValue = Convert.ToDecimal(GetRound(objInputValue, intDigs));
            return decInputValue;
        }
        public static decimal SalRound(object objInputValue)
        {
            int intDigs = 0;
            decimal decInputValue = 0;
            decInputValue = Convert.ToDecimal(GetRound(objInputValue, intDigs));
            return decInputValue;
        }
        public static double SalRound(double objInputValue)
        {
            int intDigs = 0;
            double dblInputValue = 0;
            dblInputValue = Convert.ToDouble(GetRound(objInputValue, intDigs));
            return dblInputValue;
        }
        //this function is used Round, in C# we have two ways to handle the midpointround, getawayfromzero or to even
        //we want to get the standard one, if the number after rounding is 5, go up
        //example 0.285, after round two decimals, it will be 0.29
        private static string GetRound(object objInputValue, int intDigs)
        {
            decimal decInputValue = 0;
            if (SalIsNumeric(objInputValue.ToString()))
            {
                try
                {
                    decInputValue = Convert.ToDecimal(objInputValue.ToString());
                }
                catch
                {
                    decInputValue = 0;
                }
            }
            double dblIndex = Math.Pow(10, 0 - intDigs) * 0.5;
            string strIndex = "\\" + Convert.ToDecimal(dblIndex).ToString().Substring(1).Replace("0", "\\d");
            //here is used to find the number after the rounding, like if you want to round 2 digs,
            //the value is 0.285, check the 3rd number after '.', if it's 5, go up
            //the other condition, C# already handles
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(strIndex);
            if (rg.Match(objInputValue.ToString()).Success)
                decInputValue = Math.Ceiling(decInputValue / Convert.ToDecimal(Math.Pow(10, 0 - intDigs))) * Convert.ToDecimal(Math.Pow(10, 0 - intDigs));
            else
                decInputValue = Math.Round(decInputValue, intDigs);
            return decInputValue.ToString();
        }

        public static string SalFormatStringCSV(string strValue)
        {
            string strNewFormat;
            strNewFormat = "";
            if (!SalIsNull(strValue) && strValue.CompareTo("") != 0)
            {
                strNewFormat = strValue;
                if (strNewFormat.IndexOf("\"") >= 0)
                    strNewFormat = strNewFormat.Replace("\"", "\"\"");
                if (strNewFormat.IndexOf(",") >= 0 || strNewFormat.IndexOf("0") == 0)
                {
                    if (strNewFormat.IndexOf("0") == 0)
                        strNewFormat = "=" + "\"" + strNewFormat + "\"";
                    else
                        strNewFormat = "\"" + strNewFormat + "\"";
                }
            }

            return strNewFormat;
        }

        public string Left(string strInput, int intLength)
        {
            string strTemp = strInput.Substring(0, intLength);
            return strTemp;
        }
        public string Right(string strInput, int intLength)
        {
            string strTemp = strInput.Substring(strInput.Length - intLength, intLength);
            return strTemp;
        }
        public string Mid(string strInput, int intStartIndex, int intLength)
        {
            string strTemp = strInput.Substring(intStartIndex, intLength);
            return strTemp;
        }
        public string Mid(string strInput, int intStartIndex)
        {
            string strTemp = strInput.Substring(intStartIndex);
            return strTemp;
        }

        private static double MyDynamicRound(double dblInputValue)
        {
            //Rounding will happen dynamically for each value following these rules:
            //If the value has:
            //3 decimals to the left -> round to 100
            //4 decimals to the left -> round to 1k
            //5 decimals to the left -> round to 5k
            //6 decimals to the left -> round to 10k
            //7 decimals to the left -> round to 100k
            //8 decimals to the left -> round to 1M
            //example 23000 should be 25000
            int intMultiple = 1;
            if (dblInputValue < 100)
                intMultiple = 1;
            else if (dblInputValue < 1000)
                intMultiple = 100;
            else if (dblInputValue < 10000)
                intMultiple = 1000;
            else if (dblInputValue < 100000)
                intMultiple = 5000;
            else if (dblInputValue < 1000000)
                intMultiple = 10000;
            else if (dblInputValue < 10000000)
                intMultiple = 100000;
            else
                intMultiple = 1000000;

            dblInputValue = dblInputValue / intMultiple;
            //if the value is like 12.5, the round will be 12 sometime
            if (dblInputValue.ToString().IndexOf(".5") >= 0)
                return Math.Ceiling(dblInputValue) * intMultiple;
            else
                return Math.Round(dblInputValue) * intMultiple;
        }

        public static DateTime SalConvertDateTime(string strDateTime)
        {
            DateTime dateResult = new DateTime();
            if (!DateTime.TryParse(strDateTime, out dateResult))
            {
                dateResult = DateTime.Now.AddYears(-1);
            }
            return dateResult;
        }

        public static string SalFormatDateTime(string strDateTime, string strFormat, int LCID)
        {
            DateTime dateResult = new DateTime();
            if (DateTime.TryParse(strDateTime, out dateResult))
            {
                return dateResult.ToString(strFormat, new CultureInfo(LCID));
            }
            return string.Empty;
        }

        public static string SalGetValueFromRow(DataRow drDatas, string columnName)
        {
            if (drDatas != null && drDatas[columnName] != DBNull.Value)
            {
                return drDatas[columnName].ToString();
            }
            return string.Empty;
        }

        public static int SalConvertToInt32(object source)
        {
            Int32 result = 0;
            if (source != null)
                Int32.TryParse(source.ToString(), out result);
            return result;
        }
        public static string SalConvertToString(object source)
        {
            if (source != null)
                return source.ToString().Trim();
            return string.Empty;
        }

        public static string ConvertToDefaultDate(string strInputDate)
        {
            try
            {
                string strReturnDate = "";
                if (strInputDate != "")
                {
                    string strShortDateFormat = "yyyy/mm/dd";

                    string strday, strmonth, stryear;
                    strday = "";
                    strmonth = "";
                    stryear = "";
                    int iFormatLength = 0;
                    int iInputLength = 0;

                    char[] arrStrFormat = strShortDateFormat.ToLower().ToCharArray();
                    char[] arrStrInputDate = strInputDate.ToLower().ToCharArray();

                    while ((iFormatLength - arrStrFormat.Length < 0) && (iInputLength - arrStrInputDate.Length < 0))
                    {
                        if (arrStrFormat[iFormatLength].ToString() == "d")
                        {
                            if (iInputLength < arrStrInputDate.Length)
                            {
                                strday = strday + arrStrInputDate[iInputLength].ToString();
                                while (iFormatLength + 1 < arrStrFormat.Length && arrStrFormat[iFormatLength + 1].ToString() == "d")
                                    iFormatLength = iFormatLength + 1;
                                while (iInputLength + 1 < arrStrInputDate.Length && arrStrInputDate[iInputLength + 1].ToString() != "/" && arrStrInputDate[iInputLength + 1].ToString() != "-" && arrStrInputDate[iInputLength + 1].ToString() != " ")
                                {
                                    iInputLength = iInputLength + 1;
                                    if (iInputLength < arrStrInputDate.Length)
                                        strday = strday + arrStrInputDate[iInputLength].ToString();
                                }
                            }
                        }
                        else if (arrStrFormat[iFormatLength].ToString() == "m")
                        {
                            if (iInputLength < arrStrInputDate.Length)
                            {
                                strmonth = strmonth + arrStrInputDate[iInputLength].ToString();
                                while (iFormatLength + 1 < arrStrFormat.Length && arrStrFormat[iFormatLength + 1].ToString() == "m")
                                    iFormatLength = iFormatLength + 1;
                                while (iInputLength + 1 < arrStrInputDate.Length && arrStrInputDate[iInputLength + 1].ToString() != "/" && arrStrInputDate[iInputLength + 1].ToString() != "-" && arrStrInputDate[iInputLength + 1].ToString() != " ")
                                {
                                    iInputLength = iInputLength + 1;
                                    if (iInputLength < arrStrInputDate.Length)
                                        strmonth = strmonth + arrStrInputDate[iInputLength].ToString();
                                }
                            }
                        }
                        else if (arrStrFormat[iFormatLength].ToString() == "y")
                        {
                            if (iInputLength < arrStrInputDate.Length)
                            {
                                stryear = stryear + arrStrInputDate[iInputLength].ToString();
                                while (iFormatLength + 1 < arrStrFormat.Length && arrStrFormat[iFormatLength + 1].ToString() == "y")
                                    iFormatLength = iFormatLength + 1;
                                while (iInputLength + 1 < arrStrInputDate.Length && arrStrInputDate[iInputLength + 1].ToString() != "/" && arrStrInputDate[iInputLength + 1].ToString() != "-" && arrStrInputDate[iInputLength + 1].ToString() != " ")
                                {
                                    iInputLength = iInputLength + 1;
                                    if (iInputLength < arrStrInputDate.Length)
                                        stryear = stryear + arrStrInputDate[iInputLength].ToString();
                                }
                            }
                        }
                        iInputLength = iInputLength + 1;
                        iFormatLength = iFormatLength + 1;
                    }
                    strReturnDate = strmonth + "/" + strday + "/" + stryear;
                }
                if (SalIsDate(strReturnDate))
                    return strReturnDate;
                else
                    return strInputDate;
            }
            catch (Exception exp)
            {
                return strInputDate;
            }
        }
    }
}
