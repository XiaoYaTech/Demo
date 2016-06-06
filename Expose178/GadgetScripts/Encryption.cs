using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Expose178.Com.GadgetScripts
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
            System.IO.MemoryStream mmsOut = new System.IO.MemoryStream();

            //6. Start performing the encryption
            int intBytesRead;
            byte[] bytOutput = new byte[1024];
            do
            {
                intBytesRead = crsStream.Read(bytOutput, 0, 1024);
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

        public static string SalGenerateHashCode(string strInputText)
        {
            SHA1CryptoServiceProvider cspHash = new SHA1CryptoServiceProvider();
            byte[] arrInput = new byte[strInputText.Length];
            byte[] arrOutput = new byte[strInputText.Length];
            string strOutputText;

            ASCIIEncoding encAscii = new ASCIIEncoding();
            encAscii.GetBytes(strInputText, 0, strInputText.Length, arrInput, 0);

            arrOutput = cspHash.ComputeHash(arrInput);

            strOutputText = "";
            for (int i = 0; i < arrOutput.Length; i++)
                strOutputText += String.Format("{0:x2}", arrOutput[i]);

            cspHash.Clear();
            encAscii = null;

            return strOutputText;
        }

        public static string SalGenerateSalt(int intLength)
        {
            string strResult = "";

            int intRandomNum;
            Random objRandomClass = new Random();

            for (int i = 1; i <= intLength; i++)
            {
                intRandomNum = Convert.ToInt32(objRandomClass.NextDouble() * 62);
                if (intRandomNum < 10) //from 0 to 9, set 0 to 9
                    intRandomNum = intRandomNum + 48;
                else if (intRandomNum < 36)//from 10 to 35, set A to Z
                    intRandomNum = intRandomNum + 55;
                else //from 36 to 61, set a to z
                    intRandomNum = intRandomNum + 61;
                strResult += (char)intRandomNum;
            }
            return strResult;
        }
    }
}
