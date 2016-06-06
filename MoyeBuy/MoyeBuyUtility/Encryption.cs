using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MoyeBuy.Com.MoyeBuyUtility
{
    public class Encryption
    {
        //used for the encryption and decryption
        private static SymmetricAlgorithm smaRijndaelService;
        private static Byte[] bytKey = new Byte[16];
        private static Byte[] bytIV = new Byte[16];
        private static string strKey = "r/miM3DP/HNNY7zUEc813QAv01VhuTzi";

        public static string SalRijndaelEncrypt(String strOrigString)
        {
            string strResult;		//Return Result

            //1. String Length cannot exceed 90Kb. Otherwise, buffer will overflow. See point 3 for reasons
            if (strOrigString.Length > 92160)
            {
                strResult = "Error Within 90Kb";
                return strResult;
            }

            //key length can not exceed (below or above) 32 characters
            if (strKey.Length != 32)
            {
                strResult = "Error Encryption Incorrect";
                return strResult;
            }

            smaRijndaelService = new RijndaelManaged();
            smaRijndaelService.KeySize = 128;
            smaRijndaelService.BlockSize = 128;

            //2. Generate the Keys
            ASCIIEncoding aseEnc = new ASCIIEncoding();
            aseEnc.GetBytes(strKey, 0, 16, bytKey, 0);
            aseEnc.GetBytes(strKey, 16, 16, bytIV, 0);


            //3. Prepare the String
            //	The first 5 character of the string is formatted to store the actual length of the data.
            //	This is the simplest way to remember to original length of the data, without resorting to complicated computations.
            //	If anyone figure a good way to 'remember' the original length to facilite the decryption without having to use additional function parameters, pls let me know.
            strOrigString = String.Format("{0,5:00000}" + strOrigString, strOrigString.Length);


            //4. Encrypt the Data
            byte[] bytData = new byte[strOrigString.Length];
            aseEnc = new ASCIIEncoding();
            aseEnc.GetBytes(strOrigString, 0, strOrigString.Length, bytData, 0);


            ICryptoTransform ictEncrypt = smaRijndaelService.CreateEncryptor(bytKey, bytIV);
            smaRijndaelService.Clear();

            //5. Perpare the streams:
            //	mmsOut is the output stream. 
            //	mmsStream is the input stream.
            //	cs is the transformation stream.
            System.IO.MemoryStream mmsStream = new System.IO.MemoryStream(bytData);
            CryptoStream crsStream = new CryptoStream(mmsStream, ictEncrypt, CryptoStreamMode.Read);
            System.IO.MemoryStream mmsOut = new System.IO.MemoryStream(strOrigString.Length);

            //6. Start performing the encryption
            int intBytesRead;
            byte[] bytOutput = new byte[strOrigString.Length];
            do
            {
                intBytesRead = crsStream.Read(bytOutput, 0, strOrigString.Length);
                if (intBytesRead != 0)
                    mmsOut.Write(bytOutput, 0, intBytesRead);
            } while (intBytesRead > 0);

            //7. Returns the encrypted result after it is base64 encoded
            //	In this case, the actual result is converted to base64 so that it can be transported over the HTTP protocol without deformation.
            if (mmsOut.Length == 0)
                strResult = "";
            else
                strResult = Convert.ToBase64String(mmsOut.GetBuffer(), 0, (int)mmsOut.Length);

            return strResult;
        }

        public static string SalRijndaelDecrypt(String strEncryptedString)
        {
            string strResult = string.Empty;

            //key length can not exceed (below or above) 32 characters
            if (strKey.Length != 32)
            {
                strResult = "Error Encryption Incorrect";
                return strResult;
            }

            smaRijndaelService = new RijndaelManaged();
            smaRijndaelService.KeySize = 128;
            smaRijndaelService.BlockSize = 128;

            //1.  Generate the keys
            ASCIIEncoding aseEnc = new ASCIIEncoding();
            aseEnc.GetBytes(strKey, 0, 16, bytKey, 0);
            aseEnc.GetBytes(strKey, 16, 16, bytIV, 0);

            //2. Initialize the service provider
            int intReturn = 0;

            ICryptoTransform ictDecrypt = smaRijndaelService.CreateDecryptor(bytKey, bytIV);

            //3. Prepare the streams:
            //	mmsOut is the output stream. 
            //	cs is the transformation stream.
            System.IO.MemoryStream mmsOut = new System.IO.MemoryStream();
            CryptoStream crsStream = new CryptoStream(mmsOut, ictDecrypt, CryptoStreamMode.Write);

            //4. Remember to revert the base64 encoding into a byte array to restore the original encrypted data stream
            byte[] bytPlain = new byte[strEncryptedString.Length];
            try
            {
                bytPlain = Convert.FromBase64CharArray(strEncryptedString.ToCharArray(), 0, strEncryptedString.Length);
            }
            catch (Exception)
            {
                strResult = "Error Input Data";
                return strEncryptedString;
            }

            long lngRead = 0;
            long lngTotal = strEncryptedString.Length;

            try
            {
                //5. Perform the actual decryption
                while (lngTotal >= lngRead)
                {
                    crsStream.Write(bytPlain, 0, (int)bytPlain.Length);
                    //smaRijndaelService.BlockSize=128
                    lngRead = mmsOut.Length + Convert.ToUInt32(((bytPlain.Length / smaRijndaelService.BlockSize) * smaRijndaelService.BlockSize));
                };

                aseEnc = new ASCIIEncoding();
                strResult = aseEnc.GetString(mmsOut.GetBuffer(), 0, (int)mmsOut.Length);
                smaRijndaelService.Clear();
                //6. Trim the string to return only the meaningful data
                //	Remember that in the encrypt function, the first 5 character holds the length of the actual data
                //	This is the simplest way to remember to original length of the data, without resorting to complicated computations.
                String strLen = strResult.Substring(0, 5);
                int intLen = Convert.ToInt32(strLen);
                strResult = strResult.Substring(5, intLen);
                intReturn = (int)mmsOut.Length;

                return strResult;
            }
            catch (Exception)
            {
                if (strResult.Length > 5)
                    strResult = strResult.Substring(5, (strResult.Length - 5));
                else
                    strResult = "Error Decryption Failed";
                return strResult;
            }
        }
        //密钥 
        private const string sKey = "qJzGEh6hESZDVJeCnFPGuxzaiB7NLQM3";
        //矢量，矢量可以为空 
        private const string sIV = "qcDY6X+aPLw=";
        //构造一个对称算法 
        private static SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();

        public static string EncryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            //指定加密的运算模式 
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            //获取或设置加密算法的填充模式 
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string DecryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            mCSP.Key = Convert.FromBase64String(sKey);
            mCSP.IV = Convert.FromBase64String(sIV);
            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;
            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
            byt = Convert.FromBase64String(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
