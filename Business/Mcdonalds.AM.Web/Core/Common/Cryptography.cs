//================================================================= 
//  Copyright (C) 2014 NTT DATA Inc All rights reserved. 
//     
//  The information contained herein is confidential, proprietary 
//  to NTT DATA Inc. Use of this information by anyone other than 
//  authorized employees of NTT DATA Inc is granted only under a 
//  written non-disclosure agreement, expressly prescribing the 
//  scope and manner of such use. 
//================================================================= 
//  Filename: Cryptography.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/2 13:02:50. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Text;
using System.IO;

namespace Mcdonalds.AM.Web.Core.Common
{
    /// <summary>
    /// Cryptography
    /// </summary>
    public partial class Cryptography
    {
        private const int s_saltLength = 8;

        #region 数据加密（AES）

        /// <summary>
        /// AES数据加密
        /// SALT为当天日期，格式为yyyyMMdd，例如20121231（2012年12月31日）
        /// </summary>
        /// <param name="input">需要被加密的字符串</param>
        /// <param name="salt">随机值（SALT）</param>
        /// <param name="password">密码</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string input, string salt, string password)
        {
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = System.Text.UTF8Encoding.UTF8.GetBytes(salt);

            // AesManaged - 高级加密标准(AES) 对称算法的管理类
            System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();

            // Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数)
            // 通过 密码 和 salt 派生密钥
            System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(password, saltBytes);

            /**/
            /*
         * AesManaged.BlockSize - 加密操作的块大小（单位：bit）
         * AesManaged.LegalBlockSizes - 对称算法支持的块大小（单位：bit）
         * AesManaged.KeySize - 对称算法的密钥大小（单位：bit）
         * AesManaged.LegalKeySizes - 对称算法支持的密钥大小（单位：bit）
         * AesManaged.Key - 对称算法的密钥
         * AesManaged.IV - 对称算法的密钥大小
         * Rfc2898DeriveBytes.GetBytes(int 需要生成的伪随机密钥字节数) - 生成密钥
         */

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象
            System.Security.Cryptography.ICryptoTransform encryptTransform = aes.CreateEncryptor();

            // 加密后的输出流
            System.IO.MemoryStream encryptStream = new System.IO.MemoryStream();

            // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接
            System.Security.Cryptography.CryptoStream encryptor = new System.Security.Cryptography.CryptoStream
                (encryptStream, encryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成加密的过程）
            encryptor.Write(data, 0, data.Length);
            encryptor.Close();

            // 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串
            string encryptedString = Convert.ToBase64String(encryptStream.ToArray());

            return encryptedString;
        }
        #endregion

        #region 数据解密（AES）
        /// <summary>
        /// AES数据解密
        /// SALT先用当天的日期尝试
        /// 如果失败的话，就用昨天的日期来尝试（考虑到0:00-0:30的时候可能会出现这种问题
        /// SALT默认为8位，如果为9位的话，说明是已经采用昨天的日期作为SALT（默认再最前添加一个‘Y’字符作为flag）
        /// 如果都解密失败的话，就返回null
        /// </summary>
        /// <param name="input">解密前的字符串</param>
        /// <param name="salt">随机值（SALT），默认为8位，如果为9位的话，说明是已经采用昨天的日期作为SALT</param>
        /// <param name="password">密码</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string input, string salt, string password)
        {
            byte[] encryptBytes;
            try
            {
                //传参数时，会将加号替换成空格
                encryptBytes = Convert.FromBase64String(input.Replace(' ', '+'));
            }
            catch
            {
                return null;
            }
            bool isLastDaySalt = (salt.Length > s_saltLength) ? true : false;
            if (isLastDaySalt)
                salt = salt.Substring(salt.Length - s_saltLength);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);


            System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();

            System.Security.Cryptography.Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(password, saltBytes);

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象
            System.Security.Cryptography.ICryptoTransform decryptTransform = aes.CreateDecryptor();

            // 解密后的输出流
            MemoryStream decryptStream = new MemoryStream();

            // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接
            System.Security.Cryptography.CryptoStream decryptor = new System.Security.Cryptography.CryptoStream(
                decryptStream, decryptTransform, System.Security.Cryptography.CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成解密的过程）

            decryptor.Write(encryptBytes, 0, encryptBytes.Length);

            try
            {
                decryptor.Close();
            }
            catch
            {
                if (isLastDaySalt)
                {
                    decryptor = null;
                    return null;
                }
                DateTime parsedDate;
                DateTime.TryParseExact(salt, "yyyyMMdd", null,
                                       DateTimeStyles.None, out parsedDate);
                if (parsedDate.Year == 1)//Datetime format error, output 0001/1/1
                    return null;
                string lastDaySalt = string.Format("Y{0}"
                    , parsedDate.AddDays(1).ToString("yyyyMMdd"));//stand for yesterday's salt, and add "Y" in the 1st character as flag
                var lastDaydecryptedString = Decrypt(input, lastDaySalt, password);

                return lastDaydecryptedString;
            }

            // 将解密后所得到的流转换为字符串
            byte[] decryptBytes = decryptStream.ToArray();
            string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            return decryptedString;
        }

        #endregion
    }
}