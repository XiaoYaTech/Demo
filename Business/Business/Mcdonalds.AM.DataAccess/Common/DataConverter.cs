/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/16/2014 5:13:08 PM
 * FileName     :   AMConverter
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public class DataConverter
    {
        public static decimal ToDecimal(string input)
        {
            decimal result = 0;
            decimal.TryParse(input, out result);
            return result;
        }

        public static decimal? ToDecimalNullable(string input)
        {
            
            decimal result;
            decimal.TryParse(input, out result);
            return (Nullable<decimal>)result;
        }

        public static string ToMoney(double input, bool showCent = false)
        {
            if (showCent)
                return Math.Round(input, 2).ToString("N2").TrimEnd('0').TrimEnd('.');
            else
                return Math.Round(input, 0).ToString("N0");
        }

        public static string ToMoney(decimal? input, bool showCent = false)
        {
            if (!input.HasValue)
            {
                return "";
            }
            if (showCent)
                return Math.Round(input.Value, 2).ToString("N2").TrimEnd('0').TrimEnd('.');
            else
                return Math.Round(input.Value, 0).ToString("N0");
        }

        public static string ToMoney(string input, bool showCent = false)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return ToMoney(ToDecimal(input), showCent);
        }

        public static string ToPercentage(string input)
        {
            double result = 0;
            if (!double.TryParse(input, out result))
            {
                return "0";
            }
            result = result * 100;
            return Math.Round(result, 1).ToString("N1").TrimEnd('0').TrimEnd('.');
        }

        public static string ToPercentage(decimal? input)
        {
            return input.HasValue ? Math.Round(input.Value * 100, 1).ToString("N1").TrimEnd('0').TrimEnd('.') : "";
        }

        public static string ToYesNo(string val)
        {
            if (string.IsNullOrEmpty(val) || val == "0")
            {
                return "N";
            }
            else
            {
                return "Y";
            }
        }

        /// <summary>
        /// 下载文件时，处理文件名中的空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHexString(string s)
        {
            char[] chars = s.ToCharArray();
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < chars.Length; index++)
            {
                bool needToEncode = NeedToEncode(chars[index]);
                if (needToEncode)
                {
                    string encodedString = ToHexString(chars[index]);
                    builder.Append(encodedString);
                }
                else
                {
                    builder.Append(chars[index]);
                }
            }
            return builder.ToString();
        }

        /// <summary>   
        /// 判断字符是否需要使用特殊的 ToHexString 的编码方式   
        /// </summary>   
        /// <param name="chr"></param>   
        /// <returns></returns>   
        private static bool NeedToEncode(char chr)
        {
            string reservedChars = "$-_.+!*'(),@=&";
            if (chr > 127)
                return true;
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
                return false;
            return true;
        }

        /// <summary>   
        /// 为非 ASCII 字符编码   
        /// </summary>   
        /// <param name="chr"></param>   
        /// <returns></returns>   
        private static string ToHexString(char chr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < encodedBytes.Length; index++)
            {
                builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
            }
            return builder.ToString();
        }
    }
}